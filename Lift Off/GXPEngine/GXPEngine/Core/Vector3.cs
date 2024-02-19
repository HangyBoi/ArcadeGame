using System;
using System.CodeDom;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace GXPEngine.Core
{
	public struct Vector3
	{
		public float x;
		public float y;
        public float z;

        public Vector3 (float x, float y, float z)
		{
			this.x = x;
			this.y = y;
            this.z = z;
        }
		public Vector3 Normalized()
		{
			float len = Mathf.Sqrt(x * x + y * y + z * z);
			return new Vector3(x/len, y/len, z/len);
		}
        public float length()
        {
            float len = Mathf.Sqrt(x * x + y * y + z * z);
			return len;
        }

		public float angle(int axis)
		{
			float res = 0;
			if (axis == 0)
			{
                res = Mathf.Acos(Normalized().x);
                if (y < 0) res = -res;
            }
            else if (axis == 1)
			{
                res = Mathf.Acos(Normalized().y);
                if (x < 0) res = -res;
            }
            return res;
		}
        public Vector3 Rotate( Vector3 a)
        {
            Vector3 cosa = new Vector3();
            Vector3 sina = new Vector3();
            cosa.x = Mathf.Cos(a.x); sina.x = Mathf.Sin(a.x);
            cosa.y = Mathf.Cos(a.y); sina.y = Mathf.Sin(a.y);
            cosa.z = Mathf.Cos(a.z); sina.z = Mathf.Sin(a.z);
            Vector3 res = new Vector3(x, y, z);
            if (a.x != 0)
                res = new Vector3(res.x, res.y * cosa.x - res.z * sina.x, res.y * sina.x + res.z * cosa.x);
            if (a.y != 0)
                res = new Vector3(res.x * cosa.y + res.z * sina.y, res.y, res.z * cosa.y - res.x * sina.y);
            if (a.z != 0)
                res = new Vector3(res.x * cosa.z - res.y * sina.z, res.y * cosa.z + res.x * sina.z, res.z);
            return res;
        }
        public static Vector3 operator +(Vector3 v1, Vector3 v2) => new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        public static Vector3 operator -(Vector3 v1, Vector3 v2) => new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        public static Vector3 operator -(Vector3 v2) => new Vector3( - v2.x, - v2.y, - v2.z);
        public static float operator *(Vector3 v1, Vector3 v2) => v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        public static Vector3 operator *(Vector3 v1, float f) => new Vector3(v1.x * f, v1.y * f, v1.z * f);
        public static Vector3 operator *(float f, Vector3 v1) => new Vector3(v1.x * f, v1.y * f, v1.z * f);
        public static Vector3 operator /(Vector3 v1, float f) => new Vector3(v1.x / f, v1.y / f, v1.z / f);
        override public string ToString() {
			return "[Vector3 " + x + ", " + y + ", " + z + "]";
		}
	}
}

