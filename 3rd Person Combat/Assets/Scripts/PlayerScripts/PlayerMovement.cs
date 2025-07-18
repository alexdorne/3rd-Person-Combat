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
    
    [HideInInspector] public Vector2 moveInput;

    [SerializeField] private float acceleration; 
    [SerializeField] private float walkSpeed; 
    [SerializeField] private float jogSpeed;
    [SerializeField] private float sprintSpeed;

    [SerializeField] private float jumpForce = 5f; // Adjust this value as needed for jump height

    [SerializeField] private float rotationSpeed; 

    private bool canMove = true; 
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

        Vector3 animationSpeed = rb.linearVelocity; 
        animationSpeed.y = 0; 
        animator.SetFloat("Speed", animationSpeed.magnitude); 

        if (jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump(); 
        }

    }

    private void FixedUpdate()
    {
        Move(acceleration); 
    }

    private void Jump()
    {
        animator.SetTrigger("Jumping"); 
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
        Debug.Log("Jumping!");
    }

    private void DodgeRoll()
    {
        animator.SetTrigger("DodgeRoll");   

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

        if (IsGrounded())
        {
            rb.AddForce(moveDirection * acceleration); 
        }
        
        Vector3 horizontalVelocity = rb.linearVelocity;

        horizontalVelocity.y = 0;

        if (horizontalVelocity.magnitude > jogSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * jogSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }

        if (moveInput.magnitude > 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
            RotateTowardsMovementDirection(moveDirection);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |RigidbodyConstraints.FreezeRotationZ; 
        }

    }

    private bool IsGrounded()
    {
        float rayLength = 1.2f;
        int groundLayer = LayerMask.GetMask("Ground");

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Vector3 origin = transform.localPosition + new Vector3(0, 1, 0);


        return Physics.Raycast(origin, Vector3.down, rayLength, groundLayer);
    }

    private void RotateTowardsMovementDirection(Vector3 moveDirection)
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
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
