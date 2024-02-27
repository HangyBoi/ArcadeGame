using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;

namespace GXPEngine.Animation
{
    public class PopupAnimation : Animation
    {
        public Sprite sprite;
        public GameObject worldSpace;


        public PopupAnimation (Sprite sprite, float duration, GameObject parent = null, GameObject worldSpace = null, Color color = default) : base(duration)
        {
            this.sprite = sprite;
            sprite.color = (uint)color.ToArgb();
            sprite.x = parent == null ? 0 : parent.x;
            sprite.y = parent == null ? 0 : parent.y;
            this.worldSpace = worldSpace == null ? MyGame.self : worldSpace;
        }
        public PopupAnimation (Sprite sprite, float duration, float x = 0, float y = 0, GameObject worldSpace = null, Color color = default) : base(duration)
        {
            this.sprite = sprite;
            sprite.color = (uint)color.ToArgb();
            sprite.x = x;
            sprite.y = y;
            this.worldSpace = worldSpace == null ? MyGame.self : worldSpace;
        }

        public PopupAnimation(string text, float duration, float x = 0, float y = 0, GameObject worldSpace = null, Color color = default) : base(duration)
        {
            sprite = PopupSprites.CreateLabel(text);
            sprite.color = (uint)color.ToArgb();
            sprite.x = x;
            sprite.y = y;
            this.worldSpace = worldSpace == null ? MyGame.self : worldSpace;
        }
        public PopupAnimation(string text, float duration, GameObject parent = null, GameObject worldSpace = null, Color color = default) : base(duration)
        {
            sprite = PopupSprites.CreateLabel(text);
            sprite.color = (uint)color.ToArgb();
            sprite.x = parent == null ? 0 : parent.x;
            sprite.y = parent == null ? 0 : parent.y;
            this.worldSpace = worldSpace == null ? MyGame.self : worldSpace;
        }
        public override void StartAnimation()
        {
            sprite.alpha = 1f;
            worldSpace.AddChild(sprite);
            base.StartAnimation();
        }
        public override void UpdateAnimation()
        {
            sprite.alpha = Mathf.Clamp(timer.time / duration,0,1);
            sprite.Move(0, -timer.time * 100 * Time.deltaTime/1000f);
            base.UpdateAnimation();
        }
        public override void OnAnimationEnd()
        {
            worldSpace.RemoveChild(sprite);
            base.OnAnimationEnd();
        }
    }
}
