using GXPEngine;
using GXPEngine.Core;
using System;
using System.Drawing;


public class Player : Entity
{
    protected float speed = 2f;

    protected bool isFacingRight = true;

    protected int _HP;

    public bool isDead = false;
    public bool isWin = false;

    private Sound winMusic;

    public Player()
    {
        SetEntityIdleSprites("Assets/B_witch_idle.png", 1, 6);
        SetEntityRunSprites("Assets/B_witch_run.png", 1, 8);
        SetEntityAttackSprites("Assets/B_witch_attack.png", 1, 9);

        _HP = 3;

        winMusic = new Sound("sounds/win.mp3", false, false);
    }

    private void HandleInput()
    {
        SetEntityState(EntityState.Idle);

        if (Input.GetKey(Key.A))
        {
            states[(int)EntityState.Idle].Mirror(true, false);
            isFacingRight = false;
        }

        if (Input.GetKey(Key.D))
        {
            states[(int)EntityState.Idle].Mirror(false, false);
            isFacingRight = true;
        }

        /*

        if (Input.GetKey(Key.W))
        {
            moving = true;
            Move(0, -speed);
            SetEntityState(EntityState.Run);
            states[(int)EntityState.Run].Mirror(false, false);
            isFacingRight = true;
        }

        if (Input.GetKey(Key.S))
        {
            moving = true;
            Move(0, speed);
            SetEntityState(EntityState.Run);
            states[(int)EntityState.Run].Mirror(false, false);
            isFacingRight = true;
        }

        if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
        {
            moving = false;
            SetEntityState(EntityState.Idle);
            if (isFacingRight == false)
            {
                states[(int)EntityState.Idle].Mirror(true, false);
            }
            else
            {
                states[(int)EntityState.Idle].Mirror(false, false);
            }
        }

        if (Input.GetKey(Key.X) && !moving)
        {
            SetEntityState(EntityState.Attack);
        }
        else if (Input.GetKeyUp(Key.X))
        {
            states[(int)EntityState.Attack].SetFrame(0);
        }*/
    }

    public int GetScore()
    {
        return -1;
    }

    public int GetHP()
    {
        return _HP;
    }

    public void WinGame()
    {
        isWin = true;
        winMusic.Play();
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