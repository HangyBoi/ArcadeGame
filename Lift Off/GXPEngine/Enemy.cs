using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
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

    public List<Shape> shapes = new List<Shape>();

    public Vector2 velocity = new Vector2(-150f, 0);

    public Sound buzzSound;
    public Enemy(float width, float height, float x, float y, int line) : base(width, height)
    {
        SetEntitySprites("Assets/enemy/Enemy_Fly.png", 4, 1, 0);
        //SetScaleXY(3,3);
        this.x = x;
        this.y = y;
        this.line = line;
        spellDisplay = new Pivot();
        AddChild(spellDisplay);
        spellDisplay.y = -height / 2 - 10;

    }

    public void Death()
    {
        Console.WriteLine("enemy died");
        collection[line].Remove(this);
        LateDestroy();
        if (!isDead)
            player.score++;
        isDead = true;
    }

    public void UpdateEnemy()
    {
        Move(velocity.x * Time.deltaTime/1000, velocity.y * Time.deltaTime/1000);
        Animate();
    }

    public static void SpawnEnemy()
    {
        int line = Utils.Random(0, 3);
        Enemy enemy = new Enemy(64, 64, MyGame.self.width / 2 - 200, linesY[line], line);
        collection[line].Add(enemy);
        MyGame.self.lineLayers[line].AddChild(enemy);
        enemy.GenerateSpells();
        //MagicShape.CastSpell += enemy.Damage;
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
        }
        else
        {
            RemoveSpell();
            Death();
        }
    }
    public bool IsEnemyInFront()
    {
        return x - player.x < player.reachDistance;
    }

    public static void UpdateAll()
    {
        for (int i = 0; i < 3; i++)
            foreach (var enemy in collection[i])
            {
                enemy.UpdateEnemy();

                //Move to enemy manager
                if (enemy.x < MyGame.self.crossLine)
                {
                    enemy.Death();
                    break;
                }

                if (Math.Abs(enemy.x - player.x) < 10f && enemy.line == player.line)
                {
                    enemy.Death();
                    player._HP--;
                    player.hpDecreased = true;
                    Console.WriteLine(player._HP);
                    break;
                }

                player.hpDecreased = false;
            }
    }

    public void GenerateSpells()
    {
        shapes = new List<Shape>();
        int shapeCount = Utils.Random(1, 4);

        for (int i = 0; i < shapeCount; i++)
        {
            Shape sh = (Shape)Utils.Random(0, 4);
            shapes.Add(sh);

            Sprite symbol = new Sprite(MagicShape.spellSprite[(int)sh]);
            symbol.SetOrigin(width / 2, height / 2);
            spellDisplay.AddChild(symbol);
            symbol.x = i * 40 - (shapeCount - 1) * 20;
            symbol.y = 0;
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

                player.score++;
                foreach (GameObject go in spells)
                {
                    go.Move(-20, 0);
                }
            }
        }
    }
}