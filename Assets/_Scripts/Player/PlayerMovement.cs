using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    static Transform _cam;
    public static Transform Cam
    {
        get
        {
            if (_cam == null) _cam = Camera.main.transform;
            return _cam;
        }
    }

    Animator _animator;
    CharacterController _controller;
    PlayerInputsAction _inputActions;

    [Header("Slope Variables")]
    [Tooltip("Used to add downward force when on slope")]
    [SerializeField] float _slopeForce;
    [SerializeField] float _slopeForceRayLength = 0.5f;
    [SerializeField] float _slidingSpeed = 10f;

    RaycastHit _slopeHit;
    bool _onShallowSlope, _onSteepSlope;

    [Space(5), Header("Speed Variables")]
    [Tooltip("To determine how fast player can move")]
    [SerializeField] float _speed = 5;
    bool _canRun;
    float _inputMagnitude;
    Vector3 _inputVec;
    float _turnSmoothVel;
    float _smoothTime = 0.15f;

    [Space(5), Header("Ground and Jump Variables")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _jumpHeight = 4f;
    bool _isJumpPressed, _isJumping;

    float _groundedGravity = -0.14f;
    float _groundDist = 0.28f;
    bool _isGrounded;

    float _terminalVelocity = 53f;
    Vector3 _verticalVel;

    [Header("Jump Cooldown")]
    [Tooltip("Delays the jump action based on the cooldown")]
    [SerializeField] float _cooldownTime = 2f;
    float _timer = 0.0f;

    int _isJumpingHash, _isFallingHash, _inputMagnitudeHash;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();

        // Hashes for animation parameters
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _isFallingHash = Animator.StringToHash("IsFalling");
        _inputMagnitudeHash = Animator.StringToHash("Input Magnitude");

        _inputActions = new PlayerInputsAction();

        _inputActions.Player.Enable();

        _inputActions.Player.Jump.performed += ctx => _isJumpPressed = true;
        _inputActions.Player.Jump.canceled += ctx => _isJumpPressed = false;
        _inputActions.Player.Run.started += ctx => _canRun = true;
        _inputActions.Player.Run.canceled += ctx => _canRun = false;
    }

    Vector2 GetMoveInput() => _inputActions.Player.Move.ReadValue<Vector2>();

    void JumpCooldown() => _timer = _timer > 0 ? _timer - Time.deltaTime : 0;

    void Update()
    {
        HandleMovement();
        HandleJumpAndGravity();
        GroundCheck();
        JumpCooldown();
        SetSlopeType();
    }

    void HandleMovement()
    {
        _inputVec = new Vector3(GetMoveInput().x, 0.0f, GetMoveInput().y);

        _inputMagnitude = _canRun ? 1 : 0.5f;

        if (_inputVec.magnitude > 0.0f)
        {
            // Smooth rotation towards movement direction
            float TargetAngle = Mathf.Atan2(_inputVec.x, _inputVec.z) * Mathf.Rad2Deg + Cam.eulerAngles.y;
            float SmoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref _turnSmoothVel, _smoothTime);

            transform.rotation = Quaternion.Euler(0.0f, SmoothedAngle, 0.0f);

            // Move in the direction of input
            Vector3 MoveDir = Quaternion.Euler(0.0f, TargetAngle, 0.0f) * Vector3.forward;
            _controller.Move(MoveDir * _speed * _inputMagnitude * Time.deltaTime);
        }
        else
            _inputMagnitude = 0.0f;

        // Apply downward force when on shallow slope
        if (_onShallowSlope && _inputVec != Vector3.zero && _isGrounded)
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

    void HandleJumpAndGravity()
    {
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

                _animator.SetBool(_isFallingHash, true);
            }
        }

        // Apply gravity
        if (_verticalVel.y < _terminalVelocity) _verticalVel.y += Physics.gravity.y * 2 * Time.deltaTime;

        _controller.Move(_verticalVel * Time.deltaTime);
    }

    void GroundCheck()
    {
        float GroundedOffset = -0.14f;

        Vector3 SpherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                                transform.position.z);

        _isGrounded = Physics.CheckSphere(SpherePosition, _groundDist, _groundLayer);
    }

    void SetSlopeType()
    {
        if (_isJumping)
        {
            _onShallowSlope = false;
            _onSteepSlope = false;
        }

        // Detect and set the slope type
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo,
                        _controller.height / 2 * (_slopeForceRayLength + _jumpHeight)))
        {
            if (hitInfo.normal != Vector3.up)
            {
                float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

                if (angle < _controller.slopeLimit)
                {
                    _onShallowSlope = true;
                }
                else if (angle > _controller.slopeLimit)
                {
                    _slopeHit = hitInfo;
                    _onSteepSlope = true;
                }
            }
            else
            {
                _onShallowSlope = false;
                _onSteepSlope = false;
            }
        }
    }

    void SteepSlopeSliding()
    {
        Vector3 slidingVelocity = Vector3.ProjectOnPlane(new Vector3(0.0f, _verticalVel.y, 0.0f), _slopeHit.normal);
        _controller.Move(slidingVelocity * _slidingSpeed * Time.deltaTime);
    }
}
