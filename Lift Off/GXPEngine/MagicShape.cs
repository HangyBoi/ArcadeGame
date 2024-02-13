using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GXPEngine
{
    public class MagicShape
    {
        static public float precision = 100;
        private Vector2[] points;
        private bool[] activated;
        public bool failed = false;

        public MagicShape(Vector2[] points)
        {
            this.points = points;
            this.activated = new bool[points.Length];
            for (int i=0; i<points.Length; i++)
                activated[i] = false;
        }
        public float ShapeSDF (Vector2 pos)
        {
            float min = 1000;
            for (int i = 0; i < points.Count() - 1; i++)
            {
                float segmentDistance = SDF.Line(pos, points[i], points[i + 1]);
                if (segmentDistance < min)
                    min = segmentDistance;
            }
            return min;
        }
        public float PointSDF(Vector2 pos, int i)
        {
            return (pos - points[i]).length();
        }
        public void CheckPoints(Vector2 pos)
        {
            for (int i = 0; i< points.Length; i++)
            {
                if (PointSDF(pos, i) < precision)
                {
                    if (i == 0 || i == points.Length - 1)
                    {
                        if (i == 0)
                            if (activated[1] || !activated[points.Length - 1])
                                activated[0] = true;
                        if (i == points.Length - 1)
                            if (activated[points.Length - 2] || !activated[0])
                                activated[points.Length - 1] = true;
                    }
                    else if (activated[i-1] || activated[i+1])
                        activated[i] = true;
                }
            }
        }
        public void Reset()
        {
            for (int i = 0; i < activated.Length; i++)
                activated[i] = false;
        }

        public void Draw(EasyDraw canvas)
        {
            for (int i = 0; i < points.Count() - 1; i++)
            {
                Vector2 p1 = canvas.InverseTransformPoint(points[i].x, points[i].y);
                Vector2 p2 = canvas.InverseTransformPoint(points[i+1].x, points[i+1].y);
                if (activated[i] && activated[i + 1])
                    canvas.Stroke(0, 255, 0);
                if (failed)
                    canvas.Stroke(255, 0, 0);
                canvas.Line(p1.x, p1.y, p2.x, p2.y);
                canvas.Stroke(255,255,255);
            }
        }
    }
}
