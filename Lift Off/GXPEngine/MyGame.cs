﻿using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using GXPEngine.OpenGL;
using GXPEngine.Core;
using System.Collections.Generic;

public class MyGame : Game
{
    public static MyGame self;
    private static int resolutionX = 1920;
    private static int resolutionY = 1080;

    public ParticleSystem ps;
    public AnimationSprite coin;
    public Camera cam;
    public EasyDraw canvas;
    public Vector2 cameraTarget;
    public Vector2 coinPrevPos;
    public float followSpeed = 0.05f;

    public MagicShape shape = new MagicShape(new Vector2[]
    {
        new Vector2(0,300),
        new Vector2(200,0),
        new Vector2(-200, 0),
        new Vector2(0,-300),
    });

    public ParticleSystem.RadialForce playerForce = new ParticleSystem.RadialForce(new Vector2(0, 0));

    public MyGame() : base(resolutionX, resolutionY, false, pPixelArt: true, pVSync: true)     // Create a window that's 800x600 and NOT fullscreen
    {
        ArduinoTracker.ConnectPort();

        self = this;
        cam = new Camera(0, 0, resolutionX, resolutionY);
        coin = new AnimationSprite("Assets/Coins/coin.png", 5, 1);
        canvas = new EasyDraw(new Bitmap(resolutionX, resolutionY), false);
        canvas.SetXY(-resolutionX/2, -resolutionY/2);

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
        coin.AddChild(ps);
    }

    public void UpdateFixed()
    {
    }
    public override void Update()
    {
        PositionParser.angularDeviation += PositionParser.angularVelocityDeviation * Time.deltaTime;
        ArduinoTracker.ReadInput();
        PositionParser.GetData();
        PositionParser.CalculateGravity();
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

        //cam.x = Mathf.Lerp(cam.x, cameraTarget.x, followSpeed);
        //cam.y = Mathf.Lerp(cam.y, cameraTarget.y, followSpeed);

        coin.x = -ArduinoTracker.gyroy*10;
        coin.y = -ArduinoTracker.gyroz*15;

        if (Input.GetKeyDown(Key.E))
        {
            coin.y = 0;
            coin.x = 0;
        }
        if (ArduinoTracker.D4)
        {
            Vector2 p1 = canvas.InverseTransformPoint(coin.x,coin.y);
            Vector2 p2 = canvas.InverseTransformPoint(coinPrevPos.x, coinPrevPos.y);
            canvas.Line(p1.x, p1.y, p2.x, p2.y);
            ps.enabled = true;
            ps.startSpeed = (new Vector2(coin.x,coin.y) - coinPrevPos)*0.2f;

            shape.CheckPoints(new Vector2(coin.x, coin.y));
            if (shape.ShapeSDF(new Vector2(coin.x, coin.y))>MagicShape.precision)
                shape.failed = true;    
        }
        else
        {
            shape.failed = false;
            shape.Reset();
            ps.enabled = false;
            canvas.ClearTransparent();
        }

        shape.Draw(canvas);
        if (shape.ShapeSDF(new Vector2(coin.x, coin.y)) > MagicShape.precision)
            coin.color = 0xff0000;
        else
            coin.color = 0xffffff;
        coinPrevPos = new Vector2(coin.x, coin.y);
    }
    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new MyGame().Start();               // Create a "MyGame" and start it
    }
}