using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{

    public static event System.Action OnGuardHasSpottedPlayer;

    public Transform pathHolder;
    public Light spotlight;
    public float viewDistance;
    float viewAngle;

    public float speed = 7;
    public float turnSpeed = 90;
    public float waitTime = 2;
    public LayerMask viewMask;

    Color originalSpotlightColor;
    Transform player;

    public float timeToSpotPlayer = .5f;
    float playerVisibleTimer;

    void Start() {

        viewAngle = spotlight.spotAngle;
        originalSpotlightColor = spotlight.color;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        /* Collect waypoints the guard will be walking */
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        /* Start the guarding */
        StartCoroutine(FollowPath(waypoints));
    }

    private void Update() {
        if (CanSeePlayer()) {
            spotlight.color = Color.red;
            playerVisibleTimer += Time.deltaTime;
        } else {
            spotlight.color = originalSpotlightColor;
            playerVisibleTimer -= Time.deltaTime;
        }

        /* Spotlight gets redder the longer the player is in view */
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer) {
            if (OnGuardHasSpottedPlayer != null) {
                OnGuardHasSpottedPlayer();
            }
        }
    }

    bool CanSeePlayer() {
        /* Within our view distance */
        if (Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            /* Within our scope of vision */
            if (angleBetweenGuardAndPlayer < viewAngle/2f) {
                /* Raycasting hits nothing */
                if (!Physics.Linecast(transform.position, player.position, viewMask)) {
                    return true;
                }
            }
        }
        return false;
    }

    /* CoRoutine to let the guard walk along the waypoint path, stopping at each for a given amount of time */
    IEnumerator FollowPath(Vector3[] waypoints) {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWayPoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWayPoint);

        /* The guard never gets a break, ever, we're not running a charity here. */
        while (true) {
            /* walk to it */
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint, speed * Time.deltaTime);

            if (transform.position == targetWayPoint) {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWayPoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWayPoint));
            }

            /* Wait for one frame. */
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 target) {
        Vector3 directionToLook = (target - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToLook.x, directionToLook.z) * Mathf.Rad2Deg;

        /* 0.05 instead of 0 to avoid precision issues */
        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
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
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
