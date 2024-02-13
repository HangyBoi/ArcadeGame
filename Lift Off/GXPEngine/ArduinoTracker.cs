using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace GXPEngine
{
    public static class ArduinoTracker
    {
        public static float accx, accy, accz;
        public static float gyrox, gyroy, gyroz;
        public static bool D4;

        static public SerialPort port = new SerialPort();
        public static void ConnectPort()
        {
            port.PortName = "COM7";
            port.BaudRate = 115200;
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.Open();
        }
        public static void ReadInput()
        {
            string line = port.ReadLine();
            if (line != "start\r")
            {
                return;
            }
            gyrox = ReadFloat();
            gyroy = ReadFloat();
            gyroz = ReadFloat();

            accx = ReadFloat();
            accy = ReadFloat();
            accz = ReadFloat();

            D4 = ReadBool();
                
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                port.Write(key.KeyChar.ToString());  // writing a string to Arduino
            }
        }

        public static float ReadFloat ()
        {
            string line = port.ReadLine();
            float res;
            if (line != "" && float.TryParse(line, out res))
                return res;
            else
                return 0;
        }
        public static bool ReadBool()
        {
            string line = port.ReadLine();
            if (line == "1\r")
                return true;
            return false;
        }
    }
}
