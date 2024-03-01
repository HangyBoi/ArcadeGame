using GXPEngine;
using GXPEngine.Animation;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml.Schema;

public class Enemy : Entity
{
    public static Player player;
    public static List<Enemy>[] collection = new List<Enemy>[]
        {
            new List<Enemy>(),
            new List<Enemy>(),
            new List<Enemy>()
        };

    protected float enemyX;
    protected float enemyY;
    public int line;
    public bool isDead = false;
    public Pivot spellDisplay;

    public static Timer spawnTimer = new Timer();
    SoundManager soundManager;

    public List<Shape> shapes = new List<Shape>();

    public Vector2 velocity = new Vector2(-150f, 0);
    public Enemy(float width, float height, float x, float y, int line) : base(width, height)
    {
        int variation = Utils.Random(0, 3);
        switch (variation)
        {
            case 0:
                SetEntitySprites("Assets/enemy/enemy1.png", 8, 1, 0);
                states[0].SetScaleXY(0.5f, 0.5f);
                break;
            case 1:
                SetEntitySprites("Assets/enemy/enemy2.png", 8, 1, 0);
                states[0].SetScaleXY(0.5f, 0.5f);
                break;
            case 2:
                SetEntitySprites("Assets/enemy/enemy3.png", 9, 1, 0);
                states[0].SetScaleXY(0.5f, 0.5f);
                break;
        }
        //SetScaleXY(3,3);
        this.x = x;
        this.y = y;
        this.line = line;
        spellDisplay = new Pivot();
        AddChild(spellDisplay);
        spellDisplay.x = 0;
        spellDisplay.y = 0;

        soundManager = new SoundManager();
    }


    public void Score(Shape shape, uint flags = 0)
    {
        int finalScore = 0;
        if (isDead || shapes.Count == 0)
            return;
        HUD.self.ScoreAnimation();
        if (shape == shapes[0] || shape == Shape.BOMB || shape == Shape.LIGHTNING)
        {
            MyGame.self.IncreaseCombo();
            finalScore += 50;
        }
        if ((flags & 0b1) != 0)
            finalScore += 20;

        if ((flags & 0b10) != 0)
            finalScore += 50;

        if ((flags & 0b100) != 0)
            finalScore += 150;

        if ((flags & 0b1000) != 0)
            finalScore += 50;

        finalScore = (int)(finalScore * MyGame.self.comboMultiplier);
        if (finalScore > 0)
            player.ChangeScore(finalScore);
    }
    public void Death()
    {
        collection[line].Remove(this);
        LateDestroy();
        isDead = true;
    }

    public void UpdateEnemy()
    {
        Move(velocity.x * Time.deltaTime / 1000, velocity.y * Time.deltaTime / 1000);
        Animate();
    }

    public static void SpawnEnemy()
    {
        if (StateOfTheGame.currentState != StateOfTheGame.GameState.Game)
            return;
        spawnTimer.SetLaunch(10f / Mathf.Log(MyGame.self.difficulty));
        float rgn = Utils.Random(0f, 1f);
        if (rgn < Mathf.Clamp((Mathf.Log(MyGame.self.difficulty - 3) - 1f) / 20, 0f, 1f))
            SpawnSpecial();
        else
            SpawnUsual();
    }
    public static void SpawnUsual()
    {
        int line = Utils.Random(0, 3);
        Enemy enemy = new Enemy(512, 512, MyGame.self.width / 2 - 200, linesY[line] + Utils.Random(-50, 50), line);
        collection[line].Add(enemy);
        MyGame.self.lineLayers[line].AddChild(enemy);
        enemy.GenerateSpells();
        //MagicShape.CastSpell += enemy.Damage;
        spawnTimer.SetLaunch(10f / Mathf.Log(MyGame.self.difficulty));
    }

    public void Damage(Shape shape)
    {
        if (shapes.Count > 0)
            if (shapes[0] == shape)
                Damage();
    }

