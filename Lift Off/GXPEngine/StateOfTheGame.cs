using System;
using GXPEngine;
using GXPEngine.Core;

public class StateOfTheGame : GameObject
{
    public enum GameState
    {
        Menu,
        PlayingLevel,
        GameOver
    }

    public GameState currentState;

    public GameState CurrentGameState
    {
        get { return currentState; }
    }

    public StateOfTheGame()
    {
        currentState = GameState.Menu;
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;
        Console.WriteLine("Change game state to: " + currentState);
    }
}


