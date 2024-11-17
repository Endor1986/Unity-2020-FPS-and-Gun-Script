using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkingSpeed = 5.0f;
    public float sprintingSpeed = 10.0f;
    public float crouchingSpeed = 2.5f;
    public float jumpHeight = 1.2f;
    public float gravityScale = 2.0f;

    [Header("Physics Settings")]
    public float mass = 1.0f;
    public float fallMultiplier = 2.5f;

    [Header("Key Bindings")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100.0f;
    public Transform cameraTransform;

    [Header("Height Settings")]
    public float standingHeight = 1.8f;
    public float crouchingHeight = 0.9f;

    [Header("Head Bobbing Settings")]
    public bool enableHeadBob = true;
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float midpoint = 1.8f;

    [Header("Climbing Settings")]
    public bool isClimbing = false; // Zustand für das Klettern

    private CharacterController controller;
    private float xRotation = 0f;
    private bool isCrouching = false;
    private Vector3 playerVelocity;
    private Vector3 originalCameraPosition;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        originalCameraPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleCameraRotation();

        if (!isGrounded)
        {
            playerVelocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;
        }

        controller.Move(playerVelocity * Time.deltaTime);

        if (enableHeadBob && !isCrouching) // Anwendung des Head Bobbing nur, wenn der Spieler nicht geduckt ist
        {
            ApplyHeadBob();
        }
    }

    public void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * x + transform.forward * z;

        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        float speed = isCrouching ? crouchingSpeed : (Input.GetKey(sprintKey) ? sprintingSpeed : walkingSpeed);
        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y * gravityScale);
        }
    }

    public void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchingHeight : standingHeight;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x,
                Mathf.Lerp(cameraTransform.localPosition.y, isCrouching ? originalCameraPosition.y - (standingHeight - crouchingHeight) / 2 : originalCameraPosition.y, Time.deltaTime * 10),
                cameraTransform.localPosition.z);
        }
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void ApplyHeadBob()
    {
        if (!isCrouching) // Überprüfung, ob der Spieler geduckt ist
        {
            float waveslice = Mathf.Sin(Time.time * bobbingSpeed);
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;

            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, midpoint + translateChange, cameraTransform.localPosition.z);
        }
        else
        {
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, originalCameraPosition.y, cameraTransform.localPosition.z);
        }

    }

    public CharacterController GetCharacterController()
    {
      return controller;      
    }
    public void SetClimbing(bool climbing)
    {
        isClimbing = climbing;
        controller.enabled = !climbing; // Deaktiviert den CharacterController beim Klettern
                                        // Optional: Weitere Anpassungen, wie z.B. das Deaktivieren der Schwerkraft, falls erforderlich
    }

    public bool IsClimbing()
    {
        return isClimbing;
    }

}