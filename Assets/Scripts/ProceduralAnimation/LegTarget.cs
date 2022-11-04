using UnityEngine;

namespace ProceduralAnimation
{
    public class LegTarget : MonoBehaviour
    {
        private float _stepSpeed, _stepHeight;
        private AnimationCurve _stepCurve;

        private Transform _transform;
        private Vector3 _position;
        private Movement? _movement;

        public Vector3 Position => _position;
        public bool IsMoving => _movement != null;


        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (_movement != null)
            {
                Movement m = _movement.Value;

                m.progress = Mathf.Clamp01(m.progress + Time.deltaTime * _stepSpeed);
                _position = m.Evaluate(Vector3.up, _stepHeight, _stepCurve);
                _movement = m.progress < 1 ? m : null;
            }

            _transform.position = _position;
        }


        public void Init(float stepSpeed, float stepHeight, AnimationCurve stepCurve)
        {
            _stepSpeed = stepSpeed;
            _stepHeight = stepHeight;

            _stepCurve = stepCurve;
        }

        public void MoveTo(Vector3 targetPosition)
        {
            _movement = _movement != null
                ? new Movement(_movement.Value.progress, _movement.Value.fromPosition, targetPosition)
                : new Movement(0f, _position, targetPosition);
        }

        private struct Movement
        {
            public float progress;
            public readonly Vector3 fromPosition;
            private readonly Vector3 _toPosition;
            
            public Movement(float progress, Vector3 fromPosition, Vector3 toPosition)
            {
                this.progress = progress;
                this.fromPosition = fromPosition;
                _toPosition = toPosition;
            }

            public Vector3 Evaluate(in Vector3 up, float stepheight, AnimationCurve stepCurve)
            {
                return Vector3.Lerp(fromPosition, _toPosition, progress) +
                       up * stepCurve.Evaluate(progress) * stepheight;
            }
        }
    }
}