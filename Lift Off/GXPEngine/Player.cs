using GXPEngine;
using GXPEngine.Animation;
using GXPEngine.Core;
using System;
using System.Drawing;
using System.Runtime.Remoting.Lifetime;


public class Player : Entity
{
    protected float speed = 2f;

    public int HP;
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

    SoundManager soundManager;

    public Player(float width, float height, float x = 0, float y = 0) : base (width, height)
    {
        SetEntitySprites("Assets/player/idle.png", 8, 1, 0);
        SetEntitySprites("Assets/player/hit.png", 3, 1, 1);
        SetEntitySprites("Assets/player/death.png", 20, 1, 2);
        SetEntitySprites("Assets/player/cast1.png", 8, 1, 3);
        SetEntitySprites("Assets/player/cast2.png", 8, 1, 4);
        SetEntitySprites("Assets/player/cast3.png", 8, 1, 5);
        SetEntitySprites("Assets/player/cast4.png", 8, 1, 6);
        SetEntitySprites("Assets/player/death.png", 20, 1, 7);
        SetEntitySprites("Assets/player/disappear.png", 8, 1, 8, 30f);
        SetEntitySprites("Assets/player/appear.png", 7, 1, 9, 30f);
        SetEntitySprites("Assets/player/lightning.png", 12, 1, 10, 10f);
        SetEntitySprites("Assets/player/bomb.png", 8, 1, 11, 10f);

        states[7].SetCycle(18, 1);

        targetPosition = new Vector3(x, y, 0);
        SetXY(x, y);
        SetScaleXY(0.5f, 0.5f);

        HP = 3;
        score = 0;
        this.width = width;
        this.height = height;

        attackTimer = new Timer();
        movementTimer = new Timer();

        soundManager = new SoundManager();

        SetEntityState(0);
    }

    private void HandleInput()
    {

        if (Input.GetKeyDown(Key.W) || Input.GetKeyDown(Key.S))
        {
        }

        else if (attackTimer.time <= 0.0f && movementTimer.time <= 0.0f)
        {
            moving = false;
        }

        if (Input.GetMouseButtonDown(0) && !moving)
        {
            AnimateOnce(Utils.Random(3,7), 0);
        }
    }
    public void SwitchLines(int line)
    {
        this.line = line;
        targetPosition.y = linesY[line];
        targetPosition.z = linesZ[line];

        attackTimer.Reset();
        movementTimer.SetLaunch(0.5f);
        movementTimer.OnTimerEnd += StopMovement;
        moving = true;
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
        return HP;
    }
    public void ChangeHP(int hp)
    {
        if (StateOfTheGame.currentState != StateOfTheGame.GameState.Game)
            return;
        HP += hp;
        if (HP < 0)
            HP = 0;
        hpDecreased = true;
        soundManager.PlayerHurtSoundPlay();
        HUD.self.SetHp(HP);
        AnimateOnce(1, 0);
        if (HP == 0)
        {
            AnimateOnce(2, 7, 19);
            soundManager.GameOverSoundPlay();
            LeaderBoard.AddScore("", score);
            LeaderBoard.Enable();
            Enemy.DestroyAllEnemies();
        }
    }

    public int GetScore()
    {
        return score;
    }
    public void ChangeScore(int score)
    {
        this.score+=score;
        HUD.self.SetScore(this.score);
        PopupAnimation popup = new PopupAnimation("+" + score, 0.8f, 0,-MyGame.self.height/2+200, MyGame.self.cam, MyGame.self.GetComboColor());
        popup.StartAnimation();
    }

    public void WinGame()
    {
        isWin = true;
    }

    public void Death()
    {
        isDead = true;
    }

    public void StopMovement()
    {
        SetEntityState(0);
        movementTimer.Reset();

        x = targetPosition.x;
        y = targetPosition.y;
        ZOrder.SetZ(this, targetPosition.z);
    }

    public void UpdatePlayer()
    {
        HandleInput();
        //x = Mathf.CosLerp(x, targetPosition.x, smoothingRate * Time.deltaTime/1000f);
        //y = Mathf.CosLerp(y, targetPosition.y, smoothingRate * Time.deltaTime/1000f);
        //float z = ZOrder.GetZ(this);
        //z = Mathf.CosLerp(z, targetPosition.z, smoothingRate * Time.deltaTime / 1000f);
        //ZOrder.SetZ(this, z);

        if (moving)
        {
            if (movementTimer.time > 0.25f)
            {
                if (currentState != 8)
                    SetEntityState(8);
                else
                    if (states[currentState].currentFrame == states[currentState].frameCount - 1)
                        states[currentState].currentFrame = states[currentState].frameCount - 2;
            }
            if (movementTimer.time < 0.25f)
            {
                x = targetPosition.x;
                y = targetPosition.y;
                //z = targetPosition.z;
                ZOrder.SetZ(this, targetPosition.z);

                if (currentState != 9)
                    SetEntityState(9);
                else
                    if (states[currentState].currentFrame == states[currentState].frameCount - 1)
                    states[currentState].currentFrame = states[currentState].frameCount - 2;
            }
        }

        Animate();
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
                HP--;
                if (HP == 0)
                {
                    gameOver.Play();
                    Death();
                }
            }
        }
    }*/