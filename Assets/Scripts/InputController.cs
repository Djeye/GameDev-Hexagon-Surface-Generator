using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }

    public Vector2 MoveVector { get; private set; }
    public bool IsMoving => MoveVector.magnitude >= 0.1f;


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
    }
    

    private void GetInputAxis()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        var verticalAxis = Input.GetAxis("Vertical");

        MoveVector = new Vector2(horizontalAxis, verticalAxis).normalized;
    }
}