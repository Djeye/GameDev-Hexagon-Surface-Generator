using MeshCreation;
using ProceduralAnimation;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Mech mech;

        [Space]
        [SerializeField] private float speed;
        [SerializeField] private float turnSmoothTime;

        private CharacterController _controller;
        private HexInteractor _hexInteractor;

        private Transform _transform;
        private Camera _cam;

        private float _angleVelocity;
        private Vector3 _moveDirection;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _controller = GetComponent<CharacterController>();

            if (Camera.main != null)
            {
                _cam = Camera.main;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            _hexInteractor = new HexInteractor(_cam);
        }

        private void Update()
        {
            Moving();
            mech.UpdateLegs(Vector3.zero);
            _hexInteractor.Update();
        }

        private void Moving()
        {
            if (!InputSystem.Instance.IsMoving)
            {
                return;
            }

            Vector2 moveVector = InputSystem.Instance.MoveVector;
            float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _angleVelocity,
                turnSmoothTime);
            var moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // if (!_controller.isGrounded)
            // {
            //     moveDirection += Physics.gravity;
            // }

            _transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(Time.deltaTime * speed * moveDirection.normalized);
        }
    }
}