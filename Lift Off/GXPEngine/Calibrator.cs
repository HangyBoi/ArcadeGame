using GXPEngine.Core;
using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GXPEngine
{
    /*
     * 
     * THIS ClASS IS NOT IMPLEMENTED
     * 
     * 
     */
    
    
    public static class Calibrator
    {
        public static Vector3 gravity;
        public static Vector3 downMask;
        public static Vector3 rightMask;
        public static EasyDraw screen;

        public delegate void OnCalibration();
        public static event OnCalibration StartCalibration;
        public static event OnCalibration EndCalibration;

        public static event OnCalibration StartTracking;
        public static event OnCalibration EndTracking;

        public enum Status
        {
            DISABLED,
            STATIC,
            VERTICAL,
            VERTICAL_TOO_SLOW,
            HORISONTAL,
            HORISONTAL_TOO_SLOW
        }
        private static string message;
        private static Status status;
        private static Vector3 maxRecord;
        private static bool recorded;
        //private static List<Vector3> buffer;
        public static void Setup()
        {
            screen = new EasyDraw(MyGame.self.width, MyGame.self.height, addCollider:false);
            screen.x = -MyGame.self.width / 2;
            screen.y = -MyGame.self.height / 2;
            screen.ClearTransparent();
            screen.ShapeAlign(CenterMode.Center, CenterMode.Center);
            screen.TextAlign(CenterMode.Center, CenterMode.Center);

            status = Status.DISABLED;
        }
        public static void BeginCalibration()
        {
            StartCalibration?.Invoke();
            status = Status.STATIC;
            downMask = new Vector3(0,0,0);
            rightMask = new Vector3(0, 0, 0);
            MyGame.self.AddChild(screen);
        }
        public static void Update()
        {
            //Console.WriteLine(status);
            if (status == Status.DISABLED)
                return;
            switch (status)
            {
                case Status.STATIC:
                    message = "Hold the controller still and press the UPPER button";
                    break;
                case Status.VERTICAL:
                    message = "Draw a vertical line from UP to DOWN while pressing the UPPER button";
                    break;
                case Status.HORISONTAL:
                    message = "Draw a horizontal line from LEFT to RIGHT pressing the UPPER button";
                    break;
                case Status.VERTICAL_TOO_SLOW:
                case Status.HORISONTAL_TOO_SLOW:
                    message = "Draw again faster";
                    break;
            }
            Draw();
            switch (status)
            {
                case Status.STATIC:
                    if (ArduinoTracker.D[4] == 3)
                        gravity = PositionParser.acc;
                    if (ArduinoTracker.D[4] == 2)
                        status = Status.VERTICAL;
                    break;

                case Status.VERTICAL_TOO_SLOW:
                case Status.VERTICAL:
                    if (ArduinoTracker.D[4] == 1)
                    {
                        maxRecord = new Vector3(0, 0, 2);
                    }
                    if (ArduinoTracker.D[4] == 3)
                    {
                        if (PositionParser.playerAccLocal.length() > maxRecord.length())
                        {
                            recorded = true;
                            maxRecord = PositionParser.playerAccLocal;
                        }
                    }

                    if (ArduinoTracker.D[4] == 2)
                    {
                        if (!recorded)
                            status = Status.VERTICAL_TOO_SLOW;
                        else
                        {
                            downMask = maxRecord;
                            status = Status.HORISONTAL;
                        }
                    }
                    break;
                case Status.HORISONTAL_TOO_SLOW:
                case Status.HORISONTAL:
                    if (ArduinoTracker.D[4] == 1)
                    {
                        maxRecord = new Vector3(0, 0, 2);
                    }
                    if (ArduinoTracker.D[4] == 3)
                    {
                        if (PositionParser.playerAccLocal.length() > maxRecord.length())
                        {
                            recorded = true;
                            maxRecord = PositionParser.playerAccLocal;
                        }
                    }

                    if (ArduinoTracker.D[4] == 2)
                    {
                        if (!recorded)
                            status = Status.HORISONTAL_TOO_SLOW;
                        else
                        {
                            rightMask = maxRecord;
                            status = Status.DISABLED;

                            MyGame.self.RemoveChild(screen);
                            EndCalibration?.Invoke();
                        }
                    }
                    break;
            }
        }
        public static bool CheckRecording()
        {
            if (maxRecord.length() < 1)
                return false;
            return true;
        }
        public static void Draw()
        {
            screen.Clear(0);
            screen.Fill(255);
            screen.Text(message, screen.width/2, screen.height/2);
        }
    }
}
