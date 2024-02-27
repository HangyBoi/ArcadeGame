using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using GXPEngine.OpenGL;
using GXPEngine.Core;
using GXPEngine.Animation;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MyGame : Game
{
    public static MyGame self;
    public Menu _menu;
    private bool _gameStarted;
    private static int resolutionX = 1920;
    private static int resolutionY = 1080;

    public ParticleSystem ps;
    public AnimationSprite coin;
    public Camera cam;
    public EasyDraw canvas;
    public Sprite test;
    public Vector2 cameraTarget;
    public Vector2 coinPrevPos;
    public float followSpeed = 0.05f;
    public float crossLine = -resolutionX/2;


    public ParticleSystem.RadialForce playerForce = new ParticleSystem.RadialForce(new Vector2(0, 0));


    protected StateOfTheGame gameState;
    protected Player player;

    protected HUD hud;

    private float timeSinceLastSpawn;
    private float spawnInterval = 3000;

    public MyGame() : base(resolutionX, resolutionY, false, pPixelArt: false, pVSync: true)     // Create a window that's 800x600 and NOT fullscreen
    {
        //ArduinoTracker.ConnectPort();

        self = this;
        cam = new Camera(0, 0, resolutionX, resolutionY);
        coin = new AnimationSprite("Assets/Coins/coin.png", 5, 1);
        canvas = new EasyDraw(new Bitmap(resolutionX, resolutionY), false);
        canvas.SetXY(-resolutionX/2, -resolutionY/2);

        test = new Sprite("Assets\\bg.jpg");
        test.SetXY(-100,-100);

        ps = new ParticleSystem("Assets\\bubble.png", 0, 0, ParticleSystem.EmitterType.rect, ParticleSystem.Mode.force, this);
        //ps.forces.Add(playerForce);
        //ps.forces[0].magnitude = 300f;
        ps.forces.Add(new ParticleSystem.GravityForce(new Vector2(0,1)));
        ps.forces[0].magnitude = 0.003f;
        ps.startAlpha = 0.25f;

        ps.lifetime = 2f;
        ps.spawnPeriod = 0.002f;
        //ps.startPosDelta = new Vector2(50, 50);
        ps.worldSpace = this;
        ps.startSpeedDelta = new Vector2(0.5f, 0.5f);
        ps.enabled = false;

        AddChild(cam);
        AddChild(canvas);
        AddChild(coin);
        //AddChild(test);
        coin.AddChild(ps);

        PositionParser.OnPlayerInput += DisplayInput;
        PositionParser.OnPlayerInput += MagicShape.AddStroke;

        MagicShape.FillShapeList();
        MagicShape.LoadSprites();
        MagicShape.CastSpell += SpellEffect;
        Calibrator.Setup();


        //test.origin = new Vector2(0.5f, 0.5f);
        //Console.WriteLine(test.origin);

        _menu = new Menu(this);
        AddChild(_menu);

        _gameStarted = false;

        player = new Player(64, 64, -resolutionX/2 + 200, 0);
        Enemy._player = player;
        AddChild(player);

        hud = new HUD();
        AddChild(hud);

        gameState = new StateOfTheGame();
        AddChild(gameState);
        gameState.SetGameState(StateOfTheGame.GameState.Menu);

        timeSinceLastSpawn = Time.time;
    }


    public static void DisplayInput(Direction dir)
    {
        Console.WriteLine(dir);
    }
    public void UpdateFixed()
    {
    }
    public override void Update()
    {
        //ArduinoTracker.ReadInput();

        float amp = (Mathf.Sin(Time.time / 430f + 128f)) * 0.03f;
        float period = 200f;
        test.transform = new float[,] {
            { amp * Mathf.Sin(Time.time/period) + 1f , Mathf.Cos(Time.time/period) * amp},
            { Mathf.Cos(Time.time/period) * amp, -amp * Mathf.Sin(Time.time/period) + 1f}
        };
        PositionParser.angularDeviation += PositionParser.angularVelocityDeviation * Time.deltaTime;
        PositionParser.GetData();
        PositionParser.UpdateCoordinates();
        PositionParser.FilterMovement();

        Calibrator.Update();

        playerForce.affectorPos = new Vector2(cam.x, cam.y);
        canvas.StrokeWeight(10);
        coin.Animate(0.05f);
        if (Input.GetKey(Key.D))
            cameraTarget.x += Time.deltaTime;
        if (Input.GetKey(Key.A))
            cameraTarget.x -= Time.deltaTime;
        if (Input.GetKey(Key.W))
            cameraTarget.y -= Time.deltaTime;
        if (Input.GetKey(Key.S))
            cameraTarget.y += Time.deltaTime;

        if (Input.GetKeyDown(Key.E))
        { 
            BombEffect bombEffect = new BombEffect();
            Enemy closest = player.FindClosestEnemy(player.line);
            if (closest != null)
                bombEffect.SetTrajectory(new Vector2(player.x, player.y), new Vector2(closest.x + closest.velovity.x * 0.5f, closest.y + closest.velovity.y * 0.5f), 0.5f);
            else
                bombEffect.SetTrajectory(new Vector2(player.x, player.y), new Vector2(player.x + 1000, player.y), 0.5f);
            bombEffect.StartAnimation();
        }

        //coin.x = PositionParser.playerAcc.z;
        //coin.y = PositionParser.playerAcc.y;

        //coin.x = PositionParser.playerAccLocal.z;
        //coin.y = PositionParser.playerAccLocal.y;

        coin.x = PositionParser.position.x;
        coin.y = PositionParser.position.y;

        if (ArduinoTracker.D[7] == 3)
        {
            PositionParser.Calibrate();
        }
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

        if (Time.time - timeSinceLastSpawn > spawnInterval)
        {
            Enemy.SpawnEnemy();
            timeSinceLastSpawn = Time.time;
        }

        Enemy.UpdateAll();

        hud.HudUpdate(player);

        if (Input.GetKeyDown(Key.T))
            MagicShape.SpellPerform(Shape.RED);
        if (Input.GetKeyDown(Key.Y))
            MagicShape.SpellPerform(Shape.BLUE);
        if (Input.GetKeyDown(Key.U))
            MagicShape.SpellPerform(Shape.GREEN);
        if (Input.GetKeyDown(Key.I))
            MagicShape.SpellPerform(Shape.YELLOW);
        if (Input.GetKeyDown(Key.O))
            MagicShape.SpellPerform(Shape.LIGHTNING);
        if (Input.GetKeyDown(Key.P))
            MagicShape.SpellPerform(Shape.BOMB);

        if (Input.GetKeyDown(Key.TAB))
        {
            Console.WriteLine(gameState.currentState);
        }

        if (Input.GetKeyDown(Key.SPACE))
        {
            gameState.SetGameState(StateOfTheGame.GameState.PlayingLevel);
            Console.WriteLine(gameState.currentState);
        }
    }

    public void SpellEffect(Shape shape)
    {
        switch (shape)
        {
            case Shape.RED:
                canvas.Clear(255, 0, 0, 50);
                break;
            case Shape.GREEN:
                canvas.Clear(0, 255, 0, 50);
                break;
            case Shape.BLUE:
                canvas.Clear(0, 0, 255, 50);
                break;
            case Shape.YELLOW:
                canvas.Clear(255, 255, 0, 50);
                break;
            case Shape.LIGHTNING:
                canvas.Clear(255, 255, 255, 50);
                break;
            case Shape.BOMB:
                canvas.Clear(128, 0, 128, 50);
                break;
        }
    }
    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();               // Create a "MyGame" and start it
    }
}