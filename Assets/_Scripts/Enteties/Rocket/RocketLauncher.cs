using MeshCreation;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [SerializeField] private RocketMovement rocketPrefab;
    [SerializeField] private RocketTarget rocketTargetPrefab;

    private HexagonInteractor _hexagonInteractor;


    public void Init(HexagonInteractor hexagonInteractor)
    {
        _hexagonInteractor = hexagonInteractor;
    }

    public void LaunchRocket(Vector3 position)
    {
        RocketMovement rocket = Instantiate(rocketPrefab, transform.position + transform.right * 2,
            Quaternion.LookRotation(Vector3.up * 0.5f + transform.forward));
        
        rocket.Init(_hexagonInteractor, position);

        RocketMovement rocket2 = Instantiate(rocketPrefab, transform.position - transform.right * 2,
            Quaternion.LookRotation(Vector3.up * 0.5f + transform.forward));
        RocketTarget rocketTarget = Instantiate(rocketTargetPrefab, position, Quaternion.identity);
        rocket2.Init(_hexagonInteractor, rocketTarget.transform, rocketTarget.Rb);
    }
}