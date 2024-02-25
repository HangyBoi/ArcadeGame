using System;
using GXPEngine;
using GXPEngine.Core;
using System.Collections.Generic;

public class MyGame : Game
{
    private static int resolutionX = 1920;
    private static int resolutionY = 1080;

    public Vector2 cameraTarget;

    protected StateOfTheGame gameState;
    protected Player player;
    public List<Enemy> enemies = new List<Enemy>();

    private float timeSinceLastSpawn;
    private float spawnInterval = 5000;

    public MyGame() : base(resolutionX, resolutionY, false, pPixelArt: true, pVSync: true)
    {
        player = new Player();
        AddChild(player);

        gameState = new StateOfTheGame();
        AddChild(gameState);
        gameState.SetGameState(StateOfTheGame.GameState.Menu);

        timeSinceLastSpawn = Time.time;
    }

    public void Update()
    {
        player.UpdatePlayer();

        if (Time.time - timeSinceLastSpawn > spawnInterval)
        {
            SpawnEnemy();
            timeSinceLastSpawn = Time.time;
        }

        foreach (Enemy enemy in enemies)
        {
            enemy.UpdateEnemy();

            //Move to enemy manager
            if (enemy.x < -100)
            {
                enemy.Death();
                break;
            }
        }

        if (Input.GetMouseButton(0)) {
            DestroyFrontEnemy();
        }

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

    //Should be moved to Enemy class or EnemyManager class!
    void SpawnEnemy()
    {
        enemies.Add(new Enemy(width));

        foreach (Enemy enemy in enemies)
        {
            AddChild(enemy);
        }
    }

    //Move to EnemyManager class!
    void DestroyFrontEnemy()
    {
        foreach (Enemy enemy in enemies)
        {
            if (IsEnemyInFront(enemy) && player.y == enemy.y)
            {
                enemy.Death();
                break;
            } 
        }
    }

    //Move to EnemyManager class!
    private bool IsEnemyInFront(Enemy enemy)
    {
        return Math.Abs(player.x - enemy.x) < player.width * 5;
    }

    static void Main()
    {
        new MyGame().Start();
    }
}