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
    public string Name = "";
    public byte[] RawImages = new byte[0];
    public DateTime Date = DateTime.Now;

    private Dictionary<string, object> keysAndValues = new Dictionary<string, object>();

    public Save(string Name, DateTime Date)
    {
        this.Name = Name;
        this.Date = Date;
    }

    public void SetTexture(byte[] RawImages)
    {
        this.RawImages = RawImages;
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
                Debug.Log("length :" + length);
                ReadkeysAndValues(Reader, length);
            }
        }
    }

    private void ReadkeysAndValues(BinaryReader Reader, int dlength)
    {
        string key = "";
        object value = null;
        int length = 0;
        byte[] data = new byte[0];

        Action Call = new Action(() =>
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
        });

        if (dlength > 1)
            for (int i = 0; i < dlength - 1; i++)
            {
                Call();
            }
        else
        {
            Call();
        }
    }

    public byte[] GetData()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter Writer = new BinaryWriter(ms))
            {
                Debug.Log("test");
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

                        if (Data.Length > 1024)
                            throw new Exception("if you want to save the whole earth use a database");

                        Writer.Write((Int32)Data.Length);
                        Writer.Write((Byte[])Data);
                    }
                }
                Writer.Write(new byte[1024]);
                ms.Flush();
                return ms.ToArray();
            }
        }
    }


    public T GetValue<T>(string key)
    {
        if (!keysAndValues.ContainsKey(key))
        {
            return default(T);
        }

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
        if (keysAndValues.Count > 500)
            throw new Exception("Se system is not made to save block minecraft");

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