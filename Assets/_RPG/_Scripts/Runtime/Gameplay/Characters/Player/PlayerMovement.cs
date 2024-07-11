using RPG.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        private static Transform _cam;
        public static Transform Cam
        {
            get
            {
                if (_cam == null)
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null) _cam = mainCamera.transform;
                }
                return _cam;
            }
        }

        private Animator _animator;
        private CharacterController _controller;
        private GameInputAction _inputActions;

        [Header("Sound Variables")]
        [SerializeField] private AudioSource _footstep;
        [SerializeField] private AudioClip _walkingClip;
        [SerializeField] private AudioClip _runningClip;
        [SerializeField] private AudioClip _jumpingClip;

        [Header("Slope Variables")]
        [Tooltip("Used to add downward force when on slope")]
        [SerializeField] private float _slopeForce = 1f;
        [SerializeField] private float _slopeForceRayLength = 0.5f;
        [SerializeField] private float _slidingSpeed = 10f;

        private RaycastHit _slopeHit;
        private bool _onShallowSlope, _onSteepSlope;

        [Space(5), Header("Speed Variables")]
        [Tooltip("To determine how fast player can move")]
        [SerializeField] private float _speed = 5f;
        private bool _canRun;
        private float _inputMagnitude;
        private Vector3 _inputVec;
        private float _turnSmoothVel;
        [SerializeField] private float _smoothTime = 0.15f;

        [Space(5), Header("Ground and Jump Variables")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _jumpHeight = 4f;
        private bool _isJumpPressed, _isJumping;

        [SerializeField] private float _groundedGravity = -0.14f;
        [SerializeField] private float _groundDist = 0.28f;
        private bool _isGrounded;

        private Vector3 _verticalVel;

        [Header("Jump Cooldown")]
        [Tooltip("Delays the jump action based on the cooldown")]
        [SerializeField] private float _cooldownTime = 2f;
        private float _timer = 0.0f;

        private int _isJumpingHash, _isFallingHash, _inputMagnitudeHash;

        private const string IS_JUMPING = "IsJumping";
        private const string IS_FALLING = "IsFalling";
        private const string INPUT_MAGNITUDE = "Input Magnitude";

        private void Awake()
        {
            InitializeComponents();
            InitializeAnimationHashes();
            InitializeInputActions();
        }

        private void OnEnable()
        {
            InputManager.Instance.PlayerActions.Move.started += Move_started;
            InputManager.Instance.PlayerActions.Move.canceled += Move_canceled;
            InputManager.Instance.PlayerActions.Run.started += Run_started;
            InputManager.Instance.PlayerActions.Run.canceled += Run_canceled;
        }

        private void OnDisable()
        {
            InputManager.Instance.PlayerActions.Move.started -= Move_started;
            InputManager.Instance.PlayerActions.Move.canceled -= Move_canceled;
            InputManager.Instance.PlayerActions.Run.started -= Run_started;
            InputManager.Instance.PlayerActions.Run.canceled -= Run_canceled;
        }

        private void Run_canceled(InputAction.CallbackContext context)
        {
            _footstep.Pause();
        }

        private void Run_started(InputAction.CallbackContext context)
        {
            if (_inputMagnitude > 0)
            {
                _footstep.clip = _runningClip;
                _footstep.Play();
            }
        }

        private void Move_canceled(InputAction.CallbackContext context)
        {
            _footstep.Pause();
        }

        private void Move_started(InputAction.CallbackContext context)
        {
            _footstep.clip = _walkingClip;
            _footstep.Play();
        }

        private void InitializeComponents()
        {
            _animator = GetComponentInChildren<Animator>();
            _controller = GetComponent<CharacterController>();
        }

        private void InitializeInputActions()
        {
            InputManager.Instance.PlayerActions.Jump.performed += ctx => _isJumpPressed = true;
            InputManager.Instance.PlayerActions.Jump.canceled += ctx => _isJumpPressed = false;
            InputManager.Instance.PlayerActions.Run.started += ctx => _canRun = true;
            InputManager.Instance.PlayerActions.Run.canceled += ctx => _canRun = false;
        }

        private void InitializeAnimationHashes()
        {
            _isJumpingHash = Animator.StringToHash(IS_JUMPING);
            _isFallingHash = Animator.StringToHash(IS_FALLING);
            _inputMagnitudeHash = Animator.StringToHash(INPUT_MAGNITUDE);
        }

        private Vector2 GetMoveInput() => InputManager.Instance.PlayerActions.Move.ReadValue<Vector2>();

        private void JumpCooldown() => _timer = _timer > 0 ? _timer - Time.deltaTime : 0;

        private void Update()
        {
            GroundCheck();
            HandleMovement();
            HandleJumpAndGravity();
            JumpCooldown();
            SetSlopeType();
        }

        private void HandleMovement()
        {
            Vector2 moveInput = GetMoveInput();
            _inputVec = new Vector3(moveInput.x, 0.0f, moveInput.y);

            _inputMagnitude = _canRun ? 1.0f : 0.5f;

            if (_inputVec.magnitude > 0.0f)
            {
                // Smooth rotation towards movement direction
                float targetAngle = Mathf.Atan2(_inputVec.x, _inputVec.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, _smoothTime);

                transform.rotation = Quaternion.Euler(0.0f, smoothedAngle, 0.0f);

                // Move in the direction of input
                Vector3 moveDir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                _controller.Move(moveDir * _speed * _inputMagnitude * Time.deltaTime);
            }
            else
            {
                _inputMagnitude = 0.0f;
            }

            // Apply downward force when on shallow slope
            if (_onShallowSlope && _inputVec.magnitude > 0.0f && _isGrounded)
                _controller.Move(Vector3.down * _controller.height / 2 * _slopeForce * Time.deltaTime);

            // Slide down steep slopes
            if (_onSteepSlope)
            {
                if (_isGrounded)
                    SteepSlopeSliding();
                else
                    _animator.SetBool(_isFallingHash, true);
            }
        }

        private void HandleJumpAndGravity()
        {
            float gravityMultiplier = 2f;

            // Set Idle, Walk and Run blend parameters
            _animator.SetFloat(_inputMagnitudeHash, _inputMagnitude, 0.1f, Time.deltaTime);

            if (_isGrounded)
            {
                _isJumping = false;

                _animator.SetBool(_isJumpingHash, false);
                _animator.SetBool(_isFallingHash, false);

                // Apply gravity when grounded and not jumping
                if (_verticalVel.y < 0.0f) _verticalVel.y = _groundedGravity;

                // Handle jump input
                if (_isJumpPressed && !_isJumping && _timer <= 0.0f)
                {
                    _timer = _cooldownTime;

                    _isJumping = true;

                    _animator.SetBool(_isJumpingHash, true);

                    // Calculate jump velocity
                    _verticalVel.y = Mathf.Sqrt(-2 * Physics.gravity.y * _jumpHeight);
                }
            }
            else
            {
                // Player is in the air
                if (_verticalVel.y < 0.0f)
                {
                    _isJumping = false;

                    _animator.SetBool(_isJumpingHash, false);
                    _animator.SetBool(_isFallingHash, true);
                }
            }

            // Apply gravity
            _verticalVel.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;

            _controller.Move(_verticalVel * Time.deltaTime);
        }

        private void GroundCheck()
        {
            float groundedOffset = -0.14f;

            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);

            _isGrounded = Physics.CheckSphere(spherePosition, _groundDist, _groundLayer);
        }

        private void SetSlopeType()
        {
            if (_isJumping)
            {
                _onShallowSlope = false;
                _onSteepSlope = false;
                return;
            }

            // Detect and set the slope type
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, _controller.height / 2 * (_slopeForceRayLength + _jumpHeight)))
            {
                if (hitInfo.normal != Vector3.up)
                {
                    float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

                    if (angle < _controller.slopeLimit)
                    {
                        _onShallowSlope = true;
                        _onSteepSlope = false;
                    }
                    else
                    {
                        _slopeHit = hitInfo;
                        _onSteepSlope = true;
                        _onShallowSlope = false;
                    }
                }
                else
                {
                    _onShallowSlope = false;
                    _onSteepSlope = false;
                }
            }
        }

        private void SteepSlopeSliding()
        {
            // Calculate the sliding direction along the slope
            Vector3 slideDirection = new Vector3(_slopeHit.normal.x, -_slopeHit.normal.y, _slopeHit.normal.z);
            _controller.Move(slideDirection * _slidingSpeed * Time.deltaTime);
        }
    }
}
