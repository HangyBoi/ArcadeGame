using GXPEngine;
using GXPEngine.Animation;
using System;
using System.Drawing;
using GXPEngine.Animation;

public class HUD : EasyDraw
{
    public static HUD self;
    public Player player;
    private int baseTextSize;

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

    public EasyDraw HPDisplay;
    public EasyDraw ScoreDisplay;
    public EasyDraw ComboDisplay;

    private Timer scoreAnimationTimer;
    private Timer comboAnimationTimer;
    private Timer hpAnimationTimer;

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
        self = this;
        player = MyGame.self.player;

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

        scoreAnimationTimer = new Timer();
        hpAnimationTimer = new Timer();
        comboAnimationTimer = new Timer();

        SetXY(-MyGame.self.width / 2, -MyGame.self.height / 2);
    }

    public void HudUpdate(Player _player)
    {
        graphics.Clear(Color.Empty);
        string hpText = "HP: " + _player.GetHP();
        Font customFont = new Font(SystemFonts.DefaultFont.FontFamily, (int)(Mathf.Lerp(baseTextSize, targetTextSize, lerpTextTimer)));
        graphics.DrawString(hpText, customFont, Brushes.White, 0, 0);
    {
        AddChild(HPDisplay);
        AddChild(ScoreDisplay);
        AddChild(ComboDisplay);

        SetXY(-MyGame.self.width / 2, -MyGame.self.height/ 2);
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
        HPDisplay = new EasyDraw(500,200);
        HPDisplay.TextSize(40);
        HPDisplay.SetXY(0, 0);
        HPDisplay.TextAlign(CenterMode.Center, CenterMode.Center);

        HPDisplay.ClearTransparent();
        string hpText = "HP: " + player.HP;
        HPDisplay.Text(hpText, 250, 100);

        ScoreDisplay = new EasyDraw(500,200);
        ScoreDisplay.SetOrigin(ScoreDisplay.width / 2, 0);
        ScoreDisplay.TextSize(40);
        ScoreDisplay.SetXY(MyGame.self.width / 2, 0);
        ScoreDisplay.TextAlign(CenterMode.Center, CenterMode.Center);

        ScoreDisplay.ClearTransparent();
        string scoreText = "" + player.score;
        ScoreDisplay.Text(scoreText, 250, 100);


        ComboDisplay = new EasyDraw(500, 200);
        ComboDisplay.SetOrigin(ScoreDisplay.width / 2, 0);
        ComboDisplay.TextSize(60);
        ComboDisplay.SetXY(MyGame.self.width / 2 + 300, 0);
        ComboDisplay.TextAlign(CenterMode.Center, CenterMode.Center);

        ComboDisplay.ClearTransparent();
        if (MyGame.self.comboMultiplier > 1)
        {
            string comboText = "x" + MyGame.self.comboMultiplier;
            ComboDisplay.Fill(MyGame.self.GetComboColor());
            ComboDisplay.Text(comboText, 250, 100);
        }
    {
        scoreAnimationTimer.SetLaunch(0.3f);
    }
    public void StartComboAnimation()
    {
        comboAnimationTimer.SetLaunch(0.3f);
        SetCombo();
    }
    public void HPAnimation()
    {
        HPDisplay.scale = 1 + hpAnimationTimer.time;
    }
    public void ScoreAnimation()
    {
        ScoreDisplay.scale = 1 + scoreAnimationTimer.time;
    }

    public void ComboAnimation()
    {
        ComboDisplay.scale = 1 + comboAnimationTimer.time;
        ComboDisplay.ClearTransparent();
        if (MyGame.self.comboMultiplier > 1)
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

    public void SetCombo()
    {
        ComboDisplay.ClearTransparent();
        if (MyGame.self.comboMultiplier > 1)
        {
            string comboText = "x" + MyGame.self.comboMultiplier;
            ComboDisplay.Text(comboText, 250, 100);
        }
    }
    public void SetScore(int score)
    {
        string scoreText = "" + score;
        ScoreDisplay.ClearTransparent();
        ScoreDisplay.Text(scoreText, 250, 100);
        StartScoreAnimation();

    }
    public void SetHp(int hp)
    {
        string hpText = "HP: " + hp;
        HPDisplay.ClearTransparent();
        HPDisplay.Text(hpText, 250, 100);
        StartHPAnimation();
    }
            HPAnimation();
        }
        if (scoreAnimationTimer.time > 0)
        {
            ScoreAnimation();
        }
        if (scoreAnimationTimer.time > 0)
        {
            ComboAnimation();
        }
    }
}
