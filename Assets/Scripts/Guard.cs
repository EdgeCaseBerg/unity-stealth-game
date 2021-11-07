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
        }

        /* Start the guarding */
        StartCoroutine(WalkGuardPath(waypoints, waitTime));
    }

    /* CoRoutine to let the guard walk along the waypoint path, stopping at each for a given amount of time */
    IEnumerator WalkGuardPath(Vector3[] waypoints, float waitAtEachPointTime) {
        /* The guard never gets a break, ever, we're not running a charity here. */
        while (true) {
            /* For each waypoint of the Guard's */
            foreach (Vector3 waypoint in waypoints) {
                /* walk to it */
                while (transform.position != waypoint) {
                    transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
                    yield return null;
                }
                /* Destination reached, pause and look around a bit */
                yield return new WaitForSeconds(waitAtEachPointTime);
            }
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
