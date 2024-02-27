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

    private Timer scoreAnimationTimer;
    private Timer hpAnimationTimer;

    public HUD() : base(MyGame.self.width, MyGame.self.height, false)
    {
        self = this;
        player = MyGame.self.player;
        baseTextSize = 20;

        scoreAnimationTimer = new Timer();
        hpAnimationTimer = new Timer();

        HPDisplay = new EasyDraw(500,200);
        HPDisplay.TextSize(60);

        ScoreDisplay = new EasyDraw(500,200);
        ScoreDisplay.SetOrigin(ScoreDisplay.width / 2, 0);
        ScoreDisplay.TextSize(60);

        AddChild(HPDisplay);
        AddChild(ScoreDisplay);

        SetXY(-MyGame.self.width / 2, -MyGame.self.height/ 2);
        HPDisplay.SetXY(0, 0);
        ScoreDisplay.SetXY(MyGame.self.width / 2, 0);
        HPDisplay.TextAlign(CenterMode.Center,CenterMode.Center);
        ScoreDisplay.TextAlign(CenterMode.Center, CenterMode.Center);

        string scoreText = "" + player.score;
        ScoreDisplay.ClearTransparent();
        ScoreDisplay.Text(scoreText, 250, 100);

        string hpText = "HP: " + player.HP;
        HPDisplay.ClearTransparent();
        HPDisplay.Text(hpText, 250, 100);

    }
    public void StartHPAnimation()
    {
        hpAnimationTimer.SetLaunch(0.3f);
    }
    public void StartScoreAnimation()
    {
        scoreAnimationTimer.SetLaunch(0.3f);
    }
    public void HPAnimation()
    {
        HPDisplay.scale = 1 + hpAnimationTimer.time;
    }
    public void ScoreAnimation()
    {
        ScoreDisplay.scale = 1 + scoreAnimationTimer.time;
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
    }
}
