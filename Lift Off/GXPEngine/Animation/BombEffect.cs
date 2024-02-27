using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GXPEngine.Core;

namespace GXPEngine.Animation
{
    public class BombEffect : Animation
    {
        public float x,y;
        public ParticleSystem explosion;
        public Sprite bomb;
        public Vector2 startPos;
        public Vector2 bombVelocity;
        public Vector2 endPos;
        public float gravity = 5000f;
        public float airTime;
        public BombEffect(GameObject parent) : base(2f)
        {
            bomb = new Sprite("Assets\\bubble.png");
            bomb.scale = 3f;
            bomb.color = 0xff00ff;

            explosion = new ParticleSystem("Assets\\smoke.png",0,0,ParticleSystem.EmitterType.rect,ParticleSystem.Mode.velocity,parent);
            explosion.spawnPeriod = 0.0003f; 
            //explosion.blendMode = BlendMode.ADDITIVE;
            explosion.startColor = Color.Magenta;
            explosion.endColor = Color.Gray;
            explosion.startAlpha = 1f;
            explosion.endAlpha = 0f;
            explosion.startSize = 0.3f;
            explosion.startSizeDelta = 0.2f;
            explosion.endSize = 0.1f;
            explosion.startPosDelta = new Vector2(20, 20);
            explosion.lifetime = 1.5f;
            explosion.lifetimeDelta = 0.5f;
            explosion.startSpeed = new Vector2(0, -1f);
            explosion.startSpeedDelta = new Vector2(1f, 0.5f);
            explosion.endSpeed = new Vector2(0, 0f);
            explosion.endSpeedDelta = new Vector2(0.1f,0.1f);
            explosion.enabled = false;

            parent.AddChild(bomb);
            bomb.AddChild(explosion);
        }
        public void SetTrajectory(Vector2 startPos, Vector2 endPos, float airTime)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            this.airTime = airTime;
            bomb.x = startPos.x;
            bomb.y = startPos.y;

            bombVelocity.x = (endPos.x - startPos.x) / airTime;
            bombVelocity.y = (endPos.y - startPos.y) / airTime - gravity * airTime / 2;
            
        }
        public override void StartAnimation()
        {
            base.StartAnimation();
            Console.WriteLine("boop");
            explosion.enabled = false;
            //SetTrajectory(new Vector2(-300, -100), new Vector2(300, 0), 0.5f);
            bomb.alpha = 1f;
        }
        public override void UpdateAnimation()
        {
            base.UpdateAnimation();

            if (timer.time > 2 - airTime)
            {
                explosion.SetXY(0, 0);
                bomb.alpha = 1f;
                bombVelocity.y += gravity * Time.deltaTime/1000;
                bomb.y += bombVelocity.y * Time.deltaTime/1000;
                bomb.x += bombVelocity.x * Time.deltaTime/1000;

            }
            if (timer.time > 2f-airTime-0.03f && timer.time < 2f - airTime)
            {
                explosion.SetXY(0, 0);
                explosion.enabled = true;
                bomb.alpha = 0f;
                explosion.Update();
            }
            else
            {
                explosion.SetXY(0, 0);
                explosion.enabled = false;
            }
        }
        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            bomb.LateDestroy();
            explosion.LateDestroy();
        }
    }
}
