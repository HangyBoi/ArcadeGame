using System;
using GXPEngine;
using GXPEngine.Core;

public static class StateOfTheGame
{
    public enum GameState
    {
        Game,
        GameOver,
        Typing
    }

    public static GameState currentState = GameState.Game;

    public static GameState CurrentGameState
    {
        get { return currentState; }
    }

    public static void SetGameState(GameState newState)
    {
        currentState = newState;
        Console.WriteLine("Change game state to: " + currentState);
    }
}


