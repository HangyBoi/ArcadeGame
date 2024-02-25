using GXPEngine;
using GXPEngine.Core;
using System;
using System.Drawing;
using TiledMapParser;


public class Player : Entity
{
    protected float speed = 2f;

    protected int _HP;
    public float width;
    public float height;

    public bool isDead = false;
    public bool isWin = false;
    protected bool isFacingRight = true;


    public Player()
    {
        SetEntityIdleSprites("sprites/player/B_witch_idle.png", 1, 6);
        SetEntityRunSprites("sprites/player/B_witch_run.png", 1, 8);
        SetEntityAttackSprites("sprites/player/B_witch_attack.png", 1, 9);
        SetEntityDeathSprites("sprites/player/B_witch_death.png", 1, 12);

        SetXY(0, 100);
        SetScaleXY(2, 2);

        _HP = 3;
        width = 30;
        height = 30;
    }

    private void HandleInput()
    {

        if (Input.GetKeyDown(Key.W) || Input.GetKey(Key.S))
        {
            SetEntityState(EntityState.Run);
            states[(int)EntityState.Run].Mirror(false, false);
            moving = true;
            isFacingRight = true;
        }
        else
        {
            moving = false;
            SetEntityState(EntityState.Idle);
        }

        SwitchLines();

        if (Input.GetMouseButton(0) && !moving)
        {
            SetEntityState(EntityState.Attack);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            states[(int)EntityState.Attack].SetFrame(0);
        }
    }

    private void SwitchLines()
    {
        {
            int closestLine = GetClosestLine(y, linesY);

            if (Input.GetKeyDown(Key.W) && closestLine > 0)
            {
                y = linesY[closestLine - 1]; // Move up to the previous line
            }
            else if (Input.GetKeyDown(Key.S) && closestLine < linesY.Length - 1)
            {
                y = linesY[closestLine + 1]; // Move down to the next line
            }
        }

    }

    private int GetClosestLine(float playerY, int[] linesY)
    {
        float minDistance = float.MaxValue;
        int closestLine = 0;

        for (int i = 0; i < linesY.Length; i++)
        {
            float distance = Mathf.Abs(playerY - linesY[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestLine = i;
            }
        }

        return closestLine;
    }

    public int GetHP()
    {
        return _HP;
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