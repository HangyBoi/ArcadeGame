using GXPEngine.Core;
using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public static class directionMethods
    {
        public static direction Opposite(this direction dir)
        {
            return (direction)(((int)dir + 4) % 8);
        }
    }
    public static class PositionParser
    {
        public delegate void PlayerInput(direction dir);
        public static event PlayerInput OnPlayerInput;
        public static Vector2 velocity = new Vector2(0, 0);
        public static Vector2 acceleration = new Vector2(0, 0);
        public static Vector2 position = new Vector2(0, 0);

        public static float sensitivity = 0.005f;
        public static float angularVelocityDeviation = 0.57f / 1000;
        public static float angularDeviation = 0.1f;
        public static Vector3 acc;
        public static Vector3 playerAcc;
        public static Vector3 playerAccLocal;
        public static Vector2 screenPos;
        public static Vector3 rot;

        public static direction[] directionBuffer = new direction[5];

        public static direction DetectMovement()
        {
            float angle = position.angle(0);
            if (position.length() > 1/sensitivity)
            {
                int dir = (int)((angle + 9*Mathf.PI/8) / Mathf.PI * 4) % 8;
                //Console.WriteLine((direction)dir);
                return (direction)dir;
            }
            //Console.WriteLine(direction.NONE);
            return direction.NONE;
        }

        public static Vector2 UpdateCoordinates ()
        {
            if (ArduinoTracker.method == Method.GYRO)
                acceleration = new Vector2(playerAcc.x, playerAcc.z);
            if (ArduinoTracker.method == Method.NOGYRO)
                acceleration = new Vector2(playerAccLocal.x, playerAccLocal.z);

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
            if (rot.x < -90 || rot.x > 90)
                rot.z = 180;
            else
                rot.z = 0;
            playerAccLocal = acc - new Vector3(0, 0, 1).Rotate(-rot * Mathf.DegToRad);

            //Console.WriteLine(new Vector3(0, 0, 1).Rotate(-rot * Mathf.DegToRad));
            //Console.WriteLine(acc);
            //Console.WriteLine(playerAcc);
            //Console.WriteLine(playerAccLocal);
            //Console.WriteLine(rot);

            //Console.WriteLine(Mathf.Loop(0, 1000, Time.time));
        }
        public static void Calibrate()
        {
            position = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
        }
        public static void Update()
        {
        }
        public static void FilterMovement ()
        {
            direction newDir = DetectMovement();

            for (int i=1; i<directionBuffer.Length; i++)
                directionBuffer[i - 1] = directionBuffer[i];

            directionBuffer[4] = newDir;


            if (newDir == direction.NONE)
                return;

            int count = 0;
            for (int i = 0; i < directionBuffer.Length; i++)
                if (directionBuffer[i] == newDir)
                    count++;
            if (count == directionBuffer.Length)
                return;

            int empty = 0;
            int action = 0;

            for (int i=0; i<directionBuffer.Length-1; i++)
            {
                if (directionBuffer[i] == direction.NONE || directionBuffer[i] == directionBuffer[4].Opposite())
                    empty++;
                if (directionBuffer[i] == directionBuffer[4])
                    action++;
            }

            if (empty > 1)
                return;
            if (action == 3)
                OnPlayerInput?.Invoke(directionBuffer[4]);
        }
    }
}
