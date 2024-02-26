using GXPEngine;
using GXPEngine.Core;

public class Entity : GameObject
{
    protected bool moving = false;
    public float width, height;

    protected static int[] linesY = { -300, 0, 300 };

    protected AnimationSprite[] states = new AnimationSprite[4];
    /*
     * Index 0: Idle, 
     * Index 1: Run, 
     * Index 2: Attack, 
     * Index 3: Death
     */

    private float[] AnimationSpeed = new float[]
    {
        10f,10f,10f,10f
    };

    // Represents the state of the entity (Idle, Run, Attack)
    public enum EntityState
    {
        Idle,    // The entity is idle
        Run,     // The entity is running
        Attack,  // The entity is attacking
        Death    // The entity is dying
    }

    public EntityState currentState;

    public Entity(float width, float height) : base(true)
    {
        this.width = width;
        this.height = height;
        
        SetEntityState(EntityState.Idle);
    }

    // Sets the sprites for different entity states

    public void SetEntitySprites(string path, int cols, int rows, int id)
    {
        states[id] = new AnimationSprite(path, cols, rows, -1, false, false);
        states[id].SetOrigin(width/2, height/2);
        AddChild(states[id]);
    }

    // Sets the current state of the entity
    public void SetEntityState(EntityState newState)
    {

        if (states[(int)newState] == null)
        {
            // Handle the null sprites
            return;
        }

        foreach (AnimationSprite state in states)
        {
            state.visible = false;
        }
        states[(int)newState].visible = true;

        if (currentState == newState)
            return;

        states[(int)newState].SetFrame(0);
        states[(int)newState].SetCycle(0, states[(int)newState].frameCount);

        currentState = newState;

    }
    public void Animate()
    {
        states[(int)currentState].Animate(AnimationSpeed[(int)currentState] * Time.deltaTime / 1000);
    }

    // Gets the current state of the entity
    public EntityState GetCurrentState()
    {
        return currentState;
    }
}