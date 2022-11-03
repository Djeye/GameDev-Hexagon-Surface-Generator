using UnityEngine;

namespace ProceduralAnimation
{
    public class LegRaycast : MonoBehaviour
    {
        private RaycastHit _hit;
        private Transform _transform;

        public Vector3 Position => _hit.point;
        public Vector3 Normal => _hit.normal;


        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            var ray = new Ray(_transform.position, Vector3.down);
            Physics.Raycast(ray, out _hit);
        }
    }
}