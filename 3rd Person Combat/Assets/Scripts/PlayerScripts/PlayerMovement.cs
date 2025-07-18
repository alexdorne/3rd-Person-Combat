using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction moveAction; 
    private InputAction jumpAction; 

    ThirdPersonCamera thirdPersonCamera; 

    private Animator animator; 
    private Rigidbody rb; 

    
    private Vector2 moveInput;
    
    [SerializeField] private float walkSpeed; 
    [SerializeField] private float jogSpeed; 
    [SerializeField] private float sprintSpeed;

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
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();


    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>(); 


    }

    private void FixedUpdate()
    {
        Move(jogSpeed); 
    }

    private void Move(float speed)
    {
        if (IsGrounded())
        {
            Debug.Log("Grounded");
            Vector3 moveDirection = new Vector3(moveInput.x * thirdPersonCamera.lookDirection.x, 0, moveInput.y * thirdPersonCamera.lookDirection.z) * speed;
        }
    }

    private bool IsGrounded()
    {
        int groundLayer = LayerMask.GetMask("Ground");
        return Physics.Raycast(transform.position, Vector3.down, 0.1f + Physics.defaultContactOffset, groundLayer); 
    }
}
