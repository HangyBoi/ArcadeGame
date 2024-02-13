using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public static class PositionParser
    {
        public static Vector3 acc;
        public static Vector3 rot;

        public static void GetData()
        {
            acc = new Vector3(ArduinoTracker.accx, ArduinoTracker.accy, ArduinoTracker.accz);
            rot = new Vector3(ArduinoTracker.gyrox, ArduinoTracker.gyroy, ArduinoTracker.gyroz);

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
    }
}
