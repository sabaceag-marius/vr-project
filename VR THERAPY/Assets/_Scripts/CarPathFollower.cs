using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CarPathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField]
    private GameObject waypoint;

    [SerializeField]
    private float speed = 5.0f;
    
    [SerializeField]
    private float rotationSpeed = 2.0f;
    
    [SerializeField]
    private float reachDistance = 1.0f; // How close to get before switching to next point

    [SerializeField]
    private int currentWaypointIndex = 0;

    private Transform[] waypoints;

    private void Awake()
    {
        waypoints = waypoint.GetComponentsInChildren<Transform>().Where(t => t != waypoint.transform).ToArray();

        if (waypoints.Length < currentWaypointIndex)
        {
            currentWaypointIndex = 0;
        }
    }

    void Start()
    {
        currentWaypointIndex = GetClosestForwardWaypoint();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // 2. Move the car towards the target
        // We use MoveTowards to ensure we move at a constant speed
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            speed * Time.deltaTime
        );

        //Rotate the car to face the target
        Vector3 direction = targetWaypoint.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Check if we have reached the waypoint
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        if (distance <= reachDistance)
        {
            // Increment the index to go to the next waypoint
            currentWaypointIndex++;

            // Loop back to the start if we reached the end of the array
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
    }

    int GetClosestForwardWaypoint()
    {
        if (waypoints.Length == 0) return 0;

        int bestIndex = -1;
        float closestDistSqr = Mathf.Infinity; // Start with a very high number

        // Loop through every waypoint to find the best match
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // 1. Calculate vector from Car to Waypoint
            Vector3 toWaypoint = waypoints[i].position - transform.position;

            // 2. Check if it is "Forward" using Dot Product
            // Result > 0 means the object is in front (roughly within 180 degrees)
            float dotProduct = Vector3.Dot(transform.forward, toWaypoint.normalized);

            if (dotProduct > 0)
            {
                // 3. Check distance (Squared magnitude is faster than Distance for comparisons)
                float distSqr = toWaypoint.sqrMagnitude;
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    bestIndex = i;
                }
            }
        }

        if (bestIndex == -1)
        {
            bestIndex = GetAbsoluteClosestWaypoint();
        }

        return bestIndex;
    }

    // Helper method for the fallback
    int GetAbsoluteClosestWaypoint()
    {
        int bestIndex = 0;
        float closestDistSqr = Mathf.Infinity;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 toWaypoint = waypoints[i].position - transform.position;
            float distSqr = toWaypoint.sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    // Draws the path in the Editor Scene view
    void OnDrawGizmos()
    {
        return;

        // 1. Draw the basic path lines (Yellow)
        if (waypoints == null || waypoints.Length < 2) return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Draw the connection lines
            Gizmos.color = Color.yellow;
            Transform nextWaypoint = waypoints[(i + 1) % waypoints.Length];
            if (nextWaypoint != null)
            {
                Gizmos.DrawLine(waypoints[i].position, nextWaypoint.position);
            }

            // 2. Draw the Reach Distance (Green Spheres)
            // This shows exactly how close the car must get to switch targets
            Gizmos.color = new Color(0, 1, 0, 0.3f); // Transparent Green
            Gizmos.DrawSphere(waypoints[i].position, reachDistance);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(waypoints[i].position, reachDistance);
        }

        // 3. Simulate the Movement (The "Ghost" Car)
        // This simulates 100 frames into the future to show how the car behaves
        // with your current Speed and Rotation Speed settings.

        Vector3 ghostPos = transform.position;
        Quaternion ghostRot = transform.rotation;
        int ghostTargetIndex = currentWaypointIndex;

        // Simulation parameters
        float simDeltaTime = 0.05f; // Simulate time steps (lower is smoother but heavier)
        int simulationSteps = 100;   // How many steps to look ahead

        for (int i = 0; i < simulationSteps; i++)
        {
            if (waypoints.Length == 0) break;

            Transform target = waypoints[ghostTargetIndex];

            // A. Simulate Move
            Vector3 prevPos = ghostPos;
            ghostPos = Vector3.MoveTowards(ghostPos, target.position, speed * simDeltaTime);

            // B. Simulate Rotate
            Vector3 dir = target.position - prevPos;
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                ghostRot = Quaternion.Slerp(ghostRot, targetRot, rotationSpeed * simDeltaTime);
            }

            // C. Simulate Reach Distance Check
            if (Vector3.Distance(ghostPos, target.position) < reachDistance)
            {
                ghostTargetIndex = (ghostTargetIndex + 1) % waypoints.Length;
            }

            // --- DRAWING THE SIMULATION ---

            // Draw the path of the ghost (Cyan)
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(prevPos, ghostPos);

            // Draw the "Nose" of the car (Red) to show rotation alignment
            // If the red lines point away from the cyan line, your car is drifting/sliding!
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ghostPos, ghostRot * Vector3.forward * 1.0f);
        }
    }
}