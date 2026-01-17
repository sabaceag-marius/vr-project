using UnityEngine;

public class RobotController : MonoBehaviour
{
    [Header("Body Parts")]
    public Rigidbody torso;
    public ConfigurableJoint leftHip;
    public ConfigurableJoint rightHip;
    public ConfigurableJoint leftShoulder;
    public ConfigurableJoint rightShoulder;

    [Header("Movement Settings")]
    public float speed = 10f;          // How fast legs cycle
    public float stepSize = 45f;       // How high legs lift
    public float moveForce = 2000f;    // Pushes the body forward
    public float turnSpeed = 500f;     // Rotates the body

    private float timer;

    void FixedUpdate()
    {
        // 1. Get Keyboard Input (WASD)
        float v = Input.GetAxis("Vertical");   // W / S
        float h = Input.GetAxis("Horizontal"); // A / D

        // 2. Walk Animation (Only if moving)
        if (Mathf.Abs(v) > 0.1f)
        {
            timer += Time.fixedDeltaTime * speed * v;

            // Calculate Sine Wave (Rhythm)
            float leftLegTarget = Mathf.Sin(timer);
            float rightLegTarget = Mathf.Sin(timer + Mathf.PI); // Opposite leg

            // Move Legs (Hips)
            leftHip.targetRotation = Quaternion.Euler(leftLegTarget * stepSize, 0, 0);
            rightHip.targetRotation = Quaternion.Euler(rightLegTarget * stepSize, 0, 0);

            // Move Arms (Shoulders - Opposite to legs)
            leftShoulder.targetRotation = Quaternion.Euler(rightLegTarget * stepSize, 0, 0);
            rightShoulder.targetRotation = Quaternion.Euler(leftLegTarget * stepSize, 0, 0);
        
            // 3. Move Forward Force
            torso.AddForce(torso.transform.forward * v * moveForce * Time.fixedDeltaTime);
        }

        // 4. Turn Force
        if (Mathf.Abs(h) > 0.1f)
        {
            torso.AddTorque(Vector3.up * h * turnSpeed * Time.fixedDeltaTime);
        }
    }
}