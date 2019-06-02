using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingBodyController : MonoBehaviour
{
    public float hurting;
    public float hurted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Body"))
        {
            BoxingBodyController other_body = other.gameObject.GetComponent(typeof(BoxingBodyController)) as BoxingBodyController;
            this.GetComponentInParent<BoxingAgent>().hurt(other_body.hurting * this.hurted);
        }
    }
}
