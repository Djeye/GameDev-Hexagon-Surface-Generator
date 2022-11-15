using UnityEngine;

namespace ProceduralAnimation
{
    public class LegRaycast : MonoBehaviour
    {
        private RaycastHit _hit;
        private Transform _parentTransform;
        private Transform _transform;

        private Vector3 _moveDirection;
        private float _moveImpact;

        public Vector3 Position => _hit.point;
        public Vector3 Normal => _hit.normal;


        private void Awake()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            ApplyRayCast();
        }

        public void Init(Transform parentTransform, float moveImpact)
        {
            _parentTransform = parentTransform;
            _moveImpact = moveImpact;
        }

        public void ApplyMoveDirection(Vector3 moveDirection)
        {
            _moveDirection = moveDirection;
        }

        private void ApplyRayCast()
        {
            Vector3 directionOffset = new Vector3(_moveDirection.x * _moveImpact, 0, _moveDirection.z * _moveImpact);
            Ray ray = new Ray(_transform.position + directionOffset, -_parentTransform.up);
            Physics.Raycast(ray, out _hit);
        }
    }
}