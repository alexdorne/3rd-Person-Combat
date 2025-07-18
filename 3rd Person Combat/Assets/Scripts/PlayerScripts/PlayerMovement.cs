using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction moveAction; 
    private InputAction jumpAction; 

    [SerializeField] ThirdPersonCamera thirdPersonCamera; 

    [SerializeField] private Animator animator; 
    private Rigidbody rb; 

    private Vector3 smoothedMoveDirection;
    [SerializeField] private float directionSmoothTime = 0.1f;
    private Vector3 directionSmoothVelocity;
    
    private Vector2 moveInput;

    [SerializeField] private float acceleration; 
    [SerializeField] private float walkSpeed; 
    [SerializeField] private float jogSpeed;
    [SerializeField] private float sprintSpeed;

    [SerializeField] private float rotationSpeed; 
    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable(); 
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody>();


    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>(); 
        animator.SetFloat("Speed", rb.linearVelocity.magnitude); 

    }

    private void FixedUpdate()
    {
        Move(acceleration); 
    }

    private void Move(float acceleration)
    {
        if (thirdPersonCamera == null)
        {
            Debug.LogError("ThirdPersonCamera is not assigned in PlayerMovement.");
            return;
        }

        Vector3 cameraForward = thirdPersonCamera.forwardDirection;

        cameraForward.y = 0; 
        cameraForward.Normalize(); 

        Vector3 cameraRight = thirdPersonCamera.rightDirection;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection =  cameraForward * moveInput.y + cameraRight * moveInput.x;
        rb.AddForce(moveDirection * acceleration); 
        
        Vector3 horizontalVelocity = rb.linearVelocity;

        horizontalVelocity.y = 0;

        if (horizontalVelocity.magnitude > jogSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * jogSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }

        RotateTowardsMovementDirection(moveDirection);

        if (IsGrounded())
        {
            
            Debug.Log("Grounded");
        }
    }

    private bool IsGrounded()
    {
        int groundLayer = LayerMask.GetMask("Ground");
        return Physics.Raycast(transform.position, Vector3.down, 0.1f + Physics.defaultContactOffset, groundLayer); 
    }

    private void RotateTowardsMovementDirection(Vector3 moveDirection)
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            // Smooth the move direction
            smoothedMoveDirection = Vector3.SmoothDamp(
                smoothedMoveDirection,
                moveDirection,
                ref directionSmoothVelocity,
                directionSmoothTime
            );

            // Only rotate if the smoothed direction is significant
            if (smoothedMoveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(smoothedMoveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }
    }
}
