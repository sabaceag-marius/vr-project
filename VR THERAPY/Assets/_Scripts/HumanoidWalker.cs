using UnityEngine;

public class HumanoidWalker : MonoBehaviour
{
    [Header("Core")]
    public Rigidbody torso;
    public float moveForce = 3000f;
    public float maxSpeed = 6f;
    public float uprightForce = 2000f;

    [Header("Legs")]
    public ConfigurableJoint hipL;
    public ConfigurableJoint hipR;
    public HingeJoint kneeL;
    public HingeJoint kneeR;
    public float strideAngle = 45f;
    public float kneeBendAngle = 60f;

    [Header("Arms")]
    public ConfigurableJoint shoulderL; // Was "armL"
    public ConfigurableJoint shoulderR; // Was "armR"
    public HingeJoint elbowL;          // NEW
    public HingeJoint elbowR;          // NEW
    public float armSwingAngle = 40f;
    public float elbowBend = 90f;      // 0 = Straight, 90 = Runner pose

    [Header("Timing")]
    public float walkSpeed = 8f;
    private float timer;

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime * walkSpeed;

        // Rhythm
        float sin = Mathf.Sin(timer);
        float oppSin = Mathf.Sin(timer + Mathf.PI);

        // 1. LEGS (Walk)
        hipL.targetRotation = Quaternion.Euler(sin * strideAngle, 0, 0);
        hipR.targetRotation = Quaternion.Euler(oppSin * strideAngle, 0, 0);

        // 2. KNEES (Lift feet)
        JointSpring kSpringL = kneeL.spring;
        kSpringL.targetPosition = Mathf.Clamp01(sin) * -kneeBendAngle;
        kneeL.spring = kSpringL;

        JointSpring kSpringR = kneeR.spring;
        kSpringR.targetPosition = Mathf.Clamp01(oppSin) * -kneeBendAngle;
        kneeR.spring = kSpringR;

        // 3. SHOULDERS (Swing Opposite to Legs)
        shoulderL.targetRotation = Quaternion.Euler(oppSin * armSwingAngle, 0, 0);
        shoulderR.targetRotation = Quaternion.Euler(sin * armSwingAngle, 0, 0);

        // 4. ELBOWS (Keep them bent like a runner)
        // We set the spring target to a fixed angle (e.g., 90 degrees)
        // You can also animate this if you want dynamic chopping motion
        JointSpring eSpringL = elbowL.spring;
        eSpringL.targetPosition = elbowBend;
        elbowL.spring = eSpringL;

        JointSpring eSpringR = elbowR.spring;
        eSpringR.targetPosition = elbowBend;
        elbowR.spring = eSpringR;

        // 5. BALANCE & MOVE
        Quaternion uprightTarget = Quaternion.FromToRotation(transform.up, Vector3.up);
        torso.AddTorque(new Vector3(uprightTarget.x, uprightTarget.y, uprightTarget.z) * uprightForce);

        if (torso.linearVelocity.magnitude < maxSpeed)
        {
            torso.AddForce(torso.transform.forward * moveForce * Time.fixedDeltaTime);
        }
    }
}