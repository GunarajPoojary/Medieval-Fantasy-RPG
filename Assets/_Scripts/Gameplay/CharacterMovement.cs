using UnityEngine;
using UnityEngine.InputSystem;

namespace GunarajCode
{
    public class CharacterMovement : MonoBehaviour
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

        [SerializeField] private Animator _animator;

        private int _walkingHash, _runningHash;

        private CharacterController _controller;
        private PlayerInputAction _inputActions;

        private bool _isRunPressed, _isJumpPressed;
        private const string IS_RUNNING = "IsRunning";
        private const string IS_WALKING = "IsWalking";
        private Vector3 _inputVec;
        private float _speed;
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 10f;
        private float _turnSmoothVel;
        [SerializeField] private float _smoothTime = 0.15f;

        private float _initialJumpVel;
        private float _maxJumpHeight = 1.5f;
        private float _maxJumpTime = 0.5f;
        private bool _isJumping = false;

        private float _gravity = -9.81f;
        private float _groundedGravity;
        [SerializeField] private LayerMask _groundLayer;
        private bool _isGrounded;

        private Vector3 _verticalVel = Vector3.zero;

        private void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _inputActions = new PlayerInputAction();
            _inputActions.Player.Run.performed += ctx => _isRunPressed = ctx.ReadValueAsButton();
            _inputActions.Player.Jump.performed += OnJumpPressed;
            _inputActions.Player.Jump.canceled += OnJumpPressed;
        }

        private void SetupJumpVariables()
        {
            float timeToApex = _maxJumpTime / 2;
            _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            _initialJumpVel = (2 * _maxJumpHeight) / timeToApex;
        }

        private void OnJumpPressed(InputAction.CallbackContext ctx)
        {
            _isJumpPressed = ctx.ReadValueAsButton();
        }

        private void Start()
        {
            InitializeAnimationHashes();
        }

        private void InitializeAnimationHashes()
        {
            _walkingHash = Animator.StringToHash(IS_WALKING);
            _runningHash = Animator.StringToHash(IS_RUNNING);
        }

        private Vector2 GetMoveInput() => _inputActions.Player.Move.ReadValue<Vector2>();

        private void Update()
        {
            HandleMovement();
            HandleGravity();
            HandleJump();
            GroundCheck();

            _controller.Move(_verticalVel);
        }

        private void HandleMovement()
        {
            bool isWlaking = _animator.GetBool(_walkingHash);
            bool isRunning = _animator.GetBool(_runningHash);

            if (isWlaking)
            {
                _animator.SetBool(_walkingHash, true);
            }
            else
                _animator.SetBool(_walkingHash, false);

            if (isRunning)
            {
                _animator.SetBool(_runningHash, true);
            }
            else
                _animator.SetBool(_runningHash, false);

            _inputVec = new Vector3(GetMoveInput().x, 0.0f, GetMoveInput().y);

            _speed = _isRunPressed ? _runSpeed : _walkSpeed;

            if (_inputVec.magnitude > 0.0f)
            {
                // Smooth rotation towards movement direction
                float targetAngle = Mathf.Atan2(_inputVec.x, _inputVec.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, _smoothTime);

                transform.rotation = Quaternion.Euler(0.0f, smoothedAngle, 0.0f);

                // Move in the direction of input
                Vector3 moveDir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                _controller.Move(moveDir * _speed * Time.deltaTime);
            }
        }

        private void HandleGravity()
        {
            if (_isGrounded)
            {
                _verticalVel.y = _groundedGravity;
            }
            else
            {
                float previousYVel = _verticalVel.y;
                float newYVel = _verticalVel.y + (_gravity * Time.deltaTime);
                float nextYVel = (previousYVel + newYVel) * 0.5f;
                _verticalVel.y = nextYVel;
            }
        }

        private void HandleJump()
        {
            if (!_isJumping && _isGrounded && _isJumpPressed)
            {
                _isJumping = true;
                _verticalVel.y = _initialJumpVel * 0.5f;
            }
            else if (!_isJumpPressed && _isJumping && _isGrounded)
            {
                _isJumping = false;
            }
        }

        private void GroundCheck()
        {
            float groundedOffset = -0.14f;
            float groundDist = 0.28f;

            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);

            _isGrounded = Physics.CheckSphere(spherePosition, groundDist, _groundLayer);
        }
    }
}
