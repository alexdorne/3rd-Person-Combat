using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    InputSystem_Actions InputActions;

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

    private bool isLockedOn = false; // Track if the camera is locked on to a target

    private Transform lockOnTarget; 

    private void Awake()
    {
        InputActions = new InputSystem_Actions();
        InputActions.Player.Enable();
        InputActions.Player.LockOn.performed += ctx => LockOn(); // Bind lock-on action
    }

    private void LateUpdate()
    {
        NaturalCameraMovement();

        //if (!isLockedOn)
        //{
        //}
        //else
        //{
        //    LockedOnMovement();
        //}
    }

    private void NaturalCameraMovement()
    {
        lookInput = InputActions.Player.Look.ReadValue<Vector2>();
        //lookInput.Normalize(); 

        float sensitivity = controllerLookSpeed;

        var device = InputActions.Player.Look.activeControl?.device;
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

    private void LockedOnMovement()
    {
        if (lockOnTarget == null)
        {
            isLockedOn = false;
            return;
        }

        // 1. Camera follows the player with the usual offset
        Vector3 offset = new Vector3(0, height, -distance);
        Vector3 cameraPosition = playerTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, moveSpeed);

        // 2. Camera looks at a point between the player and the enemy (for a more centered view)
        Vector3 lookPoint = (playerTransform.position + lockOnTarget.position) * 0.25f;
        lookPoint.y = playerTransform.position.y + height * 0.5f; // Keep the look point at a reasonable height

        transform.LookAt(lookPoint);

        forwardDirection = transform.forward;
        rightDirection = transform.right;
    }

    private void LockOn()
    {
        // Implement lock-on functionality here if needed
        // This could involve snapping the camera to a target or adjusting the yaw/pitch based on a target's position

        // Find all enemies in the scene (assumes they have the "Enemy" tag)

        if (isLockedOn && lockOnTarget != null)
        {
            // If already locked on, unlock
            isLockedOn = false;
            lockOnTarget = null;
            Debug.Log("Lock-on cancelled");
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            isLockedOn = false;
            lockOnTarget = null;

            Debug.Log("No enemies found"); 
            return;
        }

        Debug.Log("Locking on to closest enemy...");

        Camera cam = Camera.main;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        float minDistance = float.MaxValue;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            Renderer renderer = enemy.GetComponentInChildren<Renderer>();
            if (renderer == null || !renderer.isVisible)
                continue;

            Vector3 screenPos = cam.WorldToScreenPoint(enemy.transform.position);

            // Check if enemy is in front of the camera and within the viewport
            if (screenPos.z < 0 ||
                screenPos.x < 0 || screenPos.x > Screen.width ||
                screenPos.y < 0 || screenPos.y > Screen.height)
                continue;

            float distanceToCenter = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), screenCenter);
            if (distanceToCenter < minDistance)
            {
                minDistance = distanceToCenter;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            isLockedOn = true;
            lockOnTarget = closestEnemy;
        }
        else
        {
            isLockedOn = false;
            lockOnTarget = null;
        }

    }
}
