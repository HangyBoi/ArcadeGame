using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class Timer
    {
        public delegate void TimerEnd();
        public event TimerEnd OnTimerEnd;
        private bool enabled;

        public float time;

        public static List<Timer> TimerManager = new List<Timer>();
        public static void Update()
        {
            for (int i = TimerManager.Count() - 1; i>=0; i--)
            {
                Timer timer = TimerManager[i];
                if (timer.time > 0)
                    timer.time -= Time.deltaTime / 1000f;
                else
                {
                    timer.OnTimerEnd?.Invoke();
                    timer.Reset();
                }
            }
        }
        public Timer(bool enabled = false) 
        {
            this.enabled = enabled;
        }
        public void Set (float time)
        { 
            this.time = time; 
        }
        public void Launch()
        {
            enabled = true;
            TimerManager.Add(this);
        }
        public void Pause()
        {
            enabled = false;
            TimerManager.Remove(this);
        }
        public void Reset()
        {
            Pause();
            time = 0;
        }
        public void SetLaunch(float time)
        {
            Set(time);
            Launch();
        }
        public bool GetStatus()
        {
            return enabled;
        }
    }
}
