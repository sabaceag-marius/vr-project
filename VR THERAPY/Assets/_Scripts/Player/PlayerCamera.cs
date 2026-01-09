using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float sensivityX = 400;

    [SerializeField]
    private float sensivityY = 400;

    [SerializeField]
    private Transform orientation;

    private float xRotation, yRotation;

    private InputActions inputActions;

    private InputAction lookInputAction;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputActions = new InputActions();
        lookInputAction = inputActions.Player.Look;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseMovement = lookInputAction.ReadValue<Vector2>();

        float mouseX = mouseMovement.x * Time.deltaTime * sensivityX;
        float mouseY = mouseMovement.y * Time.deltaTime * sensivityY;

        yRotation += mouseX;
        xRotation -= mouseY; 

        // Clamp looking up/down so you can't flip over
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();
}
