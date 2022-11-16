using UnityEngine;

namespace ProceduralAnimation
{
    public class LegTarget : MonoBehaviour
    {
        [SerializeField] private Transform raycastPoint;

        public bool IsMoving => _movement != null;
        private bool IsStepAllowed => Vector3.Distance(Position, RayPosition) > _legInfo.stepLength;
        public bool IsPossibleToMove => IsMoving || IsStepAllowed;


        private Vector3 Position { get; set; }
        private Vector3 RayPosition => _hit.point;
        private Vector3 ParentUp => _parentTransform.up;


        private Mech.LegInfo _legInfo;

        private Transform _legTransform;
        private Transform _parentTransform;

        private Movement? _movement;
        private RaycastHit _hit;

        private Vector3 _moveDirection;


        private void Awake()
        {
            _legTransform = transform;
        }

        private void Update()
        {
            ApplyRayCast();
            MoveLeg();
        }


        public void Init(Mech.LegInfo legInfo, Transform parentTransform)
        {
            _legInfo = legInfo;
            _parentTransform = parentTransform;
        }

        private void ApplyRayCast()
        {
            Ray ray = new Ray(raycastPoint.position + _moveDirection, -ParentUp);
            Physics.Raycast(ray, out _hit);
        }

        private void MoveLeg()
        {
            if (_movement != null)
            {
                Movement m = _movement.Value;

                m.progress = Mathf.Clamp01(m.progress + Time.deltaTime * _legInfo.stepSpeed);
                Position = m.Evaluate(ParentUp, _legInfo.stepHeight, _legInfo.stepCurve);
                _movement = m.progress < 1 ? m : null;
            }

            _legTransform.position = Position;
        }

        public void ApplyMoveDirection(Vector3 moveDirection)
        {
            _moveDirection = new Vector3(moveDirection.x * _legInfo.moveImpact, 0, moveDirection.z * _legInfo.moveImpact);
        }

        public void Move()
        {
            _movement = _movement != null
                ? new Movement(_movement.Value.progress, _movement.Value.fromPosition, RayPosition)
                : new Movement(0f, Position, RayPosition);
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