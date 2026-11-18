//Dan Ivanovas
using UnityEngine;

public class WaypointPath : MonoBehaviour
{

    //Remember get and set? 

    //This is how to get all child gameobjects of the parent
    public int Count
    {
        get { return transform.childCount; }
    }


    //I'd start from the top and moves along the index to the bottom
    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= Count)
        {
            return null;
        }

        return transform.GetChild(index);
    }
    //This is how you can visual display the waypoints and connections
    void OnDrawGizmos()
    {
        //set colour
        Gizmos.color = Color.yellow;

        //For each child game game get their transform
        //Then draw a sphere at said transform(can adjust size of sphere by changing 0.15
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform waypoint = transform.GetChild(i);

            Gizmos.DrawSphere(waypoint.position, 0.15f);

            //This one I didn't know existing but you can draw a line from one waypoint to the next waypoitn by doing this
            if (i < transform.childCount - 1)
            {
                Transform nextWaypoint = transform.GetChild(i + 1);

                Gizmos.DrawLine(waypoint.position, nextWaypoint.position);
            }
        }
    }
}
