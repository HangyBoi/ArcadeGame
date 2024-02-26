using GXPEngine;

public class Button : Sprite
{
    protected Sprite[] buttons = new Sprite[3];

    public Button(string buttonImageFile) : base("Assets/transparent.png", false, true)
    {
        SetButtonSprite(buttonImageFile);
    }

    protected void SetButtonSprite(string buttonImageFile)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = new Sprite(buttonImageFile);
            buttons[i].SetOrigin(buttons[i].width / 2, buttons[i].height / 2);
            AddChild(buttons[i]);
        }
    }
}





