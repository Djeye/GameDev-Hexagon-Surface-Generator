using MeshCreation;
using UnityEngine;

public class RocketMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("MOVEMENT")]
    [SerializeField] private float speed = 15;
    [SerializeField] private float rotateSpeed = 150;

    [Header("PREDICTION")]
    [SerializeField] private float minDistancePredict = 5;
    [SerializeField] private float maxDistancePredict = 100;
    [SerializeField] private float maxPredictionTime = 5;

    [Header("DEVIATION")]
    [SerializeField] private float deviationAmount = 50;
    [SerializeField] private float deviationSpeed = 2;

    private Vector3 _standardPrediction, _deviatedPrediction;
    private float _leadTimePercentage;
    
    private Transform _transform;
    private Rigidbody _rb;

    private Transform _targetTransform;
    private Rigidbody _targetRb;
    private Vector3 _targetPosition;

    private HexagonInteractor _hexInteractor;

    private FlyMode _mode;

    private enum FlyMode : byte
    {
        Static = 0,
        Moving = 1
    }

    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(HexagonInteractor hexagonInteractor, Transform target, Rigidbody targetRb)
    {
        _hexInteractor = hexagonInteractor;
        _targetTransform = target;
        _targetRb = targetRb;

        _mode = FlyMode.Moving;
    }

    public void Init(HexagonInteractor hexagonInteractor, Vector3 targetPosition)
    {
        _hexInteractor = hexagonInteractor;
        _targetPosition = targetPosition;

        _mode = FlyMode.Static;
    }

    private void FixedUpdate()
    {
        _rb.velocity = _transform.forward * speed;

        Vector3 targetPosition = _targetTransform == null ? _targetPosition : _targetTransform.position;

        float distance = Vector3.Distance(_transform.position, targetPosition);

        _leadTimePercentage = Mathf.InverseLerp(minDistancePredict, maxDistancePredict, distance);

        PredictMovement(targetPosition);

        AddDeviation();

        RotateRocket();
    }

    private void PredictMovement(Vector3 targetPosition)
    {
        _standardPrediction = targetPosition;
        if (_targetRb != null)
        {
            float predictionTime = Mathf.Lerp(0, maxPredictionTime, _leadTimePercentage);

            _standardPrediction += _targetRb.velocity * predictionTime;
        }
    }

    private void AddDeviation()
    {
        Vector3 deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed), 0, 0);

        Vector3 predictionOffset = _transform.TransformDirection(deviation) * deviationAmount * _leadTimePercentage;

        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        var heading = _deviatedPrediction - _transform.position;

        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(_transform.rotation, rotation, rotateSpeed * Time.deltaTime));
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out RocketLauncher _))
        {
            return;
        }

        if (explosionPrefab)
        {
            Instantiate(explosionPrefab, _transform.position, Quaternion.identity);
        }

        if (collision.transform.TryGetComponent(out IExplode ex))
        {
            ex.Explode();
        }

        if (collision.transform.TryGetComponent(out ChunkGenerator _))
        {
            _hexInteractor.HandleRocketExplosion(_transform.position);
        }

        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
    
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }
}