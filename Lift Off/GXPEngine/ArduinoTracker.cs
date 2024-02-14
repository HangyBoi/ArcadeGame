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
        public static bool[] D = new bool[20];

        public static SerialPort port = new SerialPort();
        public static void ConnectPort()
        {
            port.PortName = "COM7";
            port.BaudRate = 9600;
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.Open();
        }
        public static void ReadInput()
        {
            string line = port.ReadLine();

            string[] input_buffer = line.Split(' ');

            if (input_buffer[0] == "start")
            {
                if (!float.TryParse(input_buffer[1], out gyrox))
                    Console.WriteLine("Couldn't read gyroscope data");
                if (!float.TryParse(input_buffer[2], out gyroy))
                    Console.WriteLine("Couldn't read gyroscope data");
                if (!float.TryParse(input_buffer[3], out gyroz))
                    Console.WriteLine("Couldn't read gyroscope data");

                if (!float.TryParse(input_buffer[4], out accx))
                    Console.WriteLine("Couldn't read accelleration data");
                if (!float.TryParse(input_buffer[5], out accy))
                    Console.WriteLine("Couldn't read accelleration data");
                if (!float.TryParse(input_buffer[6], out accz))
                    Console.WriteLine("Couldn't read accelleration data");

                if (input_buffer[7] == "1")
                    D[4] = true;
                else
                    D[4] = false;


                if (input_buffer[8] == "1")
                    D[7] = true;
                else
                    D[7] = false;
            }

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                port.Write(key.KeyChar.ToString());  // writing a string to Arduino
            }
            if (D[7])
                PositionParser.angularDeviation = 0;
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
