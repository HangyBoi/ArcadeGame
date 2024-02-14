using GXPEngine.Core;
using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GXPEngine
{
    public enum direction
    {
        NONE = -1,
        LEFT = 0, 
        UP_LEFT = 1, 
        UP = 2, 
        UP_RIGHT = 3, 
        RIGHT = 4, 
        DOWN_RIGHT = 5, 
        DOWN = 6, 
        DOWN_LEFT = 7
    }
    public static class PositionParser
    {
        public static Vector2 velocity = new Vector2(0, 0);
        public static Vector2 acceleration = new Vector2(0, 0);
        public static Vector2 position = new Vector2(0, 0);

        public static float sensitivity = 0.005f;
        public static float angularVelocityDeviation = 0.57f / 1000;
        public static float angularDeviation = 0.1f;
        public static Vector3 acc;
        public static Vector3 playerAcc;
        public static Vector2 screenPos;
        public static Vector3 rot;

        public static direction DetectMovement()
        {
            float angle = position.angle(0);
            if (position.length() > 1/sensitivity)
            {
                int dir = (int)((angle + 9*Mathf.PI/8) / Mathf.PI * 4) % 8;
                Console.WriteLine((direction)dir);
                return (direction)dir;
            }
            return direction.NONE;
        }

        public static Vector2 UpdateCoordinates ()
        {
            acceleration = new Vector2(playerAcc.x, playerAcc.z);
            velocity += new Vector2(acceleration.x, acceleration.y) * Time.deltaTime * 100;
            velocity = velocity.Lerp(new Vector2(0,0), 0.005f * Time.deltaTime);

            //cam.x = Mathf.Lerp(cam.x, cameraTarget.x, followSpeed);
            //cam.y = Mathf.Lerp(cam.y, cameraTarget.y, followSpeed);

            //coin.x = -ArduinoTracker.gyrox*10;
            //coin.y = -ArduinoTracker.gyroz*15;

            position.x = -velocity.x / 50;
            position.y = -velocity.y / 50;

            //position.x -= velocity.x * Time.deltaTime / 1000;
            //position.y -= velocity.y * Time.deltaTime / 1000;

            return position;
        }
        public static void GetData()
        {
            acc = new Vector3(ArduinoTracker.accx, ArduinoTracker.accy, ArduinoTracker.accz);
            rot = new Vector3(ArduinoTracker.gyroz, ArduinoTracker.gyroy, ArduinoTracker.gyrox);

            rot.z += angularDeviation;

            rot.x = Mathf.Loop(-180, 180, rot.x);
            rot.y = Mathf.Loop(-180, 180, rot.y);
            rot.z = Mathf.Loop(-180, 180, rot.z);

            playerAcc = (acc).Rotate(rot * Mathf.DegToRad) - new Vector3(0, 0, 1);

            //Console.WriteLine(acc);
            //Console.WriteLine(playerAcc);
            //Console.WriteLine(rot);
        }
        public static void Calibrate()
        {
            position = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
        }
        public static void Update()
        {
        }
    }
}
