using GXPEngine;
using GXPEngine.Core;
using System;
using System.Drawing;

public class Enemy : Entity
{
    public Player _player;

    private bool isFollowingPlayer = false;
    private float followDistance = 300f;

    protected float playerX;
    protected float playerY;
    protected float enemyX;
    protected float enemyY;


    private float velX = 1.5f;
    private float velY = 1;

    public Sound buzzSound;

    public Enemy(Player player)
    {
        _player = player;

        buzzSound = new Sound("sounds/bee_buzz.mp3", false, false);

        SetEntityIdleSprites("Assets/Enemy_Fly.png", 4, 1);
        SetEntityRunSprites("Assets/Enemy_Fly.png", 4, 1);
        SetEntityAttackSprites("Assets/Enemy_Attack.png", 4, 1);
    }

    public void Death()
    {
        buzzSound.Play();
        Console.WriteLine("enemy died");
        LateDestroy();
    }

    public void UpdateEnemy()
    {
        if (PlayerInRange())
        {
            isFollowingPlayer = true;
            FollowPlayer();
        }
    }

    private bool PlayerInRange()
    {
        float distanceToPlayer = DistanceToPlayer();
        return distanceToPlayer <= followDistance;
    }

    private void FollowPlayer()
    {
        SetEntityState(EntityState.Run);
        playerX = _player.x;
        playerY = _player.y;

        enemyX = x;
        enemyY = y;

        if (Mathf.Abs(playerX - enemyX) > 1)
        {
            if (playerX < enemyX)
            {
                Move(-velX, 0);
                states[(int)EntityState.Run].Mirror(false, false);
            }
            else
            {
                Move(velX, 0);
                states[(int)EntityState.Run].Mirror(true, false);
            }
        }

        if (Mathf.Abs(playerY - enemyY) > 1)
        {
            if (playerY < enemyY)
            {
                Move(0, -velY);
            }
            else
            {
                Move(0, velY);
            }
        }
    }

    private float DistanceToPlayer()
    {
        playerX = _player.x;
        playerY = _player.y;
        enemyX = x;
        enemyY = y;

        return Mathf.Sqrt((playerX - enemyX) * (playerX - enemyX) + (playerY - enemyY) * (playerY - enemyY));
    }
}