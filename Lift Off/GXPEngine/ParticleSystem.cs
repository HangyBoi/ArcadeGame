using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GXPEngine
{
    public partial class ParticleSystem : GameObject
    {
        internal class Particle : Sprite
        {
            public ParticleSystem ps;
            public Mode mode = Mode.position;
            public Particle(Texture2D texture) : base(texture, false)
            {
            }
            public Particle(string filename, bool keepInCache = true, bool addCollider = false) : base(filename, keepInCache, addCollider)
            {
                
            }
            public float lifetime;
            public float totaltime;
            public Vector2 spawnPos;
            public Vector2 endPos;

            public Vector2 startSpeed;
            public Vector2 endSpeed;

            public Color startColor;
            public Color endColor;

            public float startSize;
            public float endSize;

            public float startAlpha;
            public float endAlpha;

            public float startAngle;
            public float endAngle;

            public Vector2 speed = new Vector2 (0,0);

            public override void Update()
            {
                lifetime += Time.deltaTime / 1000f;
                float fac = lifetime / totaltime;
                switch (mode)
                {
                    case Mode.position:
                        x = Mathf.Lerp(spawnPos.x, endPos.x, fac);
                        y = Mathf.Lerp(spawnPos.y, endPos.y, fac);
                        break;
                    case Mode.velocity:
                        speed = new Vector2(Mathf.Lerp(startSpeed.x, endSpeed.x, fac), Mathf.Lerp(startSpeed.y, endSpeed.y, fac));
                        break;
                    case Mode.force:
                        foreach (Force f in ps.forces)
                        {
                            Vector2 force = f.Calculate(new Vector2(x, y)) * Time.deltaTime;
                            if (force.length() > 10)
                                force = force.Normalized() * 10f;
                            speed += force;
                        }
                        break;
                }
                x += speed.x;
                y += speed.y;

                SetColor(
                    (byte)Mathf.Lerp(startColor.R, endColor.R, fac),
                    (byte)Mathf.Lerp(startColor.G, endColor.G, fac),
                    (byte)Mathf.Lerp(startColor.B, endColor.B, fac));

                rotation = Mathf.Lerp(startAngle, endAngle, fac);
                scale = Mathf.Lerp(startSize, endSize, fac);
                alpha = Mathf.Lerp(startAlpha, endAlpha, fac);

                if (totaltime < lifetime)
                {
                    ps.particleCount--;
                    Destroy();
                }
            }
        }
        public enum ForceType
        { 
            Gravity = 0,
            Radial = 1,
            Turbulence = 2,
            Custom = 3,
        }
        public enum EmitterType
        {
            rect = 0,
            circle = 1
        }
        public enum Mode
        {
            force = 0,
            position = 1,
            velocity = 2,
        }
        public GameObject worldSpace = null;
        public BlendMode blendMode = BlendMode.NORMAL;
        public EmitterType emitterType;
        public Mode mode;
        public ParticleSystem(string path, float x, float y, EmitterType emitter, Mode mode = Mode.position, GameObject worldSpace = null) : base (false)
        {
            forces = new List<Force>();
            particles = new List<Particle>();
            this.texturePath = path;
            this.emitterType = emitter;
            this.mode = mode;
            this.y = y;
            this.x = x;

            if (worldSpace == null)
                this.worldSpace = game;
            else
                this.worldSpace = worldSpace;
        }
        public float radius;
        public Vector2 size;

        public string texturePath = "circle.png";
        private List<Particle> particles = null;
        public List<Force> forces = null;
        public bool enabled = true;
        public int particleCount = 0;

        public float lifetime = 0.5f;
        public float lifetimeDelta = 0.1f;

        public int maxParticleCount = 100;
        public float spawnPeriod = 0.3f;
        private float spawnCooldown = 0;

        public Vector2 startSpeed = new Vector2(0,0);
        public Vector2 startSpeedDelta = new Vector2(0, 0);
        public Vector2 endSpeed = new Vector2(5,5);
        public Vector2 endSpeedDelta = new Vector2(5, 5);

        public Vector2 startPos = new Vector2(0,0);
        public Vector2 startPosDelta = new Vector2(0, 0);
        public Vector2 endPos = new Vector2(100,100);
        public Vector2 endPosDelta = new Vector2(100, 100);

        public float startAngle = 0;
        public float startAngleDelta = 0;
        public float endAngle = 0;
        public float endAngleDelta = 0;
        public float startAngularSpeed = 0;
        public float endAngularSpeed = 0;

        public Color startColor = Color.White;
        public Color endColor = Color.Blue;

        public float startSize = 2f;
        public float startSizeDelta = 0f;
        public float endSize = 1f;
        public float endSizeDelta = 0f;

        public float startAlpha = 1f;
        public float endAlpha = 0f;

        public void SpawnParticle()
        {
            Particle p = new Particle(texturePath);
            particles.Add(p);
            worldSpace.AddChild(p);
            p.ps = this;
            particleCount++;
            p.mode = mode;
            p.lifetime = 0;

            p.SetOrigin(p.width / 2, p.height / 2);
            p.startAngle = Utils.Random(startAngle - startAngleDelta, startAngle + startAngleDelta);
            p.endAngle = Utils.Random(endAngle -endAngleDelta, endAngle + endAngleDelta);

            p.spawnPos = Utils.Random(worldSpace.InverseTransformVector(TransformPoint(startPos.x, startPos.y)), startPosDelta);
            p.x = p.spawnPos.x; p.y = p.spawnPos.y;

            p.totaltime = Utils.Random(lifetime - lifetimeDelta, lifetime + lifetimeDelta);
            p.blendMode = blendMode;

            switch(mode)
            {
                case Mode.position:
                    p.endPos = Utils.Random(endPos, endPosDelta);
                    p.speed = new Vector2(0,0);
                    break;
                case Mode.velocity:
                    p.startSpeed = Utils.Random(startSpeed, startSpeedDelta);
                    p.endSpeed = Utils.Random(endSpeed, endSpeedDelta);
                    break;
                case Mode.force:
                    p.speed = Utils.Random(startSpeed, startSpeedDelta);
                    break;
            }
            p.startColor = startColor; p.endColor = endColor;
            p.startAlpha = startAlpha; p.endAlpha = endAlpha;
            p.startSize = Utils.Random(startSize- startSizeDelta, startSize + startSizeDelta); p.endSize = Utils.Random(endSize - endSizeDelta, endSize + endSizeDelta);

            p.SetColor(startColor.R, startColor.G, startColor.B);
            p.alpha = startAlpha;
            p.scale = startSize;
        }
        public override void Update()
        {
            if (Input.GetKeyDown(Key.N))
            {
                enabled = !enabled;
            }
            if (enabled)
            {
                spawnCooldown += Time.deltaTime / 1000f;
                int particlesGenerated = (int)(spawnCooldown / spawnPeriod);
                spawnCooldown %= spawnPeriod;
                for (int i = 0; i < particlesGenerated; i++)
                {
                    SpawnParticle();
                }
                
            }
        }

        public void DestroyAllParticles()
        {
            for (int i=particles.Count - 1; i>=0; i--)
            {
                particles[i].LateDestroy();
                particles.RemoveAt(i);
            }
        }
    }
}
