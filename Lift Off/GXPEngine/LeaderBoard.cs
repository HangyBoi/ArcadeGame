using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GXPEngine.LeaderBoard;

namespace GXPEngine
{
    public static class LeaderBoard
    {
        public struct Score
        {
            public string name;
            public int score;
        }
        public static List<Score> scores = new List<Score>();
        private static int[,] symbols = new int[,]

        {
            {65,66,67,68,69,70 },
            {71,72,73,74,75,76 },
            {77,78,79,80,81,82 },
            {83,84,85,86,87,88 },
            {89,90,32,46,0x232B,0x2386}
        };
        public static EasyDraw display;
        private static int currentScore;
        public static char symbol;
        public static int X = 0, Y = 0;

        private static EasyDraw[,] keyboard;
        public static GameObject Keyboard;

        public static void SetupKeyboard()
        {
            symbol = (char)65;
            Keyboard = new GameObject();
            Keyboard.SetXY(200, -200);
            MyGame.self.cam.AddChild(Keyboard);
            keyboard = new EasyDraw[6, 5];
            for (int x=0; x<6; x++)
            {
                for (int y=0; y<5; y++)
                {
                    keyboard[x, y] = new EasyDraw(90, 90);
                    keyboard[x, y].TextFont(new Font(MyGame.fontFamily, 40));
                    keyboard[x, y].SetXY(x * 100, y * 100);
                    keyboard[x, y].SetOrigin(keyboard[x, y].width/2, keyboard[x, y].height / 2);
                    keyboard[x, y].TextAlign(CenterMode.Center, CenterMode.Center);
                    LightDownKey(x, y);
                    Keyboard.AddChild(keyboard[x, y]);

                }
            }
            LightUpKey(0, 0);
        }
        public static void Enable()
        {
            MyGame.self.cam.AddChild(Keyboard);
            MyGame.self.cam.AddChild(display);

        }

        public static void Disable()
        {
            MyGame.self.cam.RemoveChild(Keyboard);
            MyGame.self.cam.RemoveChild(display);

        }
        public static void LightDownKey(int x, int y)
        {
            keyboard[x, y].Clear(50, 50, 50, 50);
            keyboard[x, y].Text("" + (char)(symbols[y,x]), keyboard[x, y].width / 2, keyboard[x, y].height / 2);
        }
        public static void LightUpKey(int x, int y)
        {
            keyboard[x, y].Clear(200, 200, 50, 50);
            keyboard[x, y].Text("" + (char)(symbols[y, x]), keyboard[x, y].width / 2, keyboard[x, y].height / 2);
        }
        public static void LoadScores(string path)
        {
            StreamReader reader = File.OpenText(path);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Score score = new Score();
                string[] words = line.Split('\t');
                if (words.Length == 2)
                {
                    score.name = words[0];
                    int.TryParse(words[1], out score.score);
                    int pos = scores.Count;
                    for (int i= scores.Count -1 ; i >= 0; i--)
                    {
                        if (scores[i].score < score.score)
                            pos--;
                        else
                            break;
                    }
                    scores.Insert(pos, score);

                }
            }
            //Console.WriteLine(scores[0].name + " " + scores[0].score);
        }

        public static void SaveScores(string path)
        {
            StreamWriter writer = new StreamWriter(path);
            foreach (Score score in scores)
            {
                Console.WriteLine("savinng..");
                writer.WriteLine(score.name + '\t' + score.score);
            }
            writer.Close();
        }

        public static void UpdateDisplay()
        {
            display.Clear(0,0,0,50);
            display.TextFont(new Font(MyGame.fontFamily, 15));
            display.TextAlign(CenterMode.Center, CenterMode.Min);
            for (int i = 0; i < scores.Count; i++)
            {
                if (i == currentScore)
                {
                    if (StateOfTheGame.currentState == StateOfTheGame.GameState.Typing)
                        display.Fill(Color.Yellow);
                    if (StateOfTheGame.currentState == StateOfTheGame.GameState.GameOver)
                        display.Fill(Color.LightGreen);
                }
                else
                    display.Fill(Color.White);
                display.Text(scores[i].name, display.width / 4, i * 30 + 50);
                display.Text(scores[i].score.ToString(), 3 * display.width / 4, i * 30 + 50);
            }
        }

        public static void AddScore(string name, int sc)
        {
            StateOfTheGame.SetGameState(StateOfTheGame.GameState.Typing);

            Score score = new Score();
            score.name = name;
            score.score = sc;
            int pos = scores.Count;
            for (int i = scores.Count - 1; i >= 0; i--)
            {
                if (scores[i].score < score.score)
                    pos--;
                else
                    break;
            }
            scores.Insert(pos, score);
            currentScore = pos;
            UpdateDisplay();
        }

