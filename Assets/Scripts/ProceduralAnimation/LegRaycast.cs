using UnityEngine;

namespace ProceduralAnimation
{
    public class LegRaycast : MonoBehaviour
    {
        private RaycastHit _hit;
        private Transform _parentTransform;
        private Transform _transform;

        public Vector3 Position => _hit.point;
        public Vector3 Normal => _hit.normal;

        private void Awake()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            Ray ray = new Ray(_transform.position, -_parentTransform.up);
            Physics.Raycast(ray, out _hit);
        }

        public void Init(Transform parentTransform)
        {
            _parentTransform = parentTransform;
        }
    }
}