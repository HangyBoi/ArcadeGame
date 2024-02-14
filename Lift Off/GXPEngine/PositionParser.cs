using GXPEngine.Core;
using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public static class PositionParser
    {
        public static float angularVelocityDeviation = 0.54f/1000;
        public static float angularDeviation = 0.1f;
        public static Vector3 acc;
        public static Vector3 rot;

        public static void GetData()
        {
            acc = new Vector3(ArduinoTracker.accx, ArduinoTracker.accy, ArduinoTracker.accz);
            rot = new Vector3(ArduinoTracker.gyrox, ArduinoTracker.gyroy, ArduinoTracker.gyroz);

            rot.x += angularDeviation;

            rot.x = Mathf.Loop(-180, 180, rot.x);
            rot.y = Mathf.Loop(-180, 180, rot.y);
            rot.z = Mathf.Loop(-180, 180, rot.z);

            //Console.WriteLine(acc);
            Console.WriteLine(rot);
        }
        public static Vector3 CalculateGravity()
        {
            Vector3 gravity;
            gravity = new Vector3(0,0,1).Rotate(rot * Mathf.DegToRad);

            //Console.WriteLine(gravity.ToString());
            return gravity;

        }
        public static void Update()
        {
        }
    }
}