        public static void EnterYourName()
        {

            for (int i = 65; i <= 90; i++)
                if (Input.GetKeyDown(i))
                {
                    EnterSymbol((char)i);
                }
            if (ArduinoTracker.D[4] == 1)
            {
                EnterSymbol();
            }
            if (Input.GetKeyDown(Key.ENTER))
            {
                ConfirmName();
            }
            if (Input.GetKeyDown(Key.BACKSPACE))
            {
                RemoveSymbol();
            }
        }
        private static void EnterSymbol(char ch)
        {
            Score temp = scores[currentScore];
            temp.name += ch;
            scores[currentScore] = temp;
            UpdateDisplay();
        }

        public static void EnterSymbol()
        {
            if (X == 5 && Y == 4)
            {
                ConfirmName();
                return;
            }

            if (X == 4 && Y == 4)
            {
                RemoveSymbol();
                return;
            }
            Score temp = scores[currentScore];
            temp.name += symbol;
            scores[currentScore] = temp;
            UpdateDisplay();
        }
        public static void ConfirmName()
        {
            Score temp = scores[currentScore];
            if (temp.name == "")
                scores.RemoveAt(currentScore);

            StateOfTheGame.SetGameState(StateOfTheGame.GameState.GameOver);
            SaveScores("score.txt");
            UpdateDisplay();
        }

        public static void RemoveSymbol()
        {
            Score temp = scores[currentScore];
            if (temp.name.Length > 0)
            {
                temp.name = temp.name.Remove(temp.name.Length - 1);
                scores[currentScore] = temp;
                UpdateDisplay();
            }
        }
        public static void ChooseLetter( Direction dir )
        {
            LightDownKey(X, Y);
            switch ( dir )
            {
                case Direction.LEFT:
                case Direction.UP_LEFT:
                case Direction.DOWN_LEFT:
                    if (X>0)
                        X--;
                    break;
                case Direction.RIGHT:
                case Direction.UP_RIGHT:
                case Direction.DOWN_RIGHT:
                    if (X < 5)
                        X++;
                    break;
            }
            switch (dir )
            {
                case Direction.DOWN:
                case Direction.DOWN_RIGHT:
                case Direction.DOWN_LEFT:
                    if (Y < 4)
                        Y++;
                    break;
                case Direction.UP:
                case Direction.UP_RIGHT:
                case Direction.UP_LEFT:
                    if (Y > 0)
                        Y--;
                    break;
            }
            symbol = (char)symbols[Y, X];

            LightUpKey(X, Y);
        }
        public static void Update()
        {
            if (StateOfTheGame.currentState == StateOfTheGame.GameState.Game)
            {
                display.x = Mathf.Lerp(display.x, 0, 10 * Time.deltaTime / 1000f);
                display.y = Mathf.Lerp(display.y, 1000, 10 * Time.deltaTime / 1000f);
                display.alpha = Mathf.Lerp(display.alpha, 0, 5f * Time.deltaTime / 1000f);

                Keyboard.x = Mathf.Lerp(Keyboard.x, 0, 10 * Time.deltaTime / 1000f);
                Keyboard.y = Mathf.Lerp(Keyboard.y, 1000, 10 * Time.deltaTime / 1000f);
                foreach (EasyDraw ed in keyboard)
                {
                    ed.alpha = Mathf.Lerp(ed.alpha, 0, 5f * Time.deltaTime / 1000f);
                }
            }

            if (StateOfTheGame.currentState == StateOfTheGame.GameState.Typing)
            {
                display.x = Mathf.Lerp(display.x, -500, 10 * Time.deltaTime / 1000f);
                display.y = Mathf.Lerp(display.y, 0, 10 * Time.deltaTime / 1000f);
                display.alpha = Mathf.Lerp(display.alpha, 1, 5f * Time.deltaTime / 1000f);

                Keyboard.x = Mathf.Lerp(Keyboard.x, 200, 10 * Time.deltaTime / 1000f);
                Keyboard.y = Mathf.Lerp(Keyboard.y, -200, 10 * Time.deltaTime / 1000f);

                foreach (EasyDraw ed in keyboard)
                {
                    ed.alpha = Mathf.Lerp(ed.alpha, 1, 5f * Time.deltaTime / 1000f);
                }
            }

            if (StateOfTheGame.currentState == StateOfTheGame.GameState.GameOver)
            {
                display.x = Mathf.Lerp(display.x, 0, 10 * Time.deltaTime / 1000f);
                display.y = Mathf.Lerp(display.y, 0, 10 * Time.deltaTime / 1000f);
                display.alpha = Mathf.Lerp(display.alpha, 1, 5f * Time.deltaTime / 1000f);

                Keyboard.x = Mathf.Lerp(Keyboard.x, 1000, 10 * Time.deltaTime / 1000f);
                Keyboard.y = Mathf.Lerp(Keyboard.y, -200, 10 * Time.deltaTime / 1000f);

                foreach (EasyDraw ed in keyboard)
                {
                    ed.alpha = Mathf.Lerp(ed.alpha, 0, 5f * Time.deltaTime / 1000f);
                }
            }
        }
    }
}
