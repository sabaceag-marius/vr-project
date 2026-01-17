using UnityEngine;

public class AutoWalker : MonoBehaviour
{
    [Header("Muscles")]
    public ConfigurableJoint leftLeg;
    public ConfigurableJoint rightLeg;
    public Rigidbody torso;

    [Header("Movement Settings")]
    public float walkSpeed = 10f;      // How fast legs cycle
    public float stepSize = 25f;       // How wide legs swing
    public float moveForce = 4000f;    // Pushing power
    public float maxSpeed = 6f;        // SPEED LIMIT (Stops flying)

    [Header("Balance")]
    public float uprightForce = 500f;  // Keeps him standing

    private float timer;

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * walkSpeed;

        // 1. Leg Animation (The Walk)
        float leftTarget = Mathf.Sin(timer);
        float rightTarget = Mathf.Sin(timer + Mathf.PI);

        leftLeg.targetRotation = Quaternion.Euler(leftTarget * stepSize, 0, 0);
        rightLeg.targetRotation = Quaternion.Euler(rightTarget * stepSize, 0, 0);

        // 2. The Smart Push (With Speed Limit)
        // Only push if we are below the speed limit
        if (torso.linearVelocity.magnitude < maxSpeed)
        {
            // Use ForceMode.Force (Safe and standard)
            torso.AddForce(torso.transform.forward * moveForce * Time.fixedDeltaTime);
        }

        // 3. Balance (Keep Upright)
        Quaternion uprightTarget = Quaternion.FromToRotation(transform.up, Vector3.up);
        torso.AddTorque(new Vector3(uprightTarget.x, uprightTarget.y, uprightTarget.z) * uprightForce);
    }
}