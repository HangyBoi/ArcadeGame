using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public static class SDF
    {
        public static float Line(in Vector2 p, in Vector2 a, in Vector2 b)
        {
            Vector2 pa = p - a, ba = b - a;
            float h = Mathf.Clamp(pa*ba / (ba*ba), 0.0f, 1.0f);
            return (pa - ba * h).length();
        }
    }
}
