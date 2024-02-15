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
        private direction[] pattern;
        private bool[] activated;
        private bool reversed = false;
        private int nextSegment;
        public bool completed = false;

        public MagicShape(Vector2[] points, direction[] pattern)
        {
            if (points.Length != pattern.Length + 1)
                throw new Exception("u gay");
            this.points = points;
            this.pattern = pattern;
            completed = false;
            this.activated = new bool[points.Length];
            for (int i = 0; i < pattern.Length; i++)
                activated[i] = false;
            this.pattern = pattern;

            PositionParser.OnPlayerInput += CheckSegment;
        }
        public void CheckSegment(direction dir)
        {
            if (completed) return;
            if (!activated[pattern.Length - 1] && !activated[0])
            {
                if (pattern[0] == dir)
                {
                    nextSegment = 1;
                    activated[0] = true;
                }
                if (pattern[pattern.Length - 1].Opposite() == dir)
                {
                    nextSegment = pattern.Length - 2;
                    activated[pattern.Length - 1] = true;
                    reversed = true;
                }
            }
            else
            {
                if (reversed)
                {
                    if (dir == pattern[nextSegment].Opposite())
                    {
                        activated[nextSegment] = true;
                        nextSegment--;
                        if (nextSegment == -1)
                            completed = true;
                    }
                }
                else if (dir == pattern[nextSegment])
                {
                    activated[nextSegment] = true;
                    nextSegment++;
                    if (nextSegment == pattern.Length)
                        completed = true;
                }
            }
            Console.WriteLine(nextSegment);
        }
        public void Reset()
        {
            completed = false;
            nextSegment = 0;
            reversed = false;
            for (int i = 0; i < activated.Length; i++)
                activated[i] = false;
        }

        public void Draw(EasyDraw canvas)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                Vector2 p1 = canvas.InverseTransformPoint(points[i].x, points[i].y);
                Vector2 p2 = canvas.InverseTransformPoint(points[i+1].x, points[i+1].y);
                if (activated[i])
                    canvas.Stroke(0, 255, 0);
                if (completed)
                    canvas.Stroke(0, 255, 255);
                canvas.Line(p1.x, p1.y, p2.x, p2.y);
                canvas.Stroke(255,255,255);
            }
        }
    }
}
