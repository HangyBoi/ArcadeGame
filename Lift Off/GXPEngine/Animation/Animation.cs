using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class Animation
    {
        public static List<Animation> AnimationManager = new List<Animation>();
        public static void Update()
        {
            List<Animation> temp = new List<Animation>();
            foreach (Animation animation in AnimationManager)
                temp.Add(animation);
            foreach (Animation animation in temp)
            {
                animation.UpdateAnimation();
            }
        }

        public Timer timer = new Timer(false);
        public float duration;
        public bool isRunning;

        public Animation(float duration)
        {
            this.duration = duration;
            timer.OnTimerEnd += OnAnimationEnd;
        }
        public virtual void StartAnimation()
        {
            timer.SetLaunch(duration);
            isRunning = true;
            if (!AnimationManager.Contains(this))
                AnimationManager.Add(this);
        }
        public virtual void UpdateAnimation()
        {
            
        }
        public virtual void OnAnimationEnd()
        {
            timer.Reset();
            isRunning = false;
            if (AnimationManager.Contains(this))
                AnimationManager.Remove(this);
        }
    }
}
