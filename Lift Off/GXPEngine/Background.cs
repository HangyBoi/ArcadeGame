using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace GXPEngine
{
    public static class Environment
    {
        public static ParticleSystem fireflies;
        public static void Setup()
        {
            PutFirefliesOnLayer(MyGame.self.lineLayers[0], 0.3f, Color.FromArgb(255, 255, 255));
            PutFirefliesOnLayer(MyGame.self.lineLayers[1], 0.6f, Color.FromArgb(255, 255, 200));
            PutFirefliesOnLayer(MyGame.self.lineLayers[2], 0.9f, Color.FromArgb(255, 255, 150));
        }
        private static void PutFirefliesOnLayer(GameObject layer, float brightness, Color color)
        {

            fireflies = new ParticleSystem("Assets/glow.png", 0, 0, ParticleSystem.EmitterType.rect, ParticleSystem.Mode.velocity, layer);
            fireflies.blendMode = BlendMode.ALPHABLEND;
            fireflies.size = new Vector2(MyGame.self.width, MyGame.self.height);
            fireflies.startPos = new Vector2(0, 0);
            fireflies.startPosDelta = new Vector2(MyGame.self.width, MyGame.self.height);
            fireflies.startSpeed = new Vector2(0, 0);
            fireflies.startSpeedDelta = new Vector2(0.5f, 0.5f);
            fireflies.endSpeed = new Vector2(0, 0);
            fireflies.endSpeedDelta = new Vector2(0.5f, 0.5f);
            fireflies.lifetime = 10f;
            fireflies.lifetimeDelta = 5f;
            fireflies.spawnPeriod = 0.5f;
            fireflies.startAlpha = 1.0f;
            fireflies.endAlpha = 0f;
            fireflies.startSize = 15f;
            fireflies.startSizeDelta = 1f;
            fireflies.endSize = 10f;
            fireflies.endSizeDelta = 1f;
            fireflies.startColor = color;
            fireflies.endColor = color;
            fireflies.alphaCurve = fac => { return fac * (1 - fac) * brightness; };

            MyGame.self.Add(fireflies);
        }
        public static void Update()
        {
            //foreach (GameObject go in fireflies.parent.GetChildren())
            //{
            //    if (go is ParticleSystem.Particle)
            //    {
            //        ParticleSystem.Particle p = (ParticleSystem.Particle)go;
            //        p.alpha = p.lifetime / p.totaltime;
            //    }
            //}
        }
    }
    public static class ZOrder
    {
        private static Dictionary<Pivot, float> zValues = new Dictionary<Pivot, float>();
        public static Dictionary<GameObject, Pivot> zLayer = new Dictionary<GameObject, Pivot>();
        public static GameObject camera;
        public static float z0;
        public static void SetCamera(GameObject cam)
        {
            camera = cam;
        }
        public static void Add(GameObject obj, float z)
        {
            Pivot zlayer = new Pivot();

            zValues.Add(zlayer, z);
            zLayer.Add(obj, zlayer);
            zlayer.AddChild(obj);
            MyGame.self.AddChild(zlayer);
        }

        public static void Remove(GameObject obj)
        {
            Pivot zlayer = zLayer[obj];
            zLayer.Remove(obj);
            MyGame.self.RemoveChild(zlayer);
            zValues.Remove(zlayer);
            zlayer.LateDestroy();
        }

        public static void ApplyParallax()
        {
            foreach (Pivot pivot in zValues.Keys)
            {

                pivot.x = - z0 / (zValues[pivot] + z0) * camera.x;
                pivot.y = - z0 / (zValues[pivot] + z0) * camera.y;
                pivot.scale = 1/(1 + zValues[pivot]/z0);
            }
        }

        public static float GetZ(GameObject go)
        {
            return zValues[zLayer[go]];
        }
        public static void SetZ(GameObject go, float value)
        {
            zValues[zLayer[go]] = value;
        }
    }
}