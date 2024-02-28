using GXPEngine;
using System;
using System.Drawing;
using GXPEngine.Animation;

public class HUD : EasyDraw
{
    private float offset;
    private int baseTextSize;
    private int targetTextSize;
    private float lerpTextSpeed;
    private float lerpTextTimer;
    private bool isLerpingForward;
    private bool isLerpingBackward;

    private Sprite bombAbilityIcon;
    private Sprite greyBombAbilityIcon;
    private string bombCooldownText;
    public Timer bombCooldownTimer;
    public bool bombCooldownEnabled;

    private Sprite lightAbilityIcon;
    private Sprite greyLightbAbilityIcon;
    private string lightCooldownText;
    public Timer lightCooldownTimer;
    public bool lightCooldownEnabled;

    public HUD() : base(MyGame.self.width, MyGame.self.height, false)
    {

        bombAbilityIcon = new Sprite("Assets/abilities/bomb.png");
        greyBombAbilityIcon = new Sprite("Assets/abilities/greyBomb.png");
        bombCooldownText = "";

        lightAbilityIcon = new Sprite("Assets/abilities/lightning.png");
        greyLightbAbilityIcon = new Sprite("Assets/abilities/greyLigthning.png");
        lightCooldownText = "";

        float _scaleX = 0.25f;
        float _scaleY = 0.25f;
        offset = 35;

        bombAbilityIcon.SetScaleXY(_scaleX, _scaleY);
        bombAbilityIcon.SetXY(MyGame.self.width - bombAbilityIcon.width - offset, offset);
        greyBombAbilityIcon.SetScaleXY(_scaleX, _scaleY);
        greyBombAbilityIcon.SetXY(MyGame.self.width - bombAbilityIcon.width - offset, offset);

        lightAbilityIcon.SetScaleXY(_scaleX, _scaleY);
        lightAbilityIcon.SetXY(MyGame.self.width - bombAbilityIcon.width - offset, offset * 6);
        greyLightbAbilityIcon.SetScaleXY(_scaleX, _scaleY);
        greyLightbAbilityIcon.SetXY(MyGame.self.width - bombAbilityIcon.width - offset, offset * 6);

        bombCooldownTimer = new Timer();
        lightCooldownTimer = new Timer();

        //SCORE
        baseTextSize = 20;
        targetTextSize = baseTextSize;

        lerpTextSpeed = 0.05f;
        lerpTextTimer = 0f;
        isLerpingForward = false;
        isLerpingBackward = false;

        SetXY(-MyGame.self.width / 2, -MyGame.self.height / 2);

        bombCooldownTimer.OnTimerEnd += DisableBombCooldown;
        AddChild(bombAbilityIcon);
        lightCooldownTimer.OnTimerEnd += DisableLightCooldown;
        AddChild(lightAbilityIcon);
    }
    public void EnableBombCooldown()
    {
        RemoveChild(bombAbilityIcon);
        AddChild(greyBombAbilityIcon);

        bombCooldownTimer.SetLaunch(10f);
    }

    public void EnableLightCooldown()
    {
        RemoveChild(lightAbilityIcon);
        AddChild(greyLightbAbilityIcon);

        lightCooldownTimer.SetLaunch(10f);
    }

    public void DisableLightCooldown()
    {
        RemoveChild(greyLightbAbilityIcon);
        AddChild(lightAbilityIcon);
    }

    public void DisableBombCooldown()
    {
        RemoveChild(greyBombAbilityIcon);
        AddChild(bombAbilityIcon);
    }

    public void HudUpdate(Player _player)
    {
        Console.WriteLine(bombCooldownEnabled);

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
