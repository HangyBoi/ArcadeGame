using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class LightningEffect : Animation
    {
        public GameObject worldSpace;
        public Vector2 target;
        public AnimationSprite lightningSprite;
        public LightningEffect(float duration, GameObject worldSpace, Vector2 target) : base(duration)
        {
            this.worldSpace = worldSpace;
            this.target = target;
            lightningSprite = new AnimationSprite("Assets/lightning.png",8,1);
            lightningSprite.SetOrigin(lightningSprite.width/2, lightningSprite.height);
        }
        public override void StartAnimation()
        {
            base.StartAnimation();
            worldSpace.AddChild(lightningSprite);
            lightningSprite.x = target.x;
            lightningSprite.y = target.y;
        }
        public override void UpdateAnimation()
        {
            base.UpdateAnimation();
            lightningSprite.Animate(0.1f);
            if (lightningSprite.currentFrame == lightningSprite.frameCount - 1)
                worldSpace.RemoveChild(lightningSprite);
        }
        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            if (worldSpace.HasChild(lightningSprite))
                worldSpace.RemoveChild(lightningSprite);
        }
    }
}
