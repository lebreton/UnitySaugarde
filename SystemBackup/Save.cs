using SerializeUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class Save
{
    public string Name;
    public byte[] RawImages;
    public DateTime Date;

    private Dictionary<string, object> keysAndValues = new Dictionary<string, object>();

    public Save(string Name, DateTime Date, byte[] RawImages)
    {
        this.Name = Name;
        this.RawImages = RawImages;
        this.Date = Date;
    }

    public Save(byte[] Data)
    {
        int length = 0;
        string key = "";
        object value = null;
        byte[] data = new byte[0];

        using (MemoryStream ms = new MemoryStream(Data))
        {
            using (BinaryReader Reader = new BinaryReader(ms))
            {
                if (Reader.ReadString() != SystemBackup.BuildID)
                    throw new Exception("BuildID is not valide");

                this.Name = Reader.ReadString();

                length = Reader.ReadInt32();
                this.RawImages = Reader.ReadBytes(length);

                length = Reader.ReadInt32();

                for(int i = 0; i< length; i++)
                {
                    key = Reader.ReadString();
                    IFormatter formatter = new BinaryFormatter();
                    length = Reader.ReadInt32();
                    data = Reader.ReadBytes(length);

                    using (MemoryStream MsSerialize = new MemoryStream(data))
                    {
                        value = formatter.Deserialize(MsSerialize);
                    }

                    keysAndValues.Add(key, value);
                 }
            }
        }
    }

    public byte[] GetData()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter Writer = new BinaryWriter(ms))
            {
                Writer.Write((String)SystemBackup.BuildID);
                Writer.Write((String)Name);
                Writer.Write((Int32)RawImages.Length);
                Writer.Write((Byte[])RawImages);
                Writer.Write((Int32)keysAndValues.Count);

                foreach (KeyValuePair<string, object> keyAndValue in keysAndValues)
                {
                    Writer.Write((String)keyAndValue.Key);
                    IFormatter formatter = new BinaryFormatter();
                    using (MemoryStream MsSerialize = new MemoryStream())
                    {
                        formatter.Serialize(MsSerialize, keyAndValue.Value);
                        MsSerialize.Flush();

                        byte[] Data = MsSerialize.ToArray();

                        Writer.Write((Int32)Data.Length);
                        Writer.Write((Byte[])Data);
                    }
                }

                ms.Flush();
                return ms.ToArray();
            }
        }
    }


    public T GetValue<T>(string key)
    {
        Type type = keysAndValues[key].GetType();
        if (!typeof(T).IsSerializable)
        {
            Type[] SerializeType = Serialize.SerializeType;

            foreach (Type stype in SerializeType)
            {
                if (type == stype)
                {
                    SerializeStruct<T> SerializeStruct = (SerializeStruct<T>)keysAndValues[key];
                    return (T)SerializeStruct.ToUnity();
                }
            }

            throw new Exception("Value not DeSerializable !" + type);
        }
        else
        {
            return (T)keysAndValues[key];
        }
    }

    public void SetValue<T>(string key, T value)
    {
        object SerializableForsing = null;

        if (!value.GetType().IsSerializable)
        {
            Type[] SerializeType = Serialize.SerializeType;

            foreach (Type type in SerializeType)
            {
                Type[] types = new Type[1] { value.GetType() };
                ConstructorInfo constructorInfoObj = type.GetConstructor(
                 BindingFlags.Instance | BindingFlags.Public, null,
                 CallingConventions.HasThis, types, null);

                if (constructorInfoObj != null)
                {
                    Debug.Log(constructorInfoObj.Name);
                    SerializableForsing = constructorInfoObj.Invoke(new object[] { value });
                }
            }

            if (SerializableForsing == null)
            {
                throw new Exception("Value not Serializable !");
            }
        }

        if (keysAndValues.ContainsKey(key))
            keysAndValues[key] = SerializableForsing == null ? value : SerializableForsing;
        else
            keysAndValues.Add(key, SerializableForsing == null ? value : SerializableForsing);
    }
}