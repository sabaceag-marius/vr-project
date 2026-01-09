using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;

    private InputActions inputActions;

    private InputAction moveInputAction;
    private InputAction jumpInputAction;

    [SerializeField]
    private float movementSpeed = 20;

    [SerializeField]
    private float jumpForce = 10;

    [SerializeField]
    private Transform orientation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputActions();
        moveInputAction = inputActions.Player.Move;
        jumpInputAction = inputActions.Player.Jump;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = moveInputAction.ReadValue<Vector2>();

        Vector3 moveDirection = (orientation.forward * movement.y) + (orientation.right * movement.x);
        
        rb.linearVelocity = new Vector3(moveDirection.x * movementSpeed, rb.linearVelocity.y, moveDirection.z * movementSpeed);
        
        if (jumpInputAction.WasPressedThisFrame())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();
}
