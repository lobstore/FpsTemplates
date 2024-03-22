using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class FirstPersonController : MonoBehaviour
{
    public UnityEvent OnSprinting { get; set; } = new();
    public UnityEvent OnJumped { get; set; } = new();

    IStaminaController _staminaController;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField]
    private float MoveSpeed = 4.0f;

    [Tooltip("Rotation speed of the character")]
    [SerializeField]
    private float RotationSpeed = 1.0f;
    [Tooltip("Acceleration and deceleration")]
    [SerializeField]
    private float SpeedChangeRate = 10.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField]
    private float SprintSpeed = 6.0f;
    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField]
    private float JumpHeight = 1.2f;
    [SerializeField]
    private bool isSprinting = false;
    private bool isCrouching = false;
    [SerializeField]
    PlayerConfig _playerConfig;

    private float SprintCostStamina;
    private float JumpCostStamina;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField]
    private float Gravity = -9.81f;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField]
    private float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField]
    private float BottomClamp = -90.0f;

    // cinemachine
    private float _cinemachineTargetPitch;
    // player
    [SerializeField]
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private CharacterController _controller;
    private InputHandler _input;

    private const float _threshold = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
            return false;
#endif
        }
    }

    private void Awake()
    {
        SprintCostStamina = _playerConfig.SprintCost;
        JumpCostStamina = _playerConfig.JumpCost;
        // get a reference to our main camera
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<InputHandler>();
        _playerInput = GetComponent<PlayerInput>();
        _staminaController = GetComponent<IStaminaController>();
    }
    private void OnEnable()
    {
        _input.OnJumped.AddListener(Jump);
        _input.IsSprinting.AddListener(DoSprint);
        _input.IsCrouching.AddListener(Crouch);
    }
    private void OnDisable()
    {
        _input.OnJumped.RemoveListener(Jump);
        _input.IsSprinting.RemoveListener(DoSprint);
        _input.IsCrouching.RemoveListener(Crouch);
    }

    private void Update()
    {
        ApplyGravity();
        Move();
    }
    private void FixedUpdate()
    {
        CheckAbove();
        Debug.DrawRay(transform.position, transform.up * _controller.height, Color.green);
    }

    private void CheckAbove()
    {
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(transform.position, transform.up, out hit, _controller.height);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Character>()!=null)
            {
                SitDown();
            }
            else
            {
                _controller.height = 1f;
            }
        }
        else
        {
            SitDown();
        }
    }

   private void SitDown()
    {
        if (isCrouching == true)
        {
            _controller.height = 1f;
        }
        else
        {
            _controller.height = 2f;
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }
    private bool IsGrounded { get => _controller.isGrounded; }

    private void CameraRotation()
    {
        // if there is an input
        if (_input.look.sqrMagnitude >= _threshold)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed;
        if (isSprinting
            && _staminaController.HasEnoughStamina(SprintCostStamina)
            && !isCrouching)
        {
            _staminaController.ReduceStyamina(SprintCostStamina);
            targetSpeed = SprintSpeed;
            OnSprinting.Invoke();
        }
        else
        {
            targetSpeed = MoveSpeed;

        }
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            // move
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
        }

        // move the player
        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }
    private void ApplyGravity()
    {

        // stop our velocity dropping infinitely when grounded
        if (IsGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }
    private void Jump()
    {
        if (IsGrounded
            && !isCrouching
            && _staminaController.HasEnoughStamina(JumpCostStamina)
            )
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            OnJumped.Invoke();
            _staminaController.ReduceStyamina(JumpCostStamina);
        }

    }

    private void Crouch(bool IsCrouching)
    {
        if (IsCrouching)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }
    }

    private void DoSprint(bool IsSprinting)
    {
        if (IsSprinting
            && IsGrounded
            && _staminaController.HasEnoughStamina(SprintCostStamina)
            && !isCrouching)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}