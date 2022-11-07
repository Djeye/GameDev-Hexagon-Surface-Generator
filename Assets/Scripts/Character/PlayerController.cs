using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float turnSmoothTime;

        private CharacterController _controller;
        private Transform _transform;
        private Transform _cam;

        private float _angleVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _transform = GetComponent<Transform>();

            if (Camera.main != null)
            {
                _cam = Camera.main.transform;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Moving();
        }


        private void Moving()
        {
            if (!InputController.Instance.IsMoving)
            {
                return;
            }

            Vector2 moveVector = InputController.Instance.MoveVector;
            float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _angleVelocity,
                turnSmoothTime);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            ApplyGravity(ref moveDirection);
            ApplyMovement(angle, moveDirection);
        }

        private void ApplyGravity(ref Vector3 moveDirection)
        {
            if (_controller.isGrounded)
            {
                return;
            }

            moveDirection += Physics.gravity;
        }

        private void ApplyMovement(float angle, Vector3 moveDirection)
        {
            _transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(Time.deltaTime * speed * moveDirection.normalized);
        }
    }
}