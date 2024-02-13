using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    partial class ParticleSystem
    {
        public class Force
        {
            public ForceType type;
            public float magnitude = 1f;

            public virtual Vector2 Calculate(Vector2 pos)
            { return new Vector2(0, 0); }
        }


        public class GravityForce : Force
        {
            public Vector2 direction;

            public GravityForce(Vector2 dir)
            {
                type = ForceType.Gravity;
                direction = dir.Normalized();
            }
            public override Vector2 Calculate(Vector2 pos)
            {
                return direction * magnitude;
            }
        }

        public class RadialForce : Force
        {
            public Vector2 affectorPos;

            public RadialForce(Vector2 affectorPos)
            {
                type = ForceType.Radial;
                this.affectorPos = affectorPos;
            }
            public override Vector2 Calculate(Vector2 pos)
            {
                float r = (pos - affectorPos).length();
                return magnitude / r / r / r * (pos - affectorPos);
            }
        }
    }
}
