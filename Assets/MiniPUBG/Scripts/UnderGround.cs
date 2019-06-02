using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderGround : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponentInParent<StrikeAgent>().trig_loss();
        }
    }
}
