using GXPEngine;
using System;
using System.Collections.Generic;

public class Menu : GameObject
{
    private Button _startButton;
    private Button _notButtonHeader; //FOR GAME NAME ON THE MAIN SCREEN
    private Button _quitButton;      //FOR THE QUIT BUTTON IF NEEDED
    private float buttonOffset = 100;
    private MyGame _mygame;
    bool _hasStarted;

    public Menu(MyGame mygame) : base(false)
    {
        _mygame = mygame;
        _hasStarted = false;
        //_notButtonHeader = new Button("Assets/backgrounds/darkforest.png");
        //_quitButton = new Button("Assets/UI/quitButP.png);
        _startButton = new Button("Assets/UI/playButY.png");
        AddChild(_startButton);
        //AddChild(_notButtonHeader);
        //AddChild(_quitButton);
        _startButton.SetXY(game.width / 2, game.height / 2);
    }

    void MenuUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOverButton(_startButton))
            {
                Console.WriteLine("bro you clicked");
                HideMenu();
                StartGame();
            }
            /*else if (IsMouseOverButton(_quitButton))
            {
                game.Destroy();
            }*/             //IF WE HAVE QUIT BUTTON
        }
    }

    bool IsMouseOverButton(Button button)
    {
        float buttonWidth = button.width / 2;
        float buttonHeight = button.height / 2;

        float mouseX = Input.mouseX;
        float mouseY = Input.mouseY;

        // Check if mouse coordinates are within the visible area of the button
        return mouseX > button.x - buttonWidth &&
               mouseX < button.x + buttonWidth &&
               mouseY > button.y - buttonHeight &&
               mouseY < button.y + buttonHeight;
    }

    void HideMenu()
    {
        List<GameObject> children = GetChildren();
        foreach (GameObject child in children)
        {
            child.Destroy();
        }
    }

    void StartGame()
    {
        if (_hasStarted == false)
        {
            //_mygame.StartLevel();
            _hasStarted = true;
        }
    }
}