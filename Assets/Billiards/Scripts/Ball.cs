    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const float decayEvery = 0.5f;
    private const float stopVelocity = 0.2f;
    private const float velocityDecaySpeed = 0.2f;

    void Start()
    {
        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        InvokeRepeating("SpeeDetect", 0.0f, decayEvery);
        GetComponent<Rigidbody>().angularDrag = 0.0f;
        GetComponent<Rigidbody>().drag = 0.0f;
        GetComponent<Rigidbody>().mass = 0.01f;
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    void SpeeDetect()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude + GetComponent<Rigidbody>().angularVelocity.magnitude > 0.0f)
        {
            //Debug.Log(GetComponent<Rigidbody>().velocity.magnitude + GetComponent<Rigidbody>().angularVelocity.magnitude);
            GetComponent<Rigidbody>().velocity -= Vector3.Normalize(GetComponent<Rigidbody>().velocity)* velocityDecaySpeed;

            if (GetComponent<Rigidbody>().velocity.magnitude < stopVelocity)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (CompareTag("WhiteBall"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }
}
