﻿using MeshCreation;
using ProceduralAnimation;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Mech mech;
        [SerializeField] private bool applyGravity=true;

        [Space]
        [SerializeField] private float speed;
        [SerializeField] private float fallSpeed;
        [SerializeField] private float turnSmoothTime;

        private CharacterController _controller;
        private HexagonInteractor _hexagonInteractor;

        private Transform _transform;
        private Camera _cam;

        private float _angleVelocity;


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

            mech.InitLegs(_transform);
            _hexagonInteractor = new HexagonInteractor(_transform, _cam);

            StartCoroutine(_hexagonInteractor.SlowUpdate());
        }

        private void Update()
        {
            Moving(out Vector3 moveDirection);
            mech.UpdateLegsMovement(moveDirection);
            _hexagonInteractor.Update();
        }


        private void Moving(out Vector3 moveDirection)
        {
            if (applyGravity)
            {
                ApplyGravity();
            }

            if (!InputSystem.Instance.IsMoving)
            {
                moveDirection = Vector3.zero;
                return;
            }

            moveDirection = CalculateMoveDirection(out float angle);

            _transform.rotation = Quaternion.Euler(0f, angle, 0f);
            _controller.Move(speed * Time.deltaTime * moveDirection);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded)
            {
                return;
            }

            _controller.Move(fallSpeed * Time.deltaTime * Physics.gravity);
        }

        private Vector3 CalculateMoveDirection(out float angle)
        {
            Vector2 moveVector = InputSystem.Instance.MoveVector;

            float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _angleVelocity, turnSmoothTime);

            return (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;
        }
    }
}