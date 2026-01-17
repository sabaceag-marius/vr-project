using System.Linq;
using UnityEngine;

public class PeoplePathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField]
    private GameObject[] waypointOptions;

    private GameObject waypoint;

    [SerializeField]
    private float speed = 5.0f;

    [SerializeField]
    private float rotationSpeed = 2.0f;

    [SerializeField]
    private float reachDistance = 1.0f;

    [SerializeField]
    private float pathRandomness = 1.5f;

    [SerializeField]
    private int currentWaypointIndex = 0;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float obstacleCheckDistance = 3.0f; // How far ahead to look
    [SerializeField] private float avoidanceForce = 5.0f;        // How hard to turn away
    [SerializeField] private LayerMask obstacleLayers;

    [Range(3, 20)]
    [SerializeField] private int numberOfRays = 7; 
    [Range(10, 180)]
    [SerializeField] private float raySpreadAngle = 90f;

    private Transform[] waypoints;

    private Vector3 currentTargetPosition;

    private void Awake()
    {
        if (waypointOptions == null || waypointOptions.Length == 0) return;

        int idx = Random.Range(0, waypointOptions.Length);

        Debug.Log($"{idx}, {waypointOptions.Length}");

        waypoint = waypointOptions[idx];

        if (idx == 1)
        {
            transform.rotation = new Quaternion(0, -180, 0, 1);
        }

        waypoints = waypoint.GetComponentsInChildren<Transform>().Where(t => t != waypoint.transform).ToArray();

        if (waypoints.Length < currentWaypointIndex)
        {
            currentWaypointIndex = 0;
        }
    }

    void Start()
    {
        currentWaypointIndex = GetClosestForwardWaypoint();
        SetRandomTargetForCurrentWaypoint();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // 1. Calculate the desired direction towards the target
        Vector3 directionToTarget = (currentTargetPosition - transform.position).normalized;

        // 2. Apply Obstacle Avoidance
        Vector3 finalDirection = ApplyObstacleAvoidance(directionToTarget);

        // 3. Rotate towards the final direction
        if (finalDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // 4. Move Forward
        // Note: We simply move "Forward" now, trusting the rotation to guide us
        transform.position += transform.forward * speed * Time.deltaTime;

        // 5. Check if we reached the target
        float distance = Vector3.Distance(transform.position, currentTargetPosition);
        if (distance <= reachDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
                transform.position = waypoints[0].position;
                SetRandomTargetForCurrentWaypoint();
            }
            else
            {
                SetRandomTargetForCurrentWaypoint();
            }
        }
    }

    Vector3 ApplyObstacleAvoidance(Vector3 desiredDirection)
    {
        Vector3 avoidanceVector = Vector3.zero;
        bool obstacleDetected = false;

        // Calculate the starting angle (far left of the fan)
        float halfSpread = raySpreadAngle / 2f;
        float angleStep = raySpreadAngle / (numberOfRays - 1);

        for (int i = 0; i < numberOfRays; i++)
        {
            // 1. Calculate the direction for this specific ray
            // We start at -45 degrees and move to +45 degrees (if spread is 90)
            float currentAngle = -halfSpread + (angleStep * i);

            // Convert angle to a Vector direction relative to the person
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            // 2. Cast the Ray
            if (Physics.Raycast(transform.position + Vector3.up, rayDirection, obstacleCheckDistance, obstacleLayers))
            {
                // HIT!

                // If we hit something with this ray, we want to move in the OPPOSITE direction.
                // e.g. If the RIGHT ray hits, we subtract "Right" from our path, pushing us Left.
                avoidanceVector -= rayDirection;
                obstacleDetected = true;

                // Optional: Draw Hit Debug Line (White)
                Debug.DrawRay(transform.position + Vector3.up, rayDirection * obstacleCheckDistance, Color.white);
            }
            else
            {
                // Optional: Draw Clear Debug Line (Green - very faint)
                Debug.DrawRay(transform.position + Vector3.up, rayDirection * obstacleCheckDistance, new Color(0, 1, 0, 0.1f));
            }
        }

        if (obstacleDetected)
        {
            // Normalize the result so the force is consistent
            return (desiredDirection + (avoidanceVector.normalized * avoidanceForce)).normalized;
        }

        return desiredDirection;
    }

    void SetRandomTargetForCurrentWaypoint()
    {
        if (waypoints.Length == 0) return;
        Vector3 center = waypoints[currentWaypointIndex].position;
        Vector2 randomOffset = Random.insideUnitCircle * pathRandomness;
        currentTargetPosition = new Vector3(center.x + randomOffset.x, center.y, center.z + randomOffset.y);
    }

    int GetClosestForwardWaypoint()
    {
        // (Keep your existing logic here, it is good)
        if (waypoints.Length == 0) return 0;
        int bestIndex = -1;
        float closestDistSqr = Mathf.Infinity;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Vector3 toWaypoint = waypoints[i].position - transform.position;
            if (Vector3.Dot(transform.forward, toWaypoint.normalized) > 0)
            {
                float distSqr = toWaypoint.sqrMagnitude;
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    bestIndex = i;
                }
            }
        }
        return bestIndex == -1 ? GetAbsoluteClosestWaypoint() : bestIndex;
    }

    int GetAbsoluteClosestWaypoint()
    {
        // (Keep your existing logic here)
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

    void OnDrawGizmos()
    {
        if (waypoints == null) return;

        // Draw the target line
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, currentTargetPosition);

        // Draw the Fan of Rays (Red Lines)
        Gizmos.color = Color.red;
        float halfSpread = raySpreadAngle / 2f;

        // We need to protect against divide by zero if numberOfRays is 1 (though range limits it to 3)
        int safeRayCount = Mathf.Max(2, numberOfRays);
        float angleStep = raySpreadAngle / (safeRayCount - 1);

        for (int i = 0; i < safeRayCount; i++)
        {
            float currentAngle = -halfSpread + (angleStep * i);

            // Important: We must use the object's CURRENT rotation to draw the gizmos correctly in Editor
            Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            Gizmos.DrawRay(transform.position + Vector3.up, rayDirection * obstacleCheckDistance);
        }
    }
}
