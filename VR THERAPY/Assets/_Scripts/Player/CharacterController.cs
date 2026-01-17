using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;

    private InputActions inputActions;

    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private InputAction interactInputAction;

    [SerializeField]
    private float movementSpeed = 20;

    [SerializeField]
    private float jumpForce = 10;

    [SerializeField]
    private Transform orientation;

    [SerializeField]
    private float interactionDistance;

    [SerializeField]
    private GameObject interactionUI;

    private TextMeshProUGUI interactionUIText;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputActions();
        moveInputAction = inputActions.Player.Move;
        jumpInputAction = inputActions.Player.Jump;
        interactInputAction = inputActions.Player.Interact;

        interactionUIText = interactionUI.GetComponent<TextMeshProUGUI>();
        Debug.Log(interactionUIText);
    }

    void Update()
    {
        Vector2 movement = moveInputAction.ReadValue<Vector2>();

        Vector3 moveDirection = (orientation.forward * movement.y) + (orientation.right * movement.x);
        
        rb.linearVelocity = new Vector3(moveDirection.x * movementSpeed, rb.linearVelocity.y, moveDirection.z * movementSpeed);
        
        if (jumpInputAction.WasPressedThisFrame())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        InteractionRay();
    }

    private void InteractionRay()
    {
        Ray ray = new Ray(orientation.position, orientation.forward);
        RaycastHit hit;

        //Debug.DrawRay(orientation.position, orientation.forward, Color.green, 0.1f);

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // show text

                interactionUIText.text = $"Press E to {interactable.GetDescription()}";

                if (interactInputAction.WasPressedThisFrame())
                {
                    interactable.Interact();
                }
            }
            else
            {
                interactionUIText.text = string.Empty;
            }
        }
        else
        {
            interactionUIText.text = string.Empty;
        }
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();
}
