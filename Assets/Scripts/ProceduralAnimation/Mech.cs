using System;
using UnityEngine;

namespace ProceduralAnimation
{
    [RequireComponent(typeof(CharacterController))]
    public class Mech : MonoBehaviour
    {
        [Header("Legs Data")]
        [SerializeField] private LegData[] legs;

        [Space]
        [Header("Player parameters")]
        [SerializeField] private float speed;
        [SerializeField] private float turnSmoothTime;

        [Space]
        [Header("Legs parameters")]
        [SerializeField] private float stepSpeed = 0.5f;
        [SerializeField] private float stepLength = 0.75f;
        [SerializeField] private float stepHeight = 0.25f;
        [SerializeField] private float stepMoveImpact = 0.5f;
        [SerializeField] private AnimationCurve stepCurve;

        [Serializable]
        private struct LegData
        {
            public LegTarget leg;
            public LegRaycast raycast;

            public void Init(float stepSpeed, float stepHeight, AnimationCurve stepCurve, Transform parentTransform, float moveImpact)
            {
                leg.Init(stepSpeed, stepHeight, stepCurve);
                raycast.Init(parentTransform, moveImpact);
            }
        }

        private CharacterController _controller;
        private Transform _transform;
        private Transform _cam;

        private float _angleVelocity;
        private Vector3 _moveDirection;


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

            InitLegs();
        }

        private void Update()
        {
            Moving();
            MoveLegs();
        }


        private void InitLegs()
        {
            foreach (var legData in legs)
            {
                legData.Init(stepSpeed, stepHeight, stepCurve, transform, stepMoveImpact);
            }
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
            _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            ApplyGravity();
            ApplyMovement(angle, _moveDirection);
        }

        private void MoveLegs()
        {
            for (int index = 0; index < legs.Length; index++)
            {
                var legData = legs[index];
                legData.raycast.ApplyMoveDirection(_moveDirection);
                
                if (!CanMove(index))
                {
                    continue;
                }

                if (!legData.leg.IsMoving &&
                    !(Vector3.Distance(legData.leg.Position, legData.raycast.Position) > stepLength))
                {
                    continue;
                }

                legData.leg.MoveTo(legData.raycast.Position);
            }
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded)
            {
                return;
            }

            _moveDirection += Physics.gravity;
        }

        private void ApplyMovement(float angle, Vector3 moveDirection)
        {
            _transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(Time.deltaTime * speed * moveDirection.normalized);
        }

        private bool CanMove(int legIndex)
        {
            int legsCount = legs.Length;
            int prevLegIndex = (legIndex + legsCount - 1) % legsCount;
            int nextLegIndex = (legIndex + 1) % legsCount;

            LegData prevLeg = legs[prevLegIndex];
            LegData nextLeg = legs[nextLegIndex];

            return !prevLeg.leg.IsMoving && !nextLeg.leg.IsMoving;
        }
    }
}