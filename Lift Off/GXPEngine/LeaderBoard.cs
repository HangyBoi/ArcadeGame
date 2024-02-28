using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static EasyDraw display = new EasyDraw(600, 600);

        public static void LoadScores(string path)
        {
            StreamReader reader = File.OpenText(path);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Score score = new Score();
                string[] words = line.Split(' ');
                if (words.Length == 2)
                {
                    score.name = words[0];
                    int.TryParse(words[1], out score.score);
                    scores.Add(score);
                }
            }
        }

        public static void SaveScores(string path)
        {
            StreamWriter writer = new StreamWriter(path);
            foreach (Score score in scores)
            {
                writer.WriteLine(score.name + " " + score.score);
            }
        }
        public static void UpdateDisplay()
        {
            display.ClearTransparent();
        }
    }
}
