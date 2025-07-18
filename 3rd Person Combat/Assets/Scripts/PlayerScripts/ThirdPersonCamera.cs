using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    InputActionAsset InputActions;

    private InputAction lookAction;

    private Vector2 lookInput;

    [SerializeField] private Transform playerTransform;

    [SerializeField] private float controllerLookSpeed;
    [SerializeField] private float mouseLookSpeed;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float distance; 
    [SerializeField] private float height;
    [SerializeField] private float autoRotateSpeed = 2f; 



    private float yaw = 0f;
    private float pitch = 20f;


    public Vector3 forwardDirection; 
    public Vector3 rightDirection; 

    private void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Look"); 
    }

    private void LateUpdate()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        //lookInput.Normalize(); 

        float sensitivity = controllerLookSpeed;

        var device = lookAction.activeControl?.device;
        if (device is Mouse)
        {
            sensitivity = mouseLookSpeed; 
        }


        if (lookInput.sqrMagnitude > 0.01f)
        {
            yaw += lookInput.x * sensitivity * Time.deltaTime;
            pitch -= lookInput.y * sensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -40f, 40f); // Limit pitch to prevent flipping
        }
        else
    {
        PlayerMovement playerMovement = playerTransform.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerMovement.moveInput.magnitude > 0.01f)
        {
            Vector3 playerForward = playerTransform.forward;
            playerForward.y = 0;
            playerForward.Normalize();
            float targetYaw = Mathf.Atan2(playerForward.x, playerForward.z) * Mathf.Rad2Deg;

            yaw = Mathf.LerpAngle(yaw, targetYaw, autoRotateSpeed * Time.deltaTime);
        }
    }

        Vector3 offset = new Vector3(0, height, -distance); 
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 cameraPosition = playerTransform.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, cameraPosition, moveSpeed);
        transform.LookAt(playerTransform.position + Vector3.up * height * 0.5f);


        forwardDirection = gameObject.transform.forward; 
        rightDirection = gameObject.transform.right;
    }
}
