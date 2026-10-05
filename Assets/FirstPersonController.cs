using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform groundCheck;
    public LayerMask groundMask = ~0;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 6f;
    public float movementSharpness = 10f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.05f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.3f;

    private Rigidbody rb;

    private float yaw;
    private float pitch;
    private float pendingYawDelta;

    private bool isGrounded;
    private bool jumpRequested;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // PENTING: reset status ini setiap kali scene gameplay ini dimuat
        // (termasuk waktu Restart dari Game Over). Tanpa ini, kalau
        // sebelumnya sempat 'ketangkep' hantu waktu panel UI (soal/kode)
        // masih terbuka, status GameState.IsUIOpen bisa nyangkut 'true'
        // selamanya dan bikin SEMUA interaksi (E) serta gerakan berhenti.
        GameState.IsUIOpen = false;
    }


    private void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = 0f;

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.identity;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        // PENTING: kalau ada UI (misal panel soal logika) sedang terbuka,
        // Player tidak boleh bergerak/lihat sekeliling sama sekali.
        if (GameState.IsUIOpen)
        {
            return;
        }

        ReadMouseInput();
        ApplyCameraPitch();

        HandleJumpInput();
        HandleCursor();
    }


    private void FixedUpdate()
    {
        if (GameState.IsUIOpen)
        {
            // Hentikan gerakan horizontal supaya Player tidak "meluncur"
            // terus waktu panel UI kebuka.
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        ApplyBodyYaw();

        CheckGrounded();
        HandleMovement();
        HandleJump();
    }


    // =====================================================
    // MOUSE LOOK
    // =====================================================

    private void ReadMouseInput()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        pendingYawDelta += mouseDelta.x * mouseSensitivity;

        pitch -= mouseDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void ApplyCameraPitch()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void ApplyBodyYaw()
    {
        if (Mathf.Approximately(pendingYawDelta, 0f))
        {
            return;
        }

        yaw += pendingYawDelta;
        pendingYawDelta = 0f;

        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }


    // =====================================================
    // MOVEMENT
    // =====================================================

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
        }

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        bool sprinting = Keyboard.current != null &&
                         Keyboard.current.leftShiftKey.isPressed;

        float speed = sprinting ? sprintSpeed : walkSpeed;

        Vector3 inputDirection =
            transform.right * input.x + transform.forward * input.y;

        Vector3 targetVelocity = inputDirection * speed;

        Vector3 currentHorizontalVelocity =
            new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        Vector3 newHorizontalVelocity = Vector3.Lerp(
            currentHorizontalVelocity, targetVelocity,
            movementSharpness * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(
            newHorizontalVelocity.x, rb.linearVelocity.y, newHorizontalVelocity.z);
    }


    // =====================================================
    // JUMP
    // =====================================================

    private void HandleJumpInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void HandleJump()
    {
        if (!jumpRequested) return;
        jumpRequested = false;

        if (!isGrounded) return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }


    // =====================================================
    // GROUND CHECK
    // =====================================================

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = true;
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);
    }


    // =====================================================
    // CURSOR
    // =====================================================

    private void HandleCursor()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}