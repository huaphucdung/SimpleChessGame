using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public static InputController input { get; private set; }
    public static InputController.PlayerActions playerInput { get; private set; }
    public static InputController.UIActions uiInput { get; private set; }

    public static void Initialize()
    {
        if (input == null)
        {
            input = new InputController();
            playerInput = input.Player;
            uiInput = input.UI;
        }
    }

    public static void EnableInput()
    {
        input.Enable();
    }

    public static void DisableInput()
    {
        input.Disable();
    }

    public static void ActiveInputClicked(bool value)
    {
        if(value)
        {
            playerInput.Click.Enable();
        }
        else
        {
            playerInput.Click.Disable();
        }
    }

    public static void ActiveInputRotation(bool value)
    {
        if (value)
        {
            playerInput.Look.Enable();
        }
        else
        {
            playerInput.Look.Disable();
        }
    }
}
