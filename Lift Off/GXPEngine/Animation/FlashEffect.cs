using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class FlashEffect : Animation
    {
        Color startColor;
        Color endColor;
        float startAlpha;
        float endAlpha;
        Sprite sprite;
        public FlashEffect (float duration, Sprite sprite, Color startColor, Color endColor, float startAlpha = 1f, float endAlpha = 0f) : base (duration)
        {
            this.duration = duration;
            this.startColor = startColor;
            this.endColor = endColor;
            this.sprite = sprite;
            this.startAlpha = startAlpha;
            this.endAlpha = endAlpha;
        }

        public override void StartAnimation()
        {
            base.StartAnimation();

            unchecked
            {
                float a = startAlpha;
                float r = (startColor.ToArgb() & (int)0xff0000) >> 16;
                float g = (startColor.ToArgb() & (int)0xff00) >> 8;
                float b = (startColor.ToArgb() & (int)0xff);

                sprite.SetColor(r/255, g/255, b/255);
                sprite.alpha = a;
            }

        }
        public override void UpdateAnimation()
        {
            base.UpdateAnimation();

            unchecked
            {
                float a1 = startAlpha;
                float r1 = (startColor.ToArgb() & 0xff0000) >> 16;
                float g1 = (startColor.ToArgb() & 0xff00) >> 8;
                float b1 = (startColor.ToArgb() & 0xff);

                float a2 = endAlpha;
                float r2 = (endColor.ToArgb() & 0xff0000) >> 16;
                float g2 = (endColor.ToArgb() & 0xff00) >> 8;
                float b2 = (endColor.ToArgb() & 0xff);

                float A = Mathf.Clamp(Mathf.Lerp(a2, a1, timer.time / duration), 0f, 1f);
                float R = (int)Mathf.Lerp(r2, r1, timer.time / duration);
                float G = (int)Mathf.Lerp(g2, g1, timer.time / duration);
                float B = (int)Mathf.Lerp(b2, b1, timer.time / duration);

                sprite.SetColor(R/255, G/255, B/255);
                Console.WriteLine(A);
                sprite.alpha = A;
            }
            
        }
        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();

            float a = endAlpha;
            float r = (endColor.ToArgb() & 0xff0000) >> 16;
            float g = (endColor.ToArgb() & 0xff00) >> 8;
            float b = (endColor.ToArgb() & 0xff);

            sprite.SetColor(r/255, g/255, b/255);
            sprite.alpha = a;
        }

        public void SetStartColor(Color color)
        {
            startColor = color;
        }

        public void SetEndColor(Color color)
        {
            endColor = color;
        }
    }
}
