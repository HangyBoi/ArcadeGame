using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class LightningAnimation : Animation
    {

        public delegate void VoidEvent();
        public event VoidEvent OnStrike;
        private int strikes;
        public LightningAnimation(float duration) : base(duration)
        {
        }
        public override void StartAnimation()
        {
            base.StartAnimation();
            Strike();
        }
        public override void UpdateAnimation()
        {
            base.UpdateAnimation();
            if (timer.time < duration / 3 * 2 && strikes == 1)
                Strike();
            if (timer.time < duration / 3 && strikes == 2)
                Strike();
        }
        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }

        public void Strike()
        {
            strikes++;
            Console.WriteLine(strikes);
            OnStrike?.Invoke();
            List<Enemy> possibleTargets = new List<Enemy>();
            ScreenShake ss = new ScreenShake(0.4f, 30, MyGame.self.cam, 0.01f);
            ss.StartAnimation();
            for (int i=0; i < 3; i++)
            {
                foreach (Enemy enemy in Enemy.collection[i])
                {
                    possibleTargets.Add(enemy);
                }
            }
            if (possibleTargets.Count > 0)
            {
                Enemy target = possibleTargets[Utils.Random(0, possibleTargets.Count)];
                target.Score(Shape.LIGHTNING);
                target.Damage();
            }
        }
    }
}
