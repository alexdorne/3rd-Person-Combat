using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    InputActionAsset InputActions;

    private InputAction lookAction;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float lookRotationSpeed; 

    public Vector3 lookDirection; 

    private void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Look"); 
    }

    private void Update()
    {
        lookDirection = gameObject.transform.forward; 
    }
}
