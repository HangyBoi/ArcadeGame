using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class SpriteAnimation : Animation
    {
        public SpriteAnimation (AnimationSprite sprite, float duration) : base(duration)
        {
            
        }
        public override void StartAnimation()
        {
            base.StartAnimation();
        }
        public override void UpdateAnimation()
        {
            base.UpdateAnimation();
        }
        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }
    }
}
