using System;
using System.CodeDom;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace GXPEngine.Core
{
	public struct Vector2
    {
        public static Vector2 zero = new Vector2(0, 0);

        public float x;
		public float y;
		
		public Vector2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		public Vector2 Normalized()
		{
			float len = Mathf.Sqrt(x * x + y * y);
			return new Vector2(x/len, y/len);
		}
        public float length()
        {
            float len = Mathf.Sqrt(x * x + y * y);
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
        public void Rotate(float angle)
        {
			float newx = Mathf.Cos(angle) * x - Mathf.Sin(angle) * y;
            float newy = Mathf.Sin(angle) * x + Mathf.Cos(angle) * y;
			x = newx;
			y = newy;
        }
        public Vector2 Lerp(Vector2 end, float fac)
        {
			Vector2 res = new Vector2();
			res.x = Mathf.Lerp(x, end.x, fac);
			res.y = Mathf.Lerp(y, end.y, fac);
			return res;
        }

        public Vector2 MatrixTransform(float[,] matrix)
        {
			if (matrix.GetLength(0) != 2 && matrix.GetLength(1) != 2)
				throw new Exception("u need 2x2 matrix hahaha u had math issue");
			return new Vector2(x * matrix[0,0] + y * matrix[0,1], x * matrix[1,0] + y * matrix[1,1]);
        }
        public static Vector2 operator +(Vector2 v1, Vector2 v2) => new Vector2(v1.x + v2.x, v1.y + v2.y);
        public static Vector2 operator -(Vector2 v1, Vector2 v2) => new Vector2(v1.x - v2.x, v1.y - v2.y);
        public static Vector2 operator -(Vector2 v2) => new Vector2( - v2.x, - v2.y);
        public static float operator *(Vector2 v1, Vector2 v2) => v1.x * v2.x + v1.y * v2.y;
        public static Vector2 operator *(Vector2 v1, float f) => new Vector2(v1.x * f, v1.y * f);
        public static Vector2 operator *(float f, Vector2 v1) => new Vector2(v1.x * f, v1.y * f);
        public static Vector2 operator /(Vector2 v1, float f) => new Vector2(v1.x / f, v1.y / f);
        override public string ToString() {
			return "[Vector2 " + x + ", " + y + "]";
		}
	}
}

