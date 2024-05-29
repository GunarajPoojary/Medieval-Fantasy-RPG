using UnityEngine;

namespace GunarajCode
{
    public class PlayerStateMachine : MonoBehaviour
    {
        PlayerBaseState _currentState;
        public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }

        private PlayerStateFactory _states;
        public PlayerStateFactory States { get => _states; set => _states = value; }

        private static Transform _cam;
        public static Transform Cam
        {
            get
            {
                if (_cam == null) _cam = Camera.main.transform;
                return _cam;
            }
        }

        [SerializeField] private Animator _animator;
        public Animator Animator { get => _animator; }
        private CharacterController _controller;
        private PlayerInputsAction _inputActions;

        [Header("Slope Variables")]
        [Tooltip("Used to add downward force when on slope")]
        [SerializeField] private float _slopeForce;
        [SerializeField] private float _slopeForceRayLength = 0.5f;
        [SerializeField] private float _slidingSpeed = 10f;

        private RaycastHit _slopeHit;
        private bool _onShallowSlope, _onSteepSlope;

        [Space(5), Header("Speed Variables")]
        [Tooltip("To determine how fast player can move")]
        [SerializeField] private float _speed = 5;
        private bool _canRun;
        private float _inputMagnitude;
        private Vector3 _inputVec;
        private float _turnSmoothVel;
        private float _smoothTime = 0.15f;

        [Space(5), Header("Ground and Jump Variables")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _jumpHeight = 4f;
        private bool _isJumpPressed, _isJumping;
        public bool IsJumpPressed { get => _isJumpPressed; }

        public bool IsJumping { get => _isJumping; set => _isJumping = value; }
        public int IsJumpingHash { get => _isJumpingHash; set => _isJumpingHash = value; }
        public int IsFallingHash { get => _isFallingHash; set => _isFallingHash = value; }

        public Vector3 VerticalVel { get => _verticalVel; set => _verticalVel = value; }
        public float GroundedGravity { get => _groundedGravity; set => _groundedGravity = value; }
        public float Timer { get => _timer; set => _timer = value; }
        public float CooldownTime { get => _cooldownTime; set => _cooldownTime = value; }
        public CharacterController Controller { get => _controller; set => _controller = value; }
        public float JumpHeight { get => _jumpHeight; set => _jumpHeight = value; }
        public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
        public float TerminalVelocity { get; internal set; }
        public int IsRunningHash { get => IsRunningHash; set => IsRunningHash = value; }

        private float _groundedGravity = -0.14f;
        private float _groundDist = 0.28f;
        private bool _isGrounded;

        private float _terminalVelocity = 53f;
        private Vector3 _verticalVel;

        [Header("Jump Cooldown")]
        [Tooltip("Delays the jump action based on the cooldown")]
        [SerializeField] private float _cooldownTime = 2f;
        private float _timer = 0.0f;

        private int _isJumpingHash, _isFallingHash, _isWalkingHash, _isRunningHash;

        void Awake()
        {
            // Hashes for animation parameters
            _isJumpingHash = Animator.StringToHash("IsJumping");
            _isFallingHash = Animator.StringToHash("IsFalling");
            _isWalkingHash = Animator.StringToHash("IsWalking");
            _isRunningHash = Animator.StringToHash("IsRunning");

            Controller = GetComponent<CharacterController>();

            _inputActions = new PlayerInputsAction();

            _inputActions.Player.Enable();

            _inputActions.Player.Jump.performed += ctx => _isJumpPressed = true;
            _inputActions.Player.Jump.canceled += ctx => _isJumpPressed = false;
            _inputActions.Player.Run.started += ctx => _canRun = true;
            _inputActions.Player.Run.canceled += ctx => _canRun = false;

            _states = new PlayerStateFactory(this);
            _currentState = _states.Grounded();
            _currentState.EnterState();
        }

        Vector2 GetMoveInput() => _inputActions.Player.Move.ReadValue<Vector2>();

        void Update()
        {
            HandleMovement();

            //if (_verticalVel.y < _terminalVelocity) _verticalVel.y += Physics.gravity.y * 2 * Time.deltaTime;

            //_controller.Move(_verticalVel * Time.deltaTime);

            JumpCooldown();

            GroundCheck();

            CurrentState.UpdateState();
        }

        void HandleMovement()
        {
            _inputVec = new Vector3(GetMoveInput().x, 0.0f, GetMoveInput().y);

            if (_inputVec.magnitude > 0.0f)
            {
                // Smooth rotation towards movement direction
                float TargetAngle = Mathf.Atan2(_inputVec.x, _inputVec.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
                float SmoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref _turnSmoothVel, _smoothTime);

                transform.rotation = Quaternion.Euler(0.0f, SmoothedAngle, 0.0f);

                // Move in the direction of input
                Vector3 MoveDir = Quaternion.Euler(0.0f, TargetAngle, 0.0f) * Vector3.forward;
                Controller.Move(MoveDir * _speed * Time.deltaTime);
            }

            // Apply downward force when on shallow slope
            //if (_onShallowSlope && _inputVec != Vector3.zero && _isGrounded)
            //    _controller.Move(Vector3.down * _controller.height / 2 * _slopeForce * Time.deltaTime);

            //// Slide down steep slopes
            //if (_onSteepSlope)
            //{
            //    if (_isGrounded)
            //        SteepSlopeSliding();
            //    else
            //        _animator.SetBool(_isFallingHash, true);
            //}
        }

        void JumpCooldown() => _timer = _timer > 0 ? _timer - Time.deltaTime : 0;

        void GroundCheck()
        {
            float GroundedOffset = -0.14f;

            Vector3 SpherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                                    transform.position.z);

            IsGrounded = Physics.CheckSphere(SpherePosition, _groundDist, _groundLayer);
        }
    }
}
