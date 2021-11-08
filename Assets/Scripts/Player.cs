using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    new Rigidbody rigidbody;

    public float moveSpeed = 7;
    public float smoothMovetime = .1f;
    public float turnSpeed = 8;

    float smoothInputMagnitude;
    float smoothMoveVelocity;
    float angle;

    Vector3 velocity;

    public event System.Action OnGoalReached;
    public event System.Action OnCoinCollected;

    bool disabled;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled) {
            /* Raw user input that will inform how we turn or move */
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }

        /* Convert that into a smoothed amount so that we don't jitter and move abruptly */
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMovetime);

        /* Smooth angle as well to prevent abrupt turning */
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        /* multiply by inputMagnitude to prevent turning back towards 0 when there is no user input (because inputMagnitude=0 this will result in no turning) */
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        /* Rotate and translate player according to smoothed parameters */
        transform.eulerAngles = Vector3.up * angle;
        transform.Translate(transform.forward * Time.deltaTime * moveSpeed * smoothInputMagnitude, Space.World);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    private void Disable() {
        disabled = true;
    }

    void FixedUpdate() {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Goal") {
            if (OnGoalReached != null) {
                Disable();
                OnGoalReached();
            }
        }
        if (other.tag == "Coin") {
            if (OnCoinCollected != null) {
                Destroy(other.gameObject);
                OnCoinCollected();
            }
        }
    }

    void OnDestroy() {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}
