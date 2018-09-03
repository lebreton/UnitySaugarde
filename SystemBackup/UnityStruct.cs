using SerializeUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerializeUnity
{
    public static class Serialize
    {
        public static Type[] SerializeType = new Type[] {
           typeof(Vector3<float>),
           typeof(Vector2<float>),
           typeof(Quaternion<float>),
        };

        public static Vector3<float> ToSerialize(this UnityEngine.Vector3 Vec)
        {
            return new Vector3<float>(Vec);
        }

        public static Vector2<float> ToSerialize(this UnityEngine.Vector2 Vec)
        {
            return new Vector2<float>(Vec);
        }

        public static Quaternion<float> ToSerialize(this UnityEngine.Quaternion Qua)
        {
            return new Quaternion<float>(Qua);
        }
    }


    [Serializable]
    public abstract class SerializeStruct<T>
    {
        public abstract T ToUnity();
    }


    [Serializable]
    public class Vector3<T> : SerializeStruct<UnityEngine.Vector3>
    {
        public T x;
        public T y;
        public T z;

        public Vector3(UnityEngine.Vector3 Vec)
        {
            this.x = (T)(object)Vec.x;
            this.y = (T)(object)Vec.y;
            this.z = (T)(object)Vec.z;
        }

        public override UnityEngine.Vector3 ToUnity()
        {
            return new UnityEngine.Vector3(
                (float)(object) this.x, 
                (float)(object)this.y, 
                (float)(object)this.z
            );
        }
    }

  
    [Serializable]
    public class Vector2<T> : SerializeStruct<UnityEngine.Vector2>
    {
        public T x;
        public T y;

        public Vector2(UnityEngine.Vector2 Vec)
        {
            this.x = (T)(object)Vec.x;
            this.y = (T)(object)Vec.y;
        }

        public override UnityEngine.Vector2 ToUnity()
        {
            return new UnityEngine.Vector2(
                (float)(object)this.x, 
                (float)(object)this.y
             );
        }
    }

    [Serializable]
    public class Quaternion<T> : SerializeStruct<UnityEngine.Quaternion>
    {
        public T x;
        public T y;
        public T z;
        public T w;

        public Quaternion(UnityEngine.Quaternion Qua)
        {
            this.x = (T)(object)Qua.x;
            this.y = (T)(object)Qua.y;
            this.z= (T)(object)Qua.z;
            this.w = (T)(object)Qua.w;
        }

        public override UnityEngine.Quaternion ToUnity()
        {
            return new UnityEngine.Quaternion(
                (float)(object)this.x, 
                (float)(object)this.y, 
                (float)(object)this.z, 
                (float)(object)this.w
             );
        }
    }

}