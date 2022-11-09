using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static readonly Vector2 SCREEN_CENTRE = new Vector2(0.5f, 0.5f);

    public Vector2 MoveVector { get; private set; }
    public readonly Dictionary<InputType, bool> buttonPressed = new Dictionary<InputType, bool>()
    {
        {InputType.MouseLeft, false},
        {InputType.MouseRight, false},
    };

    public bool IsMoving => MoveVector.magnitude >= 0.1f;
    public bool IsMouseButtonPressed => buttonPressed.Values.Any(pressed => pressed);
    
    public static InputController Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        GetInputAxis();
        GetMouseInput();
    }


    private void GetInputAxis()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        var verticalAxis = Input.GetAxis("Vertical");

        MoveVector = new Vector2(horizontalAxis, verticalAxis).normalized;
    }

    private void GetMouseInput()
    {
        buttonPressed[InputType.MouseLeft] = Input.GetMouseButtonDown(0);
        buttonPressed[InputType.MouseRight] = Input.GetMouseButtonDown(1);
    }

    public enum InputType : byte
    {
        MouseLeft = 0,
        MouseRight = 1
    }
}