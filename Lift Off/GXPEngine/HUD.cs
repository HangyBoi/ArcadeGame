using GXPEngine;
using GXPEngine.Animation;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using TiledMapParser;

public class HUD : EasyDraw
{
    public static HUD self;
    public Player player;
    private int baseTextSize;

    public EasyDraw HPDisplay;
    public EasyDraw ScoreDisplay;
    public EasyDraw ComboDisplay;

    private Timer scoreAnimationTimer;
    private Timer comboAnimationTimer;
    private Timer hpAnimationTimer;

    public HUD() : base(MyGame.self.width, MyGame.self.height, false)
    {
        self = this;
        player = MyGame.self.player;
        baseTextSize = 20;

        scoreAnimationTimer = new Timer();
        hpAnimationTimer = new Timer();
        comboAnimationTimer = new Timer();

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

        AddChild(HPDisplay);
        AddChild(ScoreDisplay);
        AddChild(ComboDisplay);

        SetXY(-MyGame.self.width / 2, -MyGame.self.height/ 2);



    }
    public void StartHPAnimation()
    {
        hpAnimationTimer.SetLaunch(0.3f);
    }
    public void StartScoreAnimation()
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

    public void SetCombo()
    {
        ComboDisplay.ClearTransparent();
        if (MyGame.self.comboMultiplier > 1)
        {
            string comboText = "x" + MyGame.self.comboMultiplier;
            ComboDisplay.Fill(MyGame.self.GetComboColor());
            ComboDisplay.Text(comboText, 250, 100);
        }
    }
    public void HudUpdate()
    {
        if (hpAnimationTimer.time > 0)
        {
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
