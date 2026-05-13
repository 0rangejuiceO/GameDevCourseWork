using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NewPlayerInput : NetworkBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference ascendAction;
    [SerializeField] private InputActionReference dropAction;
    [SerializeField] private InputActionReference lookAction;

    [Header("Camera")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    private float pitch = 0f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float ascendForce = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 18f;

    [Header("Hover")]
    [SerializeField] private float hoverHeight = 2f;
    [SerializeField] private float hoverForce = 10f;
    [SerializeField] private float hoverDamping = 0.5f;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private float standardHoverPitch = 1f;
    [SerializeField] private float usingStaminaPitch = 1.3f;
    [SerializeField] private float regenStaminaPitch = 0.9f;

    [Header("Public Variables")]
    public float currentStamina;
    private Vector3 currentHorizontalVelocity;
    private Rigidbody rb;
    private Slider staminaSlider;

    public bool LockMovement = false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            currentStamina = maxStamina;

            moveAction.action.Enable();
            sprintAction.action.Enable();
            ascendAction.action.Enable();
            dropAction.action.Enable();
            lookAction.action.Enable();

            staminaSlider = GameObject.Find("Stamina").GetComponent<Slider>();
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;

        }
    }

    private void Start()
    {
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

    }
    void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
        ascendAction.action.Disable();
        dropAction.action.Disable();
        lookAction.action.Disable();
    }

    void Update()
    {
        if (!LockMovement && IsOwner)
        {
            HandleLook();
        }

    }

    void FixedUpdate()
    {
        if (!LockMovement && IsOwner)
        {
            HandleHover();
            HandleMovement();
            HandleStamina();
        }
    }

    void HandleLook()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        // Mouse sensitivity
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Rotate player (yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera (pitch)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleHover()
    {
        if (dropAction.action.IsPressed()) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, hoverHeight))
        {
            float heightError = hoverHeight - hit.distance;
            float upwardSpeed = rb.linearVelocity.y;

            float lift = heightError * hoverForce - upwardSpeed * hoverDamping;
            rb.AddForce(Vector3.up * lift, ForceMode.Acceleration);
        }
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        float maxSpeed = moveSpeed;

        if (sprintAction.action.IsPressed() && currentStamina > 0)
        {
            maxSpeed *= sprintMultiplier;
            currentStamina -= Time.fixedDeltaTime * staminaDrainRate;
        }

        Vector3 targetVelocity = moveDir * maxSpeed;

        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

        // Decide whether to accelerate or decelerate
        float accelRate = (moveDir.magnitude > 0.1f) ? acceleration : deceleration;

        // Smoothly move toward target velocity (THIS is the "AAA feel")
        currentHorizontalVelocity = Vector3.Lerp(
            currentHorizontalVelocity,
            targetVelocity,
            accelRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(
            currentHorizontalVelocity.x,
            velocity.y,
            currentHorizontalVelocity.z
        );

        // Ascend
        if (ascendAction.action.IsPressed() && currentStamina > 0)
        {
            rb.AddForce(Vector3.up * ascendForce, ForceMode.Acceleration);
            currentStamina -= Time.fixedDeltaTime * staminaDrainRate;
        }
    }

    void HandleStamina()
    {
        bool usingStamina =
            (sprintAction.action.IsPressed() || ascendAction.action.IsPressed())
            && currentStamina > 0;

        bool regainingStamina =
            !usingStamina && currentStamina < maxStamina;

        // Drain stamina
        if (usingStamina)
        {
            // Pitch while using stamina
            playerAudio.pitch = usingStaminaPitch;
        }
        else
        {
            // Regen stamina
            if (currentStamina < maxStamina)
            {
                currentStamina += Time.fixedDeltaTime * staminaRegenRate;

                // Pitch while regenerating
                playerAudio.pitch = regenStaminaPitch;
            }
            else
            {
                // Normal pitch
                playerAudio.pitch = standardHoverPitch;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaSlider.value = currentStamina;
    }
}
