using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

public class Enemy : Entity
{
    public static Player _player;
    public static List<Enemy>[] collection = new List<Enemy>[]
        {
            new List<Enemy>(),
            new List<Enemy>(),
            new List<Enemy>()
        };

    protected float enemyX;
    protected float enemyY;
    public int line;
    public Pivot spellDisplay;

    public List<Shape> shapes = new List<Shape>();

    public Vector2 velovity = new Vector2(-150f, 0);

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
        _player.score++;
        collection[line].Remove(this);
        LateDestroy();
    }

    public void UpdateEnemy()
    {
        Move(velovity.x * Time.deltaTime / 1000, velovity.y * Time.deltaTime / 1000);
        Animate();
    }

    public static void SpawnEnemy()
    {
        int line = Utils.Random(0, 3);
        Enemy enemy = new Enemy(64, 64, MyGame.self.width / 2 - 200, linesY[line], line);
        collection[line].Add(enemy);

        MyGame.self.AddChild(enemy);
        enemy.GenerateSpells();
        MagicShape.CastSpell += enemy.Damage;
    }

    public void Damage(Shape shape)
    {
        if (shapes[0] == shape && _player.line == line)
        {
            if (shapes.Count > 1)
            {
                RemoveSpell();
            }
            else
            {
                Death();
            }
        }
    }
    public bool IsEnemyInFront()
    {
        return x - _player.x < _player.reachDistance;
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

                if (Math.Abs(enemy.x - _player.x) < 10f && enemy.line == _player.line)
                {
                    enemy.Death();
                    _player._HP--;
                    _player.hpDecreased = true;
                    Console.WriteLine(_player._HP);
                    break;
                }

                _player.hpDecreased = false;
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

        foreach (var sh in shapes)
        {
            Console.WriteLine(sh);
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
                _player.score++;
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
}