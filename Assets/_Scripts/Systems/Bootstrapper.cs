using UnityEngine;

public static class Bootstrapper
{
    private const string SYSTEMS = "Systems";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load(SYSTEMS)));
    }
}