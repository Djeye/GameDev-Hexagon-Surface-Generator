using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class InputSystem : Singleton<InputSystem>
{
    public static readonly Vector2 SCREEN_CENTRE = new Vector2(0.5f, 0.5f);

    public Vector2 MoveVector { get; private set; }
    public readonly Dictionary<InputType, bool> buttonPressed = new Dictionary<InputType, bool>()
    {
        {InputType.MouseLeft, false},
        {InputType.MouseRight, false},
    };

    public bool IsMoving => MoveVector.magnitude >= 0.1f;
    public bool IsMouseButtonPressed => buttonPressed[InputType.MouseLeft] || buttonPressed[InputType.MouseRight];


    private void Update()
    {
        GetInputAxis();
        GetMouseInput();
        GetKeyboardInput();
    }


    private void GetInputAxis()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        MoveVector = new Vector2(horizontalAxis, verticalAxis).normalized;
    }

    private void GetMouseInput()
    {
        buttonPressed[InputType.MouseLeft] = Input.GetMouseButtonDown(0);
        buttonPressed[InputType.MouseRight] = Input.GetMouseButtonDown(1);
    }

    private void GetKeyboardInput()
    {
        buttonPressed[InputType.Shift] = Input.GetKeyDown(KeyCode.LeftShift);
        buttonPressed[InputType.Ctrl] = Input.GetKeyDown(KeyCode.LeftControl);
        buttonPressed[InputType.Space] = Input.GetKeyDown(KeyCode.Space);
        buttonPressed[InputType.Launch] = Input.GetKeyDown(KeyCode.E);
    }


    public enum InputType : byte
    {
        MouseLeft = 0,
        MouseRight = 1,
        Shift = 2,
        Ctrl = 3,
        Space = 4,
        Launch = 5
    }
}