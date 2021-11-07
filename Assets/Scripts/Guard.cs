using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    public Transform pathHolder;

    public float speed = 7;
    public float waitTime = 2;

    void Start() {

        /* Collect waypoints the guard will be walking */
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        /* Start the guarding */
        StartCoroutine(FollowPath(waypoints));
    }

    /* CoRoutine to let the guard walk along the waypoint path, stopping at each for a given amount of time */
    IEnumerator FollowPath(Vector3[] waypoints) {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWayPoint = waypoints[targetWaypointIndex];

        /* The guard never gets a break, ever, we're not running a charity here. */
        while (true) {
            /* walk to it */
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint, speed * Time.deltaTime);

            if (transform.position == targetWayPoint) {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWayPoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
            }

            /* Wait for one frame. */
            yield return null;
        }
    }

    /* Show guard's path with gizmos for debugging*/
    private void OnDrawGizmos() {
        Vector3 startPos = pathHolder.GetChild(0).position;
        Vector3 previousPos = startPos;

        foreach(Transform waypoint in pathHolder) {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPos, waypoint.position);
            previousPos = waypoint.position;
        }

        Gizmos.DrawLine(previousPos, startPos);
    }
}
