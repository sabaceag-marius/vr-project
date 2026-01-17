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

    private Transform[] waypoints;

    private Vector3 currentTargetPosition;

    private void Awake()
    {
        if (waypointOptions == null) return;

        int idx = Random.Range(0, waypointOptions.Count());

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

        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTargetPosition,
            speed * Time.deltaTime
        );

        Vector3 direction = currentTargetPosition - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        float distance = Vector3.Distance(transform.position, currentTargetPosition);

        if (distance <= reachDistance)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;

                // Teleport logic
                transform.position = waypoints[0].position;
                SetRandomTargetForCurrentWaypoint();
            }
            else
            {
                SetRandomTargetForCurrentWaypoint();
            }
        }
    }

    void SetRandomTargetForCurrentWaypoint()
    {
        if (waypoints.Length == 0) return;

        // 1. Get the center of the waypoint
        Vector3 center = waypoints[currentWaypointIndex].position;

        // 2. Pick a random point inside a flat circle (X and Z only)
        Vector2 randomOffset = Random.insideUnitCircle * pathRandomness;

        // 3. Create the new target position (keeping Y the same as the waypoint to avoid sinking/flying)
        currentTargetPosition = new Vector3(
            center.x + randomOffset.x,
            center.y,
            center.z + randomOffset.y
        );
    }

    int GetClosestForwardWaypoint()
    {
        if (waypoints.Length == 0) return 0;

        int bestIndex = -1;
        float closestDistSqr = Mathf.Infinity;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Vector3 toWaypoint = waypoints[i].position - transform.position;
            float dotProduct = Vector3.Dot(transform.forward, toWaypoint.normalized);

            if (dotProduct > 0)
            {
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

    void OnDrawGizmos()
    {
        return;

        if (waypoints == null || waypoints.Length < 2) return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Draw connection lines
            Gizmos.color = Color.yellow;
            Transform nextWaypoint = waypoints[(i + 1) % waypoints.Length];
            if (nextWaypoint != null)
            {
                Gizmos.DrawLine(waypoints[i].position, nextWaypoint.position);
            }

            // Draw Reach Distance
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(waypoints[i].position, reachDistance);

            // --- NEW: Draw the Randomness Zone (Blue Wire Sphere) ---
            // This shows you the area where people might walk
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(waypoints[i].position, pathRandomness);
        }
    }
}