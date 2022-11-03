using UnityEngine;

namespace ProceduralAnimation
{
    public class LegTarget : MonoBehaviour
    {
        [SerializeField] private float stepSpeed = 0.5f;
        [SerializeField] private AnimationCurve stepCurve;

        private Transform _transform;
        private Vector3 _position;
        private Movement? _movement;

        public Vector3 Position => _position;
        public bool IsMoving => _movement != null;


        private struct Movement
        {
            public float progress;
            public Vector3 fromPosition;
            public Vector3 toPosition;

            public Vector3 Evaluate(in Vector3 up, AnimationCurve stepCurve)
            {
                return Vector3.Lerp(@fromPosition, toPosition, progress) + up * stepCurve.Evaluate(progress);
            }
        }

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (_movement != null)
            {
                var m = _movement.Value;
                m.progress = Mathf.Clamp01(m.progress + Time.deltaTime * stepSpeed);
                _position = m.Evaluate(Vector3.up, stepCurve);
                _movement = m.progress < 1 ? m : null;
            }

            _transform.position = _position;
        }

        public void MoveTo(Vector3 targetPosition)
        {
            if (_movement == null)
            {
                _movement = new Movement()
                {
                    progress = 0f,
                    fromPosition = _position,
                    toPosition = targetPosition
                };
            }
            else
            {
                var m = _movement.Value;
                _movement = new Movement()
                {
                    progress = m.progress,
                    fromPosition = m.fromPosition,
                    toPosition = targetPosition
                };
            }
        }
    }
}