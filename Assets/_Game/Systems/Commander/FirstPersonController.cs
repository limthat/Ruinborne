using UnityEngine;
using UnityEngine.InputSystem;
using Ruinborne.Core;

namespace Ruinborne.Systems.Commander
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("이동 설정")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.2f;

        [Header("카메라 설정")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float verticalClampAngle = 80f;

        private CharacterController _controller;
        private Vector3 _velocity;
        private float _verticalRotation;
        private bool _isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleMouseLook();
            HandleMovement();
        }

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * 0.1f * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseDelta.x);

            _verticalRotation -= mouseDelta.y;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -verticalClampAngle, verticalClampAngle);
            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }

        private void HandleMovement()
        {
            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;

            var keyboard = Keyboard.current;
            Vector2 moveInput = Vector2.zero;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveInput.x = -1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveInput.x = 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveInput.y = -1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveInput.y = 1f;

            if (moveInput.magnitude > 1f) moveInput.Normalize();

            bool isSprinting = keyboard.leftShiftKey.isPressed;
            float speed = isSprinting ? sprintSpeed : moveSpeed;

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            _controller.Move(move * speed * Time.deltaTime);

            if (keyboard.spaceKey.wasPressedThisFrame && _isGrounded)
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        public void SetCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
