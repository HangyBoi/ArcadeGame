using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class ScreenShake : Animation
    {
        float amplitude = 0;
        float shakePeriod = 0.1f;
        float smoothness = 10;
        private Vector2 currentOffset = Vector2.zero;
        private Vector2 targetOffset = Vector2.zero;
        GameObject camera;
        Timer shakeTimer;
        public ScreenShake(float duration, float amplitude, GameObject cam, float shakePeriod = 0.1f, float smoothness = 0.1f) :base(duration) 
        {
            this.amplitude = amplitude;
            this.shakePeriod = shakePeriod;
            camera = cam;
            shakeTimer = new Timer();
        }
        public override void StartAnimation()
        {
            base.StartAnimation();
            shakeTimer.SetLaunch(shakePeriod);
            shakeTimer.OnTimerEnd += RefreshShake;

            targetOffset = new Vector2(amplitude, 0);
            targetOffset.Rotate(Utils.Random(Mathf.PI / 2, Mathf.PI * 3 / 2));
        }
        public override void UpdateAnimation()
        {
            base.UpdateAnimation();
            currentOffset = currentOffset.Lerp(targetOffset, smoothness * Time.deltaTime / 1000f);
            camera.x = currentOffset.x;
            camera.y = currentOffset.y;
        }
        public void RefreshShake()
        {
            shakeTimer.SetLaunch(shakePeriod);
            targetOffset.Rotate(Utils.Random(Mathf.PI / 2, Mathf.PI * 3 / 2));
            targetOffset = targetOffset.Normalized() * amplitude * timer.time / duration;
        }
        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            shakeTimer.OnTimerEnd -= RefreshShake;
            camera.x = 0; 
            camera.y = 0;
        }
    }
}
