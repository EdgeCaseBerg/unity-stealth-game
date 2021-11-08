using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float degreesPerSpin = 180;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, degreesPerSpin * Time.deltaTime);
    }
}
