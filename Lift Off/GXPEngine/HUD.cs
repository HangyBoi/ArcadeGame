using GXPEngine;
using GXPEngine.Animation;
using System;
using System.Drawing;
using GXPEngine.Animation;
using System.Runtime.CompilerServices;

public class HUD : EasyDraw
{
    public static HUD self;
    public Player player;
    private float offset;
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

    private int hp;
    private int newHP;

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

        float _scaleX = 0.5f;
        float _scaleY = 0.5f;
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
        scoreAnimationTimer = new Timer();
        hpAnimationTimer = new Timer();
        comboAnimationTimer = new Timer();

        hpAnimationTimer.OnTimerEnd += HPAnimationEnd;

        HPDisplay = new EasyDraw(500, 200);
        HPDisplay.TextFont(new Font(MyGame.fontFamily,40));
        HPDisplay.SetXY(0, 0);
        HPDisplay.TextAlign(CenterMode.Center, CenterMode.Center);
        hp = 3;
        newHP = 3;

        for (int i=0; i<player.HP; i++)
        {
            Sprite heart = new Sprite("Assets/heart.png");
            heart.scale = 0.5f;
            heart.SetXY(i * heart.width + heart.width / 2, heart.height / 2);
            heart.SetOrigin(heart.width/2, heart.height/2);
            HPDisplay.AddChild(heart);
        }


        HPDisplay.ClearTransparent();

        ScoreDisplay = new EasyDraw(500, 200);
        ScoreDisplay.TextFont(new Font(MyGame.fontFamily, 40));
        ScoreDisplay.SetOrigin(ScoreDisplay.width / 2, 0);
        ScoreDisplay.SetXY(MyGame.self.width / 2, 0);
        ScoreDisplay.TextAlign(CenterMode.Center, CenterMode.Center);
        ScoreDisplay.ClearTransparent();
        string scoreText = "" + player.score;
        ScoreDisplay.Text(scoreText, 250, 100);


        ComboDisplay = new EasyDraw(500, 200);
        ComboDisplay.TextFont(new Font(MyGame.fontFamily, 60));
        ComboDisplay.SetOrigin(ScoreDisplay.width / 2, 0);
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

    public void StartHPAnimation(int newHP)
    {
        hpAnimationTimer.SetLaunch(0.3f);
        this.newHP = newHP;

        if (newHP > hp) 
        {
            for (int i = hp; i < newHP; i++)
            {
                Sprite heart = new Sprite("Assets/heart.png");
                heart.scale = 0.5f;
                heart.SetXY(i * heart.width + heart.width / 2, heart.height / 2);
                heart.SetOrigin(heart.width / 2, heart.height / 2);
                HPDisplay.AddChild(heart);
            }
        }
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
        if (hp > newHP)
        {
            for (int i=0; i<hp-newHP; i++)
            {
                HPDisplay.GetChildren()[HPDisplay.GetChildCount() - i-1].scale = hpAnimationTimer.time*0.5f/0.3f;
            }
        }

        if (hp < newHP)
        {
            for (int i = 0; i < newHP - hp; i++)
            {
                HPDisplay.GetChildren()[HPDisplay.GetChildCount() - i - 1].scale = Mathf.Clamp((1 - hpAnimationTimer.time),0,1)/2;
            }
        }
    }

    public void HPAnimationEnd()
    {
        if (hp > newHP)
            for (int i = 0; i < hp - newHP; i++)
            {
                HPDisplay.RemoveChild(HPDisplay.GetChildren()[HPDisplay.GetChildCount() - 1]);
            }
        hp = newHP;
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
    public void SetScore(int score)
    {
        string scoreText = "" + score;
        ScoreDisplay.ClearTransparent();
        ScoreDisplay.Text(scoreText, 250, 100);
        StartScoreAnimation();

    }
    public void SetHp(int hp)
    {
        StartHPAnimation(hp);
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
