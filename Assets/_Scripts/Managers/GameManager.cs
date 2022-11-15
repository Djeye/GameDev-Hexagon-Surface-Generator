using System;
using MeshCreation;
using Utilities;

public class GameManager : StaticInstance<GameManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    
    void Start() => ChangeState(GameState.Starting);

    
    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.Starting:
                HandleStarting();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);
    }

    private void HandleStarting()
    {
        WorldGenerator.Instance.GenerateWorld();

        //ChangeState(GameState.Ending);
    }
}


[Serializable]
public enum GameState
{
    Starting = 0,
    Ending = 1,
}