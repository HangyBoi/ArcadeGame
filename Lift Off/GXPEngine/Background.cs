using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GXPEngine
{
    public static class ZOrder
    {
        public static Dictionary<Pivot, float> zValues = new Dictionary<Pivot, float>();
        public static Dictionary<GameObject, Pivot> zLayer = new Dictionary<GameObject, Pivot>();
        public static Camera camera;
        public static float z0;
        public static void SetCamera(Camera cam)
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