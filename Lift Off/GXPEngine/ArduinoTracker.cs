using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

//wjeohgirehfjopw[egph

namespace GXPEngine
{
    public enum Method
    {
        GYRO,
        NOGYRO
    }
    public enum ButtonStatus
    {
        RELEASED = 0,
        ON_PRESS = 1,
        PRESSED = 3,
        ON_RELEASE = 2,
    }

    public static class ArduinoTracker
    {
        public static float accx, accy, accz;
        public static float gyrox, gyroy, gyroz;
        public static int[] D = new int[20];
        public static Method method;

        public static SerialPort port = new SerialPort();

        public static void ConnectPort()
        {
            port.PortName = "COM12";
            port.BaudRate = 9600;
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.Open();
        }
        //wfgjewhppjf[pkg]
        public static void ReadInput()
        {
            string line = port.ReadLine();

            string[] input_buffer = line.Split(' ');

            if (input_buffer[0] == "Gyro" || input_buffer[0] == "NoGyro")
            {
                if (input_buffer[0] == "Gyro")
                    method = Method.GYRO;
                if (input_buffer[0] == "NoGyro")
                    method = Method.NOGYRO;

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

                UpdateButtonStatus(4, input_buffer[7] == "1");

                UpdateButtonStatus(7, input_buffer[8] == "1");
            }

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                port.Write(key.KeyChar.ToString());  // writing a string to Arduino
            }
            if (D[7] == 3)
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
        public static void UpdateButtonStatus (int index, bool value)
        {
            D[index] <<= 1;
            D[index] %= 4;
            if (value)
                D[index] |= 1;

            //Console.WriteLine((ButtonStatus)D[index]);
        }
    }
    //wpfjgeo[qwpetjpwj[
}
