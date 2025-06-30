using System;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonPlayerController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;

        [SerializeField] private float _baseMoveSpeed = 2f;
        [SerializeField] private float _walkSpeedModifier = 1f;
        [SerializeField] private float _sprintSpeedModifier = 4f;
        [SerializeField] private float _speedChangeRate = 10f;

        [SerializeField] private float _baseJumpForce = 4f;
        [SerializeField] private float _walkJumpForceModifier = 1f;
        [SerializeField] private float _sprintJumpForceModifier = 1.4f;

        [SerializeField] private float _damping = 0.5f;
        [SerializeField] private float _onLandDamping = 0.3f;

        [SerializeField] private bool _strafe = false;

        [SerializeField] private LayerMask _groundLayers = 1;

        [SerializeField] private float _gravityMultiplier = 1f;
        private CharacterController _controller;

        private float _timeLastGrounded;

        private Vector2 _moveInput;
        private Vector3 _lastInput;

        private Vector3 _currentHorizontalVelocity;
        private float _currentVerticalVelocity;

        private bool _isSprinting = false;
        private bool _isJumping = false;
        private bool _jump = false;
        private bool _sprint = false;

        private float _animationBlend;
        private Animator _animator;
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Transform _cameraTransform;

        private const float GRAVITY = -9.81f;
        private const float EPSILON = 0.0001f;
        private const float RAY_MAX_DISTANCE = 1f;
        private const float RAY_HIT_DISTANCE_THRESHOLD = 0.01f;
        private const float DELAY_BEFORE_INFERRING_JUMP = 2.3f;
        private const float LOG_NEGLIGIBLE_RESIDUAL = -4.605170186f; // == math.Log(kNegligibleResidual=0.01f);

        public Action PreUpdate;
        public Action PostUpdate;
        private bool _grounded;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _controller = GetComponent<CharacterController>();

            _cameraTransform = Camera.main.transform;

            _inputReader.Initialize();
            _inputReader.EnablePlayerActions();

            AssignAnimationIDs();

        }

        private void OnEnable()
        {
            if (_inputReader != null)
            {
                _inputReader.MovePerformedAction += OnMovePerformed;
                _inputReader.MoveCanceledAction += OnMoveCanceled;
                _inputReader.RunPerformedAction += OnRunPerformed;
                _inputReader.RunCanceledAction += OnRunCanceled;
                _inputReader.JumpPerformedAction += OnJumpPerformed;
                _inputReader.JumpCanceledAction += OnJumpCanceled;
            }
        }

        private void OnDisable()
        {
            if (_inputReader != null)
            {
                _inputReader.MovePerformedAction -= OnMovePerformed;
                _inputReader.MoveCanceledAction -= OnMoveCanceled;
                _inputReader.RunPerformedAction -= OnRunPerformed;
                _inputReader.RunCanceledAction -= OnRunCanceled;
                _inputReader.JumpPerformedAction -= OnJumpPerformed;
                _inputReader.JumpCanceledAction -= OnJumpCanceled;
            }
        }

        private void Update()
        {
            PreUpdate?.Invoke();

            HandleGravity();

            // Process Jump and gravity
            bool justLanded = ProcessJump();

            // Get input and apply to movement frame
            Vector3 rawInput = new(_moveInput.x, 0, _moveInput.y);

            Vector3 cameraForward = _cameraTransform.forward;

            cameraForward.y = 0;

            _lastInput = Quaternion.LookRotation(cameraForward.normalized, Vector3.up) * rawInput;

            if (_lastInput.sqrMagnitude > 1)
                _lastInput.Normalize();

            float inputMagnitude = _lastInput.magnitude;
            float targetSpeed = _baseMoveSpeed * (_isSprinting ? _sprintSpeedModifier : _walkSpeedModifier) * inputMagnitude;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, 1f);

            // Compute the new velocity and move the player, but only if not mid-jump
            if (!_isJumping)
            {
                _isSprinting = _sprint;
                Vector3 desiredVelocity = _baseMoveSpeed * (_isSprinting ? _sprintSpeedModifier : _walkSpeedModifier) * _lastInput;
                float damping = justLanded ? 0 : _damping;

                if (Vector3.Angle(_currentHorizontalVelocity, desiredVelocity) < 100)
                    _currentHorizontalVelocity = Vector3.Slerp(
                        _currentHorizontalVelocity, desiredVelocity,
                        Damp(1, damping, Time.deltaTime));
                else
                    _currentHorizontalVelocity += Damp(
                        desiredVelocity - _currentHorizontalVelocity, damping, Time.deltaTime);
            }

            Vector3 targetMotion = _currentVerticalVelocity * Vector3.up + _currentHorizontalVelocity;

            // Apply the position change
            _controller.Move(targetMotion * Time.deltaTime);

            // If not strafing, rotate the player to face movement direction
            if (!_strafe && _currentHorizontalVelocity.sqrMagnitude > 0.001f)
            {
                Quaternion currentRotation = transform.rotation;
                Quaternion desiredRotation = Quaternion.LookRotation(_currentHorizontalVelocity, Vector3.up);
                float damping = justLanded ? _onLandDamping : _damping;
                transform.rotation = Quaternion.Slerp(currentRotation, desiredRotation, Damp(1, damping, Time.deltaTime));
            }

            PostUpdate?.Invoke();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        #region Input Methods
        private void OnJumpCanceled() => _jump = false;
        private void OnJumpPerformed() => _jump = true;
        private void OnRunPerformed() => _sprint = true;
        private void OnRunCanceled() => _sprint = false;
        private void OnMoveCanceled() => _moveInput = Vector2.zero;
        private void OnMovePerformed(Vector2 moveInput) => _moveInput = moveInput;
        #endregion

        private void HandleGravity()
        {
            GroundedCheck();

            if (_grounded)
            {
                _timeLastGrounded = Time.time;
                _currentVerticalVelocity = 0;
            }
            else
            {
                _currentVerticalVelocity += GRAVITY * _gravityMultiplier * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,
                    RAY_MAX_DISTANCE, _groundLayers, QueryTriggerInteraction.Ignore))
                _grounded = hit.distance < RAY_HIT_DISTANCE_THRESHOLD;

            _animator.SetBool(_animIDGrounded, _grounded);
        }

        private bool ProcessJump()
        {
            bool justLanded = false;

            float now = Time.time;

            if (!_isJumping)
            {
                // Process jump command
                if (_grounded && _jump)
                {
                    _isJumping = true;
                    _currentVerticalVelocity = _baseJumpForce * (_isSprinting ? _sprintJumpForceModifier : _walkJumpForceModifier);

                    _animator.SetBool(_animIDJump, true);
                }

                // If we are falling, assume the jump pose
                if (!_grounded && now - _timeLastGrounded > DELAY_BEFORE_INFERRING_JUMP)
                {
                    _isJumping = true;
                    _animator.SetBool(_animIDFreeFall, true);
                }

                if (_isJumping)
                    _grounded = false;
            }

            if (_grounded)
            {
                // If we were jumping, complete the jump
                if (_isJumping)
                {
                    _isJumping = false;
                    justLanded = true;
                }

                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            return justLanded;
        }

        private static float Damp(float initial, float dampTime, float deltaTime)
        {
            if (dampTime < EPSILON || Mathf.Abs(initial) < EPSILON)
                return initial;

            if (deltaTime < EPSILON)
                return 0;

            return initial * (1 - Mathf.Exp(LOG_NEGLIGIBLE_RESIDUAL * deltaTime / dampTime));
        }

        private static Vector3 Damp(Vector3 initial, float dampTime, float deltaTime)
        {
            for (int i = 0; i < 3; ++i)
                initial[i] = Damp(initial[i], dampTime, deltaTime);

            return initial;
        }
    }
}