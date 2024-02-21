using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using GXPEngine.OpenGL;
using GXPEngine.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class MyGame : Game
{
    public static MyGame self;
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


    public MagicShape shape = new MagicShape(new Vector2[]
    {
        new Vector2(0,300),
        new Vector2(200,0),
        new Vector2(-200, 0),
        new Vector2(0,-300),
    },
    new direction[]
    {
        direction.UP_RIGHT, 
        direction.LEFT, 
        direction.UP_RIGHT,
    });

    public ParticleSystem.RadialForce playerForce = new ParticleSystem.RadialForce(new Vector2(0, 0));

    public MyGame() : base(resolutionX, resolutionY, false, pPixelArt: false, pVSync: true)     // Create a window that's 800x600 and NOT fullscreen
    {
        ArduinoTracker.ConnectPort();

        self = this;
        cam = new Camera(0, 0, resolutionX, resolutionY);
        coin = new AnimationSprite("Assets/Coins/coin.png", 5, 1);
        canvas = new EasyDraw(new Bitmap(resolutionX, resolutionY), false);
        canvas.SetXY(-resolutionX/2, -resolutionY/2);

        test = new Sprite("Assets\\bush.jpg");
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

        AddChild(cam);
        AddChild(canvas);
        AddChild(coin);
        //AddChild(test);
        coin.AddChild(ps);

        PositionParser.OnPlayerInput += DisplayInput;
        Calibrator.Setup();

        test.origin = new Vector2(0.5f, 0.5f);
        Console.WriteLine(test.origin);

    }


    public static void DisplayInput(direction dir)
    {
        Console.WriteLine(dir);
    }
    public void UpdateFixed()
    {
    }
    public override void Update()
    {
        ArduinoTracker.ReadInput();

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
            ps.enabled = true;
            ps.startSpeed = (new Vector2(coin.x, coin.y) - coinPrevPos) * 0.2f;
        }
        else
        {
            shape.Reset();
            ps.enabled = false;
            canvas.ClearTransparent();
        }

        shape.Draw(canvas);
        coinPrevPos = new Vector2(coin.x, coin.y);
    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();               // Create a "MyGame" and start it
    }
}