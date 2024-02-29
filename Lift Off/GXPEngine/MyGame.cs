﻿using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using GXPEngine.OpenGL;
using GXPEngine.Core;
using GXPEngine.Animation;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MyGame : Game
{
    private bool arduinoEnabled = false;
    public static MyGame self;
    public Menu _menu;
    private bool _gameStarted;
    private static int resolutionX = 1920;
    private static int resolutionY = 1080;

    public ParticleSystem ps;
    public AnimationSprite coin;

    public Camera cam;
    public Pivot cameraOrigin;

    public EasyDraw canvas;

    public Sprite layer1;
    public Sprite line1, line2, line3;
    public Sprite background;
    public Sprite HUD;

    public Vector2 cameraTarget = new Vector2(0, 0);
    public Vector2 coinPrevPos;
    public float followSpeed = 5f;
    public float difficulty = 5;
    public float crossLine = -resolutionX / 2 + 250;
    public Pivot[] lineLayers;

    public ParticleSystem.RadialForce playerForce = new ParticleSystem.RadialForce(new Vector2(0, 0));
    public Timer comboTimer;
    public int combo = 0;
    public float comboMultiplier = 1;
    public Dictionary<int, float> comboList = new Dictionary<int, float>()
    {
        { 0, 1f },
        { 5, 1.2f },
        { 10, 1.5f },
        { 20, 2f },
        { 50, 5f }
    };
    public Dictionary<int, Color> comboColor = new Dictionary<int, Color>()
    {
        { 0, Color.White },
        { 5, Color.FromArgb(0xffff99) },
        { 10, Color.FromArgb(0xff9933) },
        { 20, Color.FromArgb(0xff3333) },
        { 50, Color.Magenta }
    };

    public Player player;

    protected HUD hud;
    protected SoundManager soundManager;


    public MyGame() : base(resolutionX, resolutionY, false, pRealWidth: 1366, pRealHeight: 768, pPixelArt: false)     // Create a window that's 800x600 and NOT fullscreen
    {
        if (arduinoEnabled)
            ArduinoTracker.ConnectPort();

        self = this;
        cam = new Camera(0, 0, resolutionX, resolutionY);
        cameraOrigin = new Pivot();
        cameraOrigin.SetXY(0, 0);
        coin = new AnimationSprite("Assets/Coins/coin.png", 5, 1);
        canvas = new EasyDraw(new Bitmap(resolutionX, resolutionY), false);
        canvas.SetXY(-resolutionX / 2, -resolutionY / 2);

        layer1 = new Sprite("Assets/backgrounds/layer1.png");
        layer1.SetOrigin(layer1.width / 2, layer1.height / 2);
        layer1.SetScaleXY(1 / 0.15f);

        line1 = new Sprite("Assets/backgrounds/line1.png");
        line2 = new Sprite("Assets/backgrounds/line2.png");
        line3 = new Sprite("Assets/backgrounds/line3.png");
        line1.SetOrigin(line1.width / 2, line1.height / 2);
        line2.SetOrigin(line2.width / 2, line2.height / 2);
        line3.SetOrigin(line3.width / 2, line3.height / 2);
        //testz2.SetScaleXY(1 / 0.7f);
        background = new Sprite("Assets/backgrounds/background.png");
        background.color = 0xffffff;
        background.SetOrigin(background.width / 2, background.height / 2);

        HUD = new Sprite("Assets/backgrounds/HUD.png");
        HUD.SetOrigin(HUD.width / 2, HUD.height / 2);

        ps = new ParticleSystem("Assets\\bubble.png", 0, 0, ParticleSystem.EmitterType.rect, ParticleSystem.Mode.force, this);
        //ps.forces.Add(playerForce);
        //ps.forces[0].magnitude = 300f;
        ps.forces.Add(new ParticleSystem.GravityForce(new Vector2(0, 1)));
        ps.forces[0].magnitude = 0.003f;
        ps.startAlpha = 0.25f;

        ps.lifetime = 2f;
        ps.spawnPeriod = 0.002f;
        //ps.startPosDelta = new Vector2(50, 50);
        ps.worldSpace = this;
        ps.startSpeedDelta = new Vector2(0.5f, 0.5f);
        ps.enabled = false;

        AddChild(background);

        ZOrder.z0 = 10;
        ZOrder.camera = cam;

        ZOrder.Add(layer1, 50);
        ZOrder.Add(line1, Entity.linesZ[0]);
        ZOrder.Add(line2, Entity.linesZ[1]);
        ZOrder.Add(line3, Entity.linesZ[2]);

        line1.scaleX = 1.2f;
        line2.y = 300;
        line3.y = 500;
        lineLayers = new Pivot[]
        {
            ZOrder.zLayer[line1],
            ZOrder.zLayer[line2],
            ZOrder.zLayer[line3],
        };
        //ZOrder.Add(testz2, 0);
        //ZOrder.Add(layer1, -5);

        cam.AddChild(HUD);
        cam.AddChild(canvas);
        AddChild(coin);
        coin.visible = false;
        //AddChild(test);
        coin.AddChild(ps);

        PositionParser.OnPlayerInput += Movement;
        PositionParser.OnPlayerInput += DisplayInput;
        PositionParser.OnPlayerInput += MagicShape.AddStroke;
        PositionParser.OnPlayerInput += LeaderBoard.ChooseLetter;

        MagicShape.FillShapeList();
        MagicShape.LoadSprites();
        MagicShape.CastSpell += SpellEffect;

        PopupSprites.Setup();
        Calibrator.Setup();


        //test.origin = new Vector2(0.5f, 0.5f);
        //Console.WriteLine(test.origin);

        _menu = new Menu(this);
        AddChild(_menu);

        _gameStarted = false;


        player = new Player(64, 64, -resolutionX / 2 + 200, 0);
        Enemy.player = player;
        Enemy.spawnTimer.OnTimerEnd += Enemy.SpawnEnemy;
        Enemy.spawnTimer.SetLaunch(1f);

        player.SwitchLines(0);
        ZOrder.Add(player, 0);

        hud = new HUD();
        cam.AddChild(hud);
        cameraOrigin.AddChild(cam);
        AddChild(cameraOrigin);

        soundManager = new SoundManager();
        AddChild(soundManager);
        soundManager.StartBackMusic();

        StateOfTheGame.SetGameState(StateOfTheGame.GameState.Game);

        comboTimer = new Timer();
        comboTimer.OnTimerEnd += ResetCombo;

        LeaderBoard.LoadScores("score.txt");
        LeaderBoard.display = new EasyDraw(600, 600, false);
        LeaderBoard.display.SetXY(-500, 0);
        LeaderBoard.display.SetOrigin(LeaderBoard.display.width / 2, LeaderBoard.display.height / 2);
        LeaderBoard.SetupKeyboard();
        LeaderBoard.Disable();
        LeaderBoard.UpdateDisplay();
    }

    public static void DisplayInput(Direction dir)
    {
        Console.WriteLine(dir);
    }
    public void Restart()
    {
        player.SwitchLines(0);
        player.score = 0;
        player.HP = 3;
        difficulty = 5;
        combo = 0;
        hud.SetCombo();
        hud.SetHp(3);
        hud.SetScore(0);
        comboMultiplier = 1;
        LeaderBoard.Disable();
        StateOfTheGame.SetGameState(StateOfTheGame.GameState.Game);
        cameraTarget = new Vector2(0, 0);
}
    public override void Update()
    {
        if (arduinoEnabled)
            ArduinoTracker.ReadInput();

        //float amp = (Mathf.Sin(Time.time / 430f + 128f)) * 0.03f;
        //float period = 200f;
        //test.transform = new float[,] {
        //    { amp * Mathf.Sin(Time.time/period) + 1f , Mathf.Cos(Time.time/period) * amp},
        //    { Mathf.Cos(Time.time/period) * amp, -amp * Mathf.Sin(Time.time/period) + 1f}
        //};
        PositionParser.angularDeviation += PositionParser.angularVelocityDeviation * Time.deltaTime;
        PositionParser.GetData();
        PositionParser.UpdateCoordinates();
        PositionParser.FilterMovement();

        Calibrator.Update();

        playerForce.affectorPos = new Vector2(cam.x, cam.y);
        canvas.StrokeWeight(10);
        coin.Animate(0.05f);

        difficulty = 5 + player.score / 1000f;

        cameraOrigin.x = Mathf.Lerp(cameraOrigin.x, cameraTarget.x, followSpeed * Time.deltaTime / 1000);
        cameraOrigin.y = Mathf.Lerp(cameraOrigin.y, cameraTarget.y, followSpeed * Time.deltaTime / 1000);

        ZOrder.ApplyParallax();


        //coin.x = PositionParser.playerAcc.z;
        //coin.y = PositionParser.playerAcc.y;

        //coin.x = PositionParser.playerAccLocal.z;
        //coin.y = PositionParser.playerAccLocal.y;

        coin.x = PositionParser.position.x;
        coin.y = PositionParser.position.y;

        if (Input.GetKeyDown(Key.E))
        {
            coin.y = 0;
            coin.x = 0;
        }
        if (ArduinoTracker.D[4] == 3)
        {
            //Vector2 p1 = canvas.InverseTransformPoint(coin.x, coin.y);
            //Vector2 p2 = canvas.InverseTransformPoint(coinPrevPos.x, coinPrevPos.y);
            //canvas.Line(p1.x, p1.y, p2.x, p2.y);
            //ps.enabled = true;
            ps.startSpeed = (new Vector2(coin.x, coin.y) - coinPrevPos) * 0.2f;
        }
        if (ArduinoTracker.D[4] == 2)
        {
            ps.enabled = false;
            canvas.ClearTransparent();
            MagicShape.SpellAttempt();
        }

        coinPrevPos = new Vector2(coin.x, coin.y);



        player.UpdatePlayer();


        Enemy.UpdateAll();

        hud.HudUpdate();

        if (StateOfTheGame.currentState == StateOfTheGame.GameState.Game)
        {
            if (ArduinoTracker.D[7] == 3)
            {
                PositionParser.Calibrate();
            }

            if (Input.GetKeyDown(Key.W))
                Movement(Direction.UP);
            if (Input.GetKeyDown(Key.S))
                Movement(Direction.DOWN);
            if (Input.GetKeyDown(Key.T))
                MagicShape.SpellPerform(Shape.RED);
            if (Input.GetKeyDown(Key.Y))
                MagicShape.SpellPerform(Shape.BLUE);
            if (Input.GetKeyDown(Key.U))
                MagicShape.SpellPerform(Shape.GREEN);
            if (Input.GetKeyDown(Key.I))
                MagicShape.SpellPerform(Shape.YELLOW);
            if (Input.GetKeyDown(Key.O) && hud.lightCooldownTimer.time <= 0)
            {
                MagicShape.SpellPerform(Shape.LIGHTNING);
                hud.EnableLightCooldown();
            }

            if (Input.GetKeyDown(Key.P) && hud.bombCooldownTimer.time <= 0)
            {
                MagicShape.SpellPerform(Shape.BOMB);
                hud.EnableBombCooldown();
            }
        }
        else if (StateOfTheGame.currentState == StateOfTheGame.GameState.Typing)
        {

            LeaderBoard.EnterYourName();
            if (ArduinoTracker.D[4] == 1 || Input.GetKeyDown(Key.NINE))
                LeaderBoard.EnterSymbol();
            if (ArduinoTracker.D[7] == 1 || Input.GetKeyDown(Key.ZERO))
                LeaderBoard.RemoveSymbol();

            if (Input.GetKeyDown(Key.RIGHT))
                LeaderBoard.ChooseLetter(Direction.RIGHT);
            if (Input.GetKeyDown(Key.LEFT))
                LeaderBoard.ChooseLetter(Direction.LEFT);
            if (Input.GetKeyDown(Key.UP))
                LeaderBoard.ChooseLetter(Direction.UP);
            if (Input.GetKeyDown(Key.DOWN))
                LeaderBoard.ChooseLetter(Direction.DOWN);
        }
        else if (StateOfTheGame.currentState == StateOfTheGame.GameState.GameOver)
            if (Input.GetKeyDown(Key.ENTER))
                Restart();

        if (Input.GetKeyDown(Key.H))
            player.ChangeHP(-3);
        if (Input.GetKeyDown(Key.J))
            LeaderBoard.SaveScores("score.txt");
    }

    public void SpellEffect(Shape shape)
    {
        if (StateOfTheGame.currentState != StateOfTheGame.GameState.Game)
            return;
        switch (shape)
        {
            case Shape.RED:
                CastMagicBall(shape);
                canvas.Clear(255, 0, 0, 10);
                soundManager.SpellCastSoundPlay();
                break;
            case Shape.GREEN:
                CastMagicBall(shape);
                canvas.Clear(0, 255, 0, 10);
                soundManager.SpellCastSoundPlay();
                break;
            case Shape.BLUE:
                CastMagicBall(shape);
                canvas.Clear(0, 0, 255, 10);
                soundManager.SpellCastSoundPlay();
                break;
            case Shape.YELLOW:
                CastMagicBall(shape);
                canvas.Clear(255, 255, 0, 10);
                soundManager.SpellCastSoundPlay();
                break;
            case Shape.LIGHTNING:
                canvas.Clear(255, 255, 255, 10);
                soundManager.ZappingCastSoundPlay();
                break;
            case Shape.BOMB:
                canvas.Clear(128, 0, 128, 10);
                soundManager.BombCastSoundPlay();
                BombEffect bombEffect = new BombEffect(lineLayers[player.line]);

                Enemy closest = player.FindClosestEnemy(player.line);
                Vector2 playerCoords = lineLayers[player.line].InverseTransformVector(player.TransformPoint(0, 0));

                if (closest != null)
                {
                    bombEffect.OnBombHit += closest.OnBombHit;
                    Vector2 enemyCoords = lineLayers[player.line].InverseTransformVector(closest.TransformPoint(0, 0) + closest.velocity * 0.5f);
                    bombEffect.SetTrajectory(playerCoords, enemyCoords, 0.5f);
                }
                else
                    bombEffect.SetTrajectory(playerCoords, playerCoords + new Vector2(1000, 0), 0.5f);
                bombEffect.StartAnimation();
                break;
        }
    }

    public void CastMagicBall(Shape shape)
    {
        uint flags = 0;
        Color color = Color.FromArgb(0xffffff);
        List<Enemy> targets = new List<Enemy>();
        foreach (Enemy target in Enemy.collection[player.line])
        {
            if (target.shapes[0] == shape && !target.isDead)
            {
                targets.Add(target);
            }
        }

        if (targets.Count != 0)
        {
            for (int i=0; i<targets.Count; i++) 
            {
                Enemy target = targets[i];
                if (i == 1)
                    flags |= 0b10;
                if (i == 2)
                {
                    unchecked 
                    {
                        flags &= (uint)~0b10;
                    }
                    flags |= 0b100;
                }    
                CastMagicBall(shape, target, flags);
            }
        }
        else
            CastMagicBall(shape, null, flags);
    }
    public void CastMagicBall (Shape shape, Enemy target, uint flags = 0)
    {
        if (target != null)
            if (target.shapes.Count == 1)
                flags |= 0b1;
        Vector2 playerCoords = lineLayers[player.line].InverseTransformVector(player.TransformPoint(0, 0));
        SpellVFX spell = new SpellVFX(1f, lineLayers[player.line], shape, flags);
        spell.SetTrajectory(playerCoords, target);

        spell.StartAnimation();
    }

    public void IncreaseCombo()
    {
        comboTimer.SetLaunch(3f);
        combo++;
        foreach (int key in comboList.Keys)
        {
            if (combo >= key)
                comboMultiplier = comboList[key];
        }
        hud.StartComboAnimation();
    }
    public Color GetComboColor()
    {
        Color res = Color.White;
        foreach (int key in comboColor.Keys)
        {
            if (combo >= key)
                res = comboColor[key];
        }
        return res;
    }
    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();               // Create a "MyGame" and start it
    }


    public void ResetCombo()
    {
        combo = 0;
        comboMultiplier = 1;
        hud.StartComboAnimation();
    }

    public void Movement(Direction dir)
    {
        if (StateOfTheGame.currentState != StateOfTheGame.GameState.Game)
            return;
        if ( dir == Direction.UP && ArduinoTracker.D[4] == 0)
        {

            if (player.line != 0)
            {
                player.SwitchLines(player.line - 1);
                cameraTarget.y -= 100;
                cameraTarget.x += 80;
            }
        }
        if ( dir == Direction.DOWN && ArduinoTracker.D[4] == 0)
        {
            if (player.line != 2)
            {
                player.SwitchLines(player.line + 1);
                cameraTarget.y += 100;
                cameraTarget.x -= 80;
            }
        }
    }
}