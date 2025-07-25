using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    InputSystem_Actions InputActions;

    [SerializeField] ThirdPersonCamera thirdPersonCamera; 

    [SerializeField] private Animator animator; 
    private Rigidbody rb; 

    private Vector3 smoothedMoveDirection;
    [SerializeField] private float directionSmoothTime = 0.1f;
    private Vector3 directionSmoothVelocity;

    CapsuleCollider playerCollider;

    [HideInInspector] public Vector2 moveInput;

    [SerializeField] private float acceleration; 
    [SerializeField] private float walkSpeed; 
    [SerializeField] private float jogSpeed;
    [SerializeField] private float sprintSpeed;

    [SerializeField] private float rollForce; 

    [SerializeField] private float jumpForce = 5f; // Adjust this value as needed for jump height

    [SerializeField] private float rotationSpeed;

    [SerializeField] private float hitCooldown = 0.5f; // Cooldown time between hits
    float timeBetweenHits; 

    private bool canMove = true;
    private bool canRotate = true;


    [SerializeField] Slider healthSlider; 

    [SerializeField] int maxHealth = 5;

    private int health; 

    public int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);
            healthSlider.value = health; 
            if (health <= 0)
            {
                // Handle player death here
                Debug.Log("Player has died.");
                DisableMovement(); // Disable movement on death
                DisableBodyRotation(); // Disable body rotation on death
                animator.SetBool("Dead", true); // Trigger death animation
            }
        }
    }

    private void OnEnable()
    {
        InputActions = new InputSystem_Actions();
        InputActions.Player.Enable();
        InputActions.Player.Jump.performed += ctx => Jump();
        InputActions.Player.DodgeRoll.performed += ctx => DodgeRoll();


    }
    private void OnDisable()
    {
        InputActions.Player.Jump.performed -= ctx => Jump();
        InputActions.Player.DodgeRoll.performed -= ctx => DodgeRoll();

    }

    private void Awake()
    {
        canRotate = true; // Allow rotation by default
        canMove = true; // Allow movement by default
        Health = maxHealth; // Initialize health to max health
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

    }

    private void Update()
    {
        if (timeBetweenHits < hitCooldown)
        {
            timeBetweenHits += Time.deltaTime; 
        }
        moveInput = InputActions.Player.Move.ReadValue<Vector2>();

        Vector3 animationSpeed = rb.linearVelocity; 
        animationSpeed.y = 0; 
        animator.SetFloat("Speed", animationSpeed.magnitude); 
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        animator.SetBool("Grounded", IsGrounded()); 

        //if (jumpAction.WasPressedThisFrame() && IsGrounded())
        //{
        //    Jump(); 
        //}

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

        StartCoroutine(DodgeRolling());
    }

    private IEnumerator DodgeRolling()
    {
        if (moveInput.magnitude > 0)
        {
            animator.SetTrigger("DodgeRoll");   

            playerCollider.excludeLayers = LayerMask.GetMask("Enemy"); 

            // Calculate roll direction based on current movement direction  
            Vector3 rollDirection = (thirdPersonCamera.forwardDirection * moveInput.y + thirdPersonCamera.rightDirection * moveInput.x).normalized;

            // Apply impulse force for the roll  
            rb.AddForce(rollDirection * rollForce, ForceMode.Impulse);


            float rollDuration = 0.6f; // Duration of the roll animation  
            yield return new WaitForSeconds(rollDuration);

            playerCollider.excludeLayers = LayerMask.GetMask("Nothing"); // Reset the collider layers after rolling

        }
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

        if (canMove)
        {
            rb.AddForce(moveDirection * acceleration); 
        }

        if (IsGrounded())
        {
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
        if (!canRotate)
        {
            return; 
        }

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

    public void DisableMovement()
    {
        canMove = false; 
        animator.SetBool("CanMove", false); // Disable movement in the animator
    }

    public void DisableBodyRotation()
    {
        canRotate = false;
    }

    public void EnableMovement()
    {
        canMove = true;
        animator.SetBool("CanMove", true); // Re-enable movement in the animator
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && timeBetweenHits >= hitCooldown)
        {
            timeBetweenHits = 0; // Reset hit cooldown
            TakeDamage(1); 
        }
    }

    private void TakeDamage(int damage)
    {
        Health -= damage;
    }
}
