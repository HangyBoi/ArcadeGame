using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Animation
{
    public class SpellVFX : Animation
    {
        /// <summary>
        /// 0b00000001 - kill
        /// 0b00000010 - double hit
        /// 0b00000100 - tripple hit
        /// 0b00001000 - lucky shot
        /// 0b00010000
        /// 0b00100000
        /// 0b01000000
        /// 0b10000000
        /// </summary>
        uint flags = 0x00000000;


        public float x, y;
        public ParticleSystem particles;
        public GameObject target;
        //public Sprite bomb;
        public Vector2 startPos;
        public Vector2 velocity;
        public Vector2 endPos;
        public float gravity = 5000f;
        Shape shape;
        public SpellVFX(float time, GameObject parent, Shape shape, uint flogs = 0) : base(time)
        {
            flags = flogs;
            this.shape = shape;
            particles = new ParticleSystem("Assets\\glow.png", 0, 0, ParticleSystem.EmitterType.rect, ParticleSystem.Mode.velocity, parent);
            particles.blendMode = BlendMode.ALPHABLEND;
            particles.spawnPeriod = 0.003f;

            switch (shape)
            {
                case Shape.RED:
                    particles.startColor = Color.FromArgb(0xff3333);
                    break;
                case Shape.BLUE:
                    particles.startColor = Color.FromArgb(0x3399ff);
                    break;
                case Shape.GREEN:
                    particles.startColor = Color.FromArgb(0x33ff33);
                    break;
                case Shape.YELLOW:
                    particles.startColor = Color.FromArgb(0xffff33);
                    break;
            }

            particles.endColor = Color.FromArgb(0x000000);
            particles.startAlpha = 0.6f;
            particles.endAlpha = 0f;
            particles.startSize = 2f;
            particles.startSizeDelta = 0.2f;
            particles.endSize = 0.1f;
            particles.startPosDelta = new Vector2(10, 10);
            particles.lifetime = 0.3f;
            particles.lifetimeDelta = 0.2f;
            particles.startSpeed = new Vector2(0, 0);
            particles.startSpeedDelta = new Vector2(2f, 2f);
            particles.endSpeed = new Vector2(0, 0);
            particles.endSpeedDelta = new Vector2(0.5f, 0.5f);
            particles.enabled = false;
            particles.startAngleDelta = 360;
            particles.endAngleDelta = 360;

            parent.AddChild(particles);

        }
        public void SetTrajectory(Vector2 startPos, GameObject target)
        {
            this.startPos = startPos;
            this.target = target;

            particles.x = startPos.x;
            particles.y = startPos.y;

            velocity = new Vector2(0, -1000f);
            velocity.Rotate(Utils.Random(0f, 360f));
            //velocity = (new Vector2(target.x, target.y) - startPos).Normalized() * 2000f;
            //duration = (endPos - startPos).length() / velocity.length();

        }

        public void SetTrajectory(Vector2 startPos, Vector2 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;

            particles.x = startPos.x;
            particles.y = startPos.y;

            velocity = (endPos - startPos).Normalized() * 2000f;
            //duration = (endPos - startPos).length() / velocity.length();

        }
        public override void StartAnimation()
        {
            base.StartAnimation();
            particles.enabled = false;
            //SetTrajectory(new Vector2(-300, -100), new Vector2(300, 0), 0.5f);
        }
        public override void UpdateAnimation()
        {
            base.UpdateAnimation();

            particles.enabled = true;
            particles.Update();

            particles.x += velocity.x * Time.deltaTime / 1000;
            particles.y += velocity.y * Time.deltaTime / 1000;

            if (target != null)
            {
                if (target is Enemy)
                {
                    Enemy enemy = (Enemy)target;
                    if (enemy.isDead)
                        target = null;
                    else if (enemy.shapes.Count > 0)
                        if (enemy.shapes[0] != shape)
                            target = null;
                }
            }
            else
            {
                foreach (GameObject go in particles.parent.GetChildren())
                {
                    if (go is Enemy)
                    {
                        Enemy enemy = (Enemy)go;
                        if (enemy.shapes.Count > 0)
                        {
                            if (enemy.shapes[0] == shape)
                            {
                                target = enemy;
                                flags |= 0b1000;
                                break;
                            }
                        }
                    }
                }
            }
            
            if (target != null)
            {
                timer.time = 1;
                Vector2 targetPos = new Vector2(target.x, target.y);
                Vector2 pos = new Vector2(particles.x, particles.y);
                float r = (targetPos - pos).length();
                velocity += (targetPos - pos)*50000f/r/r;
                if (velocity.length() > 2000)
                    velocity = velocity.Normalized() * 2000;
                particles.x = Mathf.Lerp(pos.x, targetPos.x, Mathf.Min(Time.deltaTime / (r), 1f));
                particles.y = Mathf.Lerp(pos.y, targetPos.y, Mathf.Min(Time.deltaTime / (r), 1f));
                if (r < 10)
                {
                    timer.time = 0;
                }
            }

        }
        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            //particles.DestroyAllParticles();
            particles.LateDestroy();
            if (target is Enemy)
            {
                Enemy enemy = (Enemy)target;
                if (enemy.shapes.Count == 1)
                    flags |= 0b1;
                if (!enemy.isDead)
                {
                    enemy.Score(shape, flags);
                    enemy.Damage(shape);

                    if ((flags & 0b1) != 0)
                    {
                        Vector2 pos = Utils.Random(new Vector2(enemy.x, enemy.y), new Vector2(50, 100));
                        PopupAnimation popup = new PopupAnimation("KILL", 0.8f, pos.x, pos.y, MyGame.self.lineLayers[enemy.line], MyGame.self.GetComboColor());
                        popup.StartAnimation();
                    }
                    if ((flags & 0b10) != 0)
                    {
                        Vector2 pos = Utils.Random(new Vector2(enemy.x, enemy.y), new Vector2(50, 100));
                        PopupAnimation popup = new PopupAnimation("DOUBLE!", 0.8f, pos.x, pos.y, MyGame.self.lineLayers[enemy.line], MyGame.self.GetComboColor());
                        popup.StartAnimation();
                    }

                    if ((flags & 0b100) != 0)
                    {
                        Vector2 pos = Utils.Random(new Vector2(enemy.x, enemy.y), new Vector2(50, 100));
                        PopupAnimation popup = new PopupAnimation("TRIPPLE!!", 0.8f, pos.x, pos.y, MyGame.self.lineLayers[enemy.line], MyGame.self.GetComboColor());
                        popup.StartAnimation();
                    }

                    if ((flags & 0b1000) != 0)
                    {
                        Vector2 pos = Utils.Random(new Vector2(enemy.x, enemy.y), new Vector2(50, 100));
                        PopupAnimation popup = new PopupAnimation("LUCKY SHOT!", 0.8f, pos.x, pos.y, MyGame.self.lineLayers[enemy.line], MyGame.self.GetComboColor());
                        popup.StartAnimation();
                    }
                }
            }
        }
    }
}