    public void Damage()
    {
        if (shapes.Count > 1)
        {
            RemoveSpell();
            soundManager.EnemySoundPlay();
        }
        else
        {
            RemoveSpell();
            Death();
            soundManager.EnemySoundPlay();
        }
    }
    public bool IsEnemyInFront()
    {
        return x - player.x < player.reachDistance;
    }

    public static void UpdateAll()
    {
        int totalEnemies = 0;
        for (int i = 0; i < 3; i++)
            foreach (var enemy in collection[i])
            {
                totalEnemies++;
                enemy.UpdateEnemy();

                //Move to enemy manager
                if (enemy.x < MyGame.self.crossLine)
                {
                    enemy.Death();
                    break;
                }

                if (Math.Abs(enemy.x - MyGame.self.crossLine) < 10f)
                {
                    enemy.Death();
                    player.ChangeHP(-1);
                    break;
                }

                player.hpDecreased = false;
            }
        if (totalEnemies == 0)
        {
            SpawnEnemy();
        }
    }

    public void GenerateSpells()
    {
        shapes = new List<Shape>();
        int shapeCount = (int)Utils.Random(1, Mathf.Clamp(Mathf.Log(MyGame.self.difficulty / 2) * 2 ,1,6));

        for (int i = 0; i < shapeCount; i++)
        {
            Shape sh = (Shape)Utils.Random(0, 4);
            shapes.Add(sh);
        }
        DrawSpells();
    }
    public void DrawSpells()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            Sprite symbol = new Sprite(MagicShape.spellSprite[(int)shapes[i]],false);
            spellDisplay.AddChild(symbol);
            symbol.SetOrigin(symbol.width / 2, symbol.height / 2);
            symbol.SetXY(i * 40 - (shapes.Count - 1) * 20, -height/8);
        }
    }
    public void RemoveSpell()
    {
        if (shapes.Count > 0)
        {
            shapes.RemoveAt(0);

            List<GameObject> spells = spellDisplay.GetChildren();
            if (spells.Count > 0)
            {

                spellDisplay.RemoveChild(spells[0]);
                spells[0].LateDestroy();
                spells.RemoveAt(0);

                foreach (GameObject go in spells)
                {
                    go.Move(-20, 0);
                }
            }
        }
    }

    public static void SpawnSpecial()
    {
        int id = Utils.Random(0, 2);

        if (id == 0)
        {
            int line = Utils.Random(0, 3);
            Enemy enemy = new Enemy(512, 512, MyGame.self.width / 2 - 200, linesY[line] + Utils.Random(-50, 50), line);
            collection[line].Add(enemy);
            MyGame.self.lineLayers[line].AddChild(enemy);

            enemy.shapes = new List<Shape>() { Shape.BLUE, Shape.RED, Shape.BLUE, Shape.RED, Shape.BLUE, Shape.RED, Shape.BLUE };

            enemy.DrawSpells();

        }
        if (id == 1)
        {
            int line = Utils.Random(0, 3);
            List<Enemy> enemies = new List<Enemy>();

            enemies.Add(new Enemy(512, 512, MyGame.self.width / 2 - 400, linesY[line] + Utils.Random(-50, 50), line));
            enemies.Add(new Enemy(512, 512, MyGame.self.width / 2 - 300, linesY[line] + Utils.Random(-50, 50), line));
            enemies.Add(new Enemy(512, 512, MyGame.self.width / 2 - 200, linesY[line] + Utils.Random(-50, 50), line));

            foreach (Enemy enemy in enemies)
            {
                collection[line].Add(enemy);
                MyGame.self.lineLayers[line].AddChild(enemy);
            }
            for (int i = 0; i < 3; i++)
            {
                Shape sh = (Shape)Utils.Random(0, 4);

                for (int j = 0; j < 3; j++)
                {
                    if (i <= j)
                    {
                        enemies[j].shapes.Add(sh);
                    }
                }
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.shapes.Reverse();
                enemy.DrawSpells();
            }
        }
    }
    public static void DestroyAllEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = collection[i].Count - 1; j >= 0; j--)
            {
                Enemy enemy = collection[i][j];
                enemy.Death();
            }
        }
    }
    public void OnBombHit()
    {
        Score(Shape.BOMB, 0b1);
        Death();
    }
}