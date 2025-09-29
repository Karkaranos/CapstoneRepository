using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCameraMovement : MonoBehaviour
{
    [Header("moving")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 1f;

    [Header("Looking")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxLookAngle = 80f;
    private float verticalRotation = 0f;
    private float turnDirection;

    private PlayerControls inputActions;
    private Rigidbody rb;
    private Vector2 lookInput;
    private Vector3 moveInput;
    private Transform eyes;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerControls();
        eyes = transform.Find("Eyes").GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (deceleration > 1)
        {
            deceleration = 1;
        }
        if (deceleration < 0)
        {
            deceleration = 0;
        }
    }
    private void FixedUpdate()
    {
        DoMovement();
    }
    private void Look(InputAction.CallbackContext obj)
    {
        lookInput = obj.ReadValue<Vector2>();
        DoLook();
    }
    private void MoveCanceled(InputAction.CallbackContext obj)
    {
        moveInput = Vector3.zero;
    }
    private void MovePerformed(InputAction.CallbackContext obj)
    {
        moveInput = obj.ReadValue<Vector3>();
    }
    private void DoMovement()
    {
        Vector3 moveDirection = transform.forward * moveInput.z + transform.right * moveInput.x + transform.up * moveInput.y;
        if (moveInput == Vector3.zero)
        {
            Vector3 horizontalVelocity = rb.linearVelocity;
            Vector3 decelerationForce = -horizontalVelocity * deceleration;
            rb.AddForce(decelerationForce, ForceMode.VelocityChange);
            return;
        }

        moveDirection.Normalize();
        Vector3 desiredVelocity = moveDirection * moveSpeed;
        Vector3 velocityDifference = desiredVelocity - rb.linearVelocity;
        Vector3 accelStep = Vector3.ClampMagnitude(velocityDifference, acceleration * Time.fixedDeltaTime);

        rb.AddForce(accelStep, ForceMode.VelocityChange);
    }
    private void DoLook()
    {
        if (lookInput.sqrMagnitude > 0.001f)
        {
            float rotationAmount = lookInput.x * rotationSpeed * Time.fixedDeltaTime * Time.timeScale;
            transform.Rotate(Vector3.up, rotationAmount);
            verticalRotation -= lookInput.y * rotationSpeed * Time.fixedDeltaTime * Time.timeScale;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            eyes.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
    private void OnEnable()
    {
        inputActions.FreeCamActions.Enable();
        inputActions.FreeCamActions.Move.performed += MovePerformed;
        inputActions.FreeCamActions.Move.canceled += MoveCanceled;
        inputActions.FreeCamActions.Look.performed += Look;

    }
    private void OnDisable()
    {
        inputActions.FreeCamActions.Disable();
        inputActions.FreeCamActions.Move.performed -= MovePerformed;
        inputActions.FreeCamActions.Move.canceled -= MoveCanceled;
        inputActions.FreeCamActions.Look.performed -= Look;
    }
}
