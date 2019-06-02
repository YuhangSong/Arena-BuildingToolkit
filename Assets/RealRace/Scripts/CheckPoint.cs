using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int Number;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPointBody"))
        {
            other.gameObject.GetComponentInParent<RealRaceAgent>().UpdateCurrentCheckPoint(this.Number);
        }
    }
}
