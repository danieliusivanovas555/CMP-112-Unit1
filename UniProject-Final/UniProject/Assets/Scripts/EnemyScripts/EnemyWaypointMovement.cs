//Dan Ivanovas
using UnityEngine;

public class EnemyWaypointMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    public WaypointPath path;

    //Privately track the current waypoint and if the enemy has reached the exit
    private int waypointIndex;
    private bool hasReachedExit;
    private Vector3 startPosition;

    private PlayerHealth playerHealth;

    void Start()
    {
        //Safety Checks

        //Find the player <---- because prefab
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        //Find the waypoint path if not assigned
        if (path == null)
        {
            path = FindFirstObjectByType<WaypointPath>();
        }

        //Stop movement if no path exists
        if (path == null || path.Count == 0)
        {
            Debug.LogError("No waypoint path found!", this);
            enabled = false;
            return;
        }

        //Remember where the enemy started for path progress
        startPosition = transform.position;
    }

    void Update()
    {
        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        //Stop if movement isn't possible
        if (path == null || hasReachedExit || moveSpeed <= 0f)
        {
            return;
        }

        //Calculate movement distance over Time and move Speed
        float remainingDistance = moveSpeed * Time.deltaTime;

        //Move through waypoints using available transform childs
        while (remainingDistance > 0f && waypointIndex < path.Count)
        {
            Transform waypoint = path.GetWaypoint(waypointIndex);

            if (waypoint == null)
            {
                break;
            }

            //Return Distance from point waypoint A to waypoint B
            float distance = Vector3.Distance(transform.position, waypoint.position);

            if (distance <= remainingDistance)
            {
                // Reach waypoint and use leftover movement
                transform.position = waypoint.position;
                remainingDistance -= distance;

                waypointIndex++;
            }
            else
            {
                // Move towards waypoint
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, remainingDistance);

                remainingDistance = 0f;
            }
        }

        //Check if the enemy has reached the exit
        if (waypointIndex >= path.Count)
        {
            ReachedExit();
        }
    }

    public float PathProgress
    {
        get
        {
            if (path == null)
            {
                return 0f;
            }

            if (waypointIndex >= path.Count)
            {
                return path.Count;
            }

            Vector3 start;

            //Use the spawn position for the first waypoint
            if (waypointIndex == 0)
            {
                start = startPosition;
            }
            else
            {
                start = path.GetWaypoint(waypointIndex - 1).position;
            }

            Vector3 end = path.GetWaypoint(waypointIndex).position;
            Vector3 direction = end - start;

            if (direction.sqrMagnitude == 0f)
            {
                return waypointIndex;
            }

            float progress = Vector3.Dot(transform.position - start, direction) / direction.sqrMagnitude;

            return waypointIndex + Mathf.Clamp01(progress);
        }
    }

    void ReachedExit()
    {
        //Prevent damaging the player multiple times
        if (hasReachedExit)
        {
            return;
        }

        hasReachedExit = true;

        //Damage the player by 1
        if (playerHealth != null)
        {
            //This is how you reference the script once you need to damage the enemies with towers but instead of player you take enemyDamage also this script is responsable for triggering the screen to go red using volumes
            playerHealth.TakeDamage(1);
        }

        // Destroy the enemy
        Destroy(gameObject);
    }
}
