using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float turnSmoothTime;

        private Transform _cam;
        private CharacterController _controller;
        private Transform _transform;

        private float _angleVelocity;
        
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _transform = GetComponent<Transform>();
            _cam = Camera.main.transform;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            var horizontalAxis = Input.GetAxis("Horizontal");
            var verticalAxis = Input.GetAxis("Vertical");
            
            Vector3 moveVector = new Vector3(horizontalAxis, 0, verticalAxis).normalized;

            if (moveVector.magnitude < 0.1f)
            {
                return;
            }
            
            var targetAngle = Mathf.Atan2(moveVector.x, moveVector.z) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
            
            var angle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _angleVelocity,
                turnSmoothTime);
            
            _transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            var moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            if (!_controller.isGrounded)
            {
                moveDirection += Physics.gravity;
            }

            var moveSpeed = Time.deltaTime * speed;
            _controller.Move(moveSpeed * moveDirection.normalized);
        }
    }
}