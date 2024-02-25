using GXPEngine;
using System.Collections.Generic;

public class ParallaxBackground : GameObject
{
    private class ParallaxLayer
    {
        public float speed;
        public List<ParallaxSprite> sprites;

        public ParallaxLayer(float pSpeed)
        {
            speed = pSpeed;
            sprites = new List<ParallaxSprite>();
        }
    }

    private class ParallaxSprite
    {
        public Sprite sprite;

        public ParallaxSprite(string imagePath, float pScale, float pX, float pY)
        {
            sprite = new Sprite(imagePath, false, false);
            sprite.SetOrigin(pX, pY);
            sprite.SetScaleXY(pScale);
        }
    }

    private List<ParallaxLayer> layers;
    private Player _playerRef;

    public ParallaxBackground() : base(false)
    {
        layers = new List<ParallaxLayer>();

       //layer[0]
        layers.Add(new ParallaxLayer(0.02f));
        layers[0].sprites.Add(new ParallaxSprite("sprites/backcolor.png", 0.5f, 0, 0));
        layers[0].sprites.Add(new ParallaxSprite("sprites/backcolor.png", 0.5f, layers[0].sprites[layers[0].sprites.Count - 1].sprite.width * 2, 0));

        //layer[1]
        layers.Add(new ParallaxLayer(0.06f));
        layers[1].sprites.Add(new ParallaxSprite("sprites/background.png", 0.5f, 0, 0));
        layers[1].sprites.Add(new ParallaxSprite("sprites/background.png", 0.5f, layers[1].sprites[layers[1].sprites.Count - 1].sprite.width * 2, 0));

        //layer[2]
        layers.Add(new ParallaxLayer(0.1f));
        layers[2].sprites.Add(new ParallaxSprite("sprites/midground.png", 0.5f, 0, 0));
        layers[2].sprites.Add(new ParallaxSprite("sprites/midground.png", 0.5f, layers[2].sprites[layers[2].sprites.Count - 1].sprite.width * 2, 0));

        //layer[3]
        layers.Add(new ParallaxLayer(0.14f));
        layers[3].sprites.Add(new ParallaxSprite("sprites/foreground.png", 0.5f, 0, 0));
        layers[3].sprites.Add(new ParallaxSprite("sprites/foreground.png", 0.5f, layers[3].sprites[layers[3].sprites.Count - 1].sprite.width * 2, 0));


        foreach (ParallaxLayer layer in layers)
        {
            foreach (ParallaxSprite _sprite in layer.sprites)
            {
                AddChild(_sprite.sprite);
            }   
        }
        
    }

    public void UpdateParallax(Player _player)
    {
        _playerRef = _player;
        for (int i = 0; i < layers.Count; i++)
        {
            float parallaxOffset = _playerRef.x * layers[i].speed;

            foreach (ParallaxSprite _sprite in layers[i].sprites)
            {
                _sprite.sprite.x = -parallaxOffset;
            }
        }
    }

    public void UpdateParallaxMouse()
    {
        float mouseX = Input.mouseX;

        for (int i = 0; i < layers.Count; i++)
        {
            float parallaxOffset = mouseX * layers[i].speed;

            foreach (ParallaxSprite _sprite in layers[i].sprites)
            {
                _sprite.sprite.x = -parallaxOffset;
            }
        }
    }

}