using GXPEngine;
using GXPEngine.Core;
using System;

public class Enemy : Entity
{
    protected float enemyX;
    protected float enemyY;

    private float velX = 3f;

    public Enemy(int width)
    {
        SetEntityRunSprites("sprites/enemy/Enemy_Fly.png", 4, 1);
        enemyY = linesY[Utils.Random(0, 3)];
        enemyX = width - 200;
        SetXY(enemyX, enemyY);
        SetScaleXY(3, 3);
    }

    public void Death()
    {
        Console.WriteLine("enemy died");
        MyGame mainGame = (MyGame)game;
        mainGame.enemies.Remove(this);
        LateDestroy();
    }


    public void UpdateEnemy()
    {
        Move(-velX, 0);
    }

}