using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public static class PopupSprites
    {
        public static Sprite kill;
        public static Sprite doubleHit;
        public static Sprite plus50;

        public static void Setup()
        {
            kill = CreateLabel("KILL");
            doubleHit = CreateLabel("DOUBLE!");
            plus50 = CreateLabel("+50");
        }
        public static Sprite CreateLabel (string text)
        {
            EasyDraw temp = new EasyDraw(200, 100);
            temp.TextSize(20);
            temp.TextAlign(CenterMode.Center, CenterMode.Center);
            temp.SetOrigin(temp.width / 2, temp.height / 2);
            temp.Text(text, temp.width/2, temp.height/2);
            return temp;

        }
    }
}
