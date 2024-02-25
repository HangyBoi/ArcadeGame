using GXPEngine;
using GXPEngine.Core;

public class Entity : GameObject
{
    protected bool moving = false;

    protected int[] linesY = { 100, 400, 700 };

    protected AnimationSprite[] states = new AnimationSprite[4]; // Index 0: Idle, Index 1: Run, Index 2: Attack, Index 3: Death

    private const float IdleAnimationSpeed = 0.07f;
    private const float RunAnimationSpeed = 0.18f;
    private const float AttackAnimationSpeed = 0.30f;
    private const float DeathAnimationSpeed = 0.25f;

    // Represents the state of the entity (Idle, Run, Attack)
    public enum EntityState
    {
        Idle,    // The entity is idle
        Run,     // The entity is running
        Attack,  // The entity is attacking
        Death    // The entity is dying
    }

    public EntityState currentState;

    public Entity() : base(true)
    {
        currentState = EntityState.Idle;
        SetEntityState(currentState);
    }

    // Sets the sprites for different entity states
    public void SetEntityIdleSprites(string idlePath, int colsIdle, int rowsIdle)
    {
        states[(int)EntityState.Idle] = new AnimationSprite(idlePath, colsIdle, rowsIdle, -1, false, false);
        AddChild(states[(int)EntityState.Idle]);
    }

    public void SetEntityRunSprites(string runPath, int colsRun, int rowsRun)
    {
        states[(int)EntityState.Run] = new AnimationSprite(runPath, colsRun, rowsRun, -1, false, false);
        AddChild(states[(int)EntityState.Run]);
    }

    public void SetEntityAttackSprites(string attackPath, int colsAttack, int rowsAttack)
    {
        states[(int)EntityState.Attack] = new AnimationSprite(attackPath, colsAttack, rowsAttack, -1, false, false);
        AddChild(states[(int)EntityState.Attack]);
    }

    public void SetEntityDeathSprites(string deathPath, int colsDeath, int rowsDeath)
    {
        states[(int)EntityState.Death] = new AnimationSprite(deathPath, colsDeath, rowsDeath, -1, false, false);
        AddChild(states[(int)EntityState.Death]);
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

        //switch between states 
        //making a loop through the array to turn off not necessary states 
        //switch between states 
        switch (newState)
        {
            case EntityState.Idle:
                states[(int)EntityState.Idle].visible = true;
                states[(int)EntityState.Idle].SetCycle(0, states[(int)EntityState.Idle].frameCount);
                states[(int)EntityState.Idle].Animate(IdleAnimationSpeed);
                break;

            case EntityState.Run:
                states[(int)EntityState.Run].visible = true;
                states[(int)EntityState.Run].SetCycle(0, states[(int)EntityState.Run].frameCount);
                states[(int)EntityState.Run].Animate(RunAnimationSpeed);
                break;

            case EntityState.Attack:
                states[(int)EntityState.Attack].visible = true;
                states[(int)EntityState.Attack].SetCycle(0, states[(int)EntityState.Attack].frameCount);
                states[(int)EntityState.Attack].Animate(AttackAnimationSpeed);
                break;
            case EntityState.Death:
                states[(int)EntityState.Death].visible = true;
                states[(int)EntityState.Death].SetCycle(0, states[(int)EntityState.Death].frameCount);
                states[(int)EntityState.Death].Animate(DeathAnimationSpeed);
                break;
        }

        currentState = newState;

    }

    // Gets the current state of the entity
    public EntityState GetCurrentState()
    {
        return currentState;
    }

}