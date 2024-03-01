using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public static class PopupSprites
    {

        public static void Setup()
        {
        }
        public static Sprite CreateLabel (string text)
        {
            EasyDraw temp = new EasyDraw(200, 100);
            temp.TextFont(new Font(MyGame.fontFamily, 20));
            temp.TextAlign(CenterMode.Center, CenterMode.Center);
            temp.SetOrigin(temp.width / 2, temp.height / 2);
            temp.Text(text, temp.width/2, temp.height/2);
            return temp;

        }
    }
}
