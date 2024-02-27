using GXPEngine;
using System;
using System.Drawing;

public class HUD : EasyDraw
{
    private float offset;
    private int baseTextSize;
    private int targetTextSize;
    private float lerpTextSpeed;
    private float lerpTextTimer;
    private bool isLerpingForward;
    private bool isLerpingBackward;

    public HUD() : base(MyGame.self.width, MyGame.self.height, false)
    {
        offset = 35;
        baseTextSize = 20;
        targetTextSize = baseTextSize; 

        lerpTextSpeed = 0.05f;
        lerpTextTimer = 0f;
        isLerpingForward = false;
        isLerpingBackward = false;

        SetXY(-MyGame.self.width / 2, -MyGame.self.height / 2);
    }

    public void HudUpdate(Player _player)
    {
        graphics.Clear(Color.Empty);
        string hpText = "HP: " + _player.GetHP();
        Font customFont = new Font(SystemFonts.DefaultFont.FontFamily, (int)(Mathf.Lerp(baseTextSize, targetTextSize, lerpTextTimer)));
        graphics.DrawString(hpText, customFont, Brushes.White, 0, 0);

        string scoreText = "" + _player.GetScore();
        graphics.DrawString(scoreText, customFont, Brushes.White, MyGame.self.width / 2, 0);

        if (_player.hpDecreased == true)
        {
            if (!isLerpingForward && !isLerpingBackward)
            {
                isLerpingForward = true;
            }
        }

        if (isLerpingForward)
        {
            lerpTextTimer += lerpTextSpeed;
            if (lerpTextTimer >= 1f)
            {
                lerpTextTimer = 0f;
                isLerpingForward = false;
                targetTextSize = 60;
                isLerpingBackward = true;
            } 
        }

        if (isLerpingBackward)
        {
            lerpTextTimer += lerpTextSpeed;
            if (lerpTextTimer >= 1f)
            {
                lerpTextTimer = 0f;
                isLerpingBackward = false;
                targetTextSize = (int)Mathf.Lerp(targetTextSize, baseTextSize, lerpTextTimer); // Use Mathf.Lerp here
            }
        }

        /*        if (_player._HP == 0)
                {
                    MyGame.self.Destroy();
                }*/
    }
}
