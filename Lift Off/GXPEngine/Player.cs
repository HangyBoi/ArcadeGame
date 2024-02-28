﻿using GXPEngine;
using GXPEngine.Animation;
using GXPEngine.Core;
using System;
using System.Drawing;
using System.Runtime.Remoting.Lifetime;


public class Player : Entity
{
    protected float speed = 2f;

    public int _HP;
    public bool hpDecreased;
    public int score;
    public int line;
    public float reachDistance = 2000;

    public bool isDead = false;
    public bool isWin = false;
    protected bool isFacingRight = true;
    public Vector3 targetPosition;
    public float smoothingRate = 20f;

    Timer attackTimer;
    Timer movementTimer;

    public Player(float width, float height, float x = 0, float y = 0) : base (width, height)
    {
        SetEntitySprites("Assets/player/B_witch_idle.png", 1, 6, 0);
        SetEntitySprites("Assets/player/B_witch_run.png", 1, 8, 1);
        SetEntitySprites("Assets/player/B_witch_attack.png", 1, 9, 2);
        SetEntitySprites("Assets/player/B_witch_death.png", 1, 12, 3);

        targetPosition = new Vector3(x, y, 0);
        SetXY(x, y);
        //SetScaleXY(2, 2);

        _HP = 3;
        score = 0;
        this.width = width;
        this.height = height;

        attackTimer = new Timer();
        movementTimer = new Timer();

        SetEntityState(EntityState.Idle);
    }

    private void HandleInput()
    {

        if (Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.S))
        {
            attackTimer.Reset();
            movementTimer.SetLaunch(0.2f);
            moving = true;
            SetEntityState(EntityState.Run);
        }

        else if (attackTimer.time <= 0.0f && movementTimer.time <= 0.0f)
        {
            moving = false; 
            SetEntityState(EntityState.Idle);
        }

        if (Input.GetMouseButtonDown(0) && !moving)
        {
            attackTimer.SetLaunch(0.8f);

            SetEntityState(EntityState.Attack);
        }
    }
    public void SwitchLines(int line)
    {
        this.line = line;
        targetPosition.y = linesY[line];
        targetPosition.z = linesZ[line];
    }

    //private int GetClosestLine(float playerY, int[] linesY)
    //{
    //    float minDistance = float.MaxValue;
    //    int closestLine = 0;

    //    for (int i = 0; i < linesY.Length; i++)
    //    {
    //        float distance = Mathf.Abs(playerY - linesY[i]);
    //        if (distance < minDistance)
    //        {
    //            minDistance = distance;
    //            closestLine = i;
    //        }
    //    }

    //    return closestLine;
    //}

    public Enemy FindClosestEnemy(int line)
    {
        Enemy closest = null;
        foreach (Enemy enemy in Enemy.collection[line]) 
        {
            if (closest == null)
            {
                closest = enemy;
            }
            if (enemy.x < closest.x)
                closest = enemy;
        }
        return closest;
    }

    public int GetHP()
    {
        return _HP;
    }

    public int GetScore()
    {
        return score;
    }

    public void WinGame()
    {
        isWin = true;
    }

    public void Death()
    {
        isDead = true;
    }

    public void UpdatePlayer()
    {
        HandleInput();
        Animate();
        x = Mathf.CosLerp(x, targetPosition.x, smoothingRate * Time.deltaTime/1000f);
        y = Mathf.CosLerp(y, targetPosition.y, smoothingRate * Time.deltaTime/1000f);
        float z = ZOrder.GetZ(this);
        z = Mathf.CosLerp(z, targetPosition.z, smoothingRate * Time.deltaTime / 1000f);
        ZOrder.SetZ(this, z);
    }
}


/*    private void CheckColissions()
    {
        GameObject[] collidngObjects = GetCollisions();
        foreach (GameObject collidingObject in collidngObjects)
        {
            if (collidingObject is Enemy)
            {
                Enemy enemy = (Enemy)collidingObject;
                Console.WriteLine("Enemy Touched!");
                enemy.LateDestroy();
                hitSound.Play();
                _HP--;
                if (_HP == 0)
                {
                    gameOver.Play();
                    Death();
                }
            }
        }
    }*/