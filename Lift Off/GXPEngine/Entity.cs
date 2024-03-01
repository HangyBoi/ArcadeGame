using GXPEngine;
using GXPEngine.Core;
using System;
using System.Collections.Generic;

public class Entity : GameObject
{
    protected bool moving = false;
    public float width, height;

    public static int[] linesY = { -100, 150, 400 };
    public static int[] linesZ = { 2, 0, -2 };

    protected List<AnimationSprite> states = new List<AnimationSprite>();
    /*
     * Index 0: Idle, 
     * Index 1: Run, 
     * Index 2: Attack, 
     * Index 3: Death
     */

    private List<float> AnimationSpeed = new List<float>();

    // Represents the state of the entity (Idle, Run, Attack)
    public enum EntityState
    {
        Idle,    // The entity is idle
        Run,     // The entity is running
        Attack,  // The entity is attacking
        Death    // The entity is dying
    }

    public int currentState;
    private int bufferedState = 0;
    private int bufferedFrame = 0;
    private bool animateOnce = false;

    public Entity(float width, float height) : base(true)
    {
        this.width = width;
        this.height = height;
        
        SetEntityState(0);
    }

    // Sets the sprites for different entity states

    public void SetEntitySprites(string path, int cols, int rows, int id, float speed = 15f)
    {
        AnimationSprite anim = new AnimationSprite(path, cols, rows, -1, false, false);
        AnimationSpeed.Add(speed);
        anim.SetOrigin(anim.width / 2, anim.height /2);
        AddChild(anim);
        anim.SetXY(0, 0);
        states.Insert(id,anim);
        anim.SetFrame(0);
        anim.SetCycle(0, anim.frameCount);
    }

    // Sets the current state of the entity
    public void SetEntityState(int newState, int startFrame = 0)
    {

        if (states.Count <= newState)
        {
            // Handle the null sprites
            return;
        }

        foreach (AnimationSprite state in states)
        {
            state.visible = false;
        }
        states[newState].visible = true;

        if (currentState == newState)
            return;


        currentState = newState;
        states[newState].SetFrame(startFrame);

    }
    public void Animate()
    {
        states[currentState].Animate(AnimationSpeed[currentState] * Time.deltaTime / 1000);
        if (animateOnce && states[currentState].currentFrame == states[currentState].frameCount - 1)
        {
            Console.WriteLine(states[currentState].currentFrame);
            animateOnce = false;
            SetEntityState(bufferedState);
            states[currentState].currentFrame = bufferedFrame;
        }
    }

    // Gets the current state of the entity
    public int GetCurrentState()
    {
        return currentState;
    }
    public void AnimateOnce(int state, int returnState, int returnFrame = 0)
    {
        bufferedState = returnState;
        bufferedFrame = returnFrame;
        SetEntityState(state);
        animateOnce = true;

    }
}