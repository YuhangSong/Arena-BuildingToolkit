using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBulletController : MonoBehaviour
{
    public GameObject explosion;

    void OnCollisionEnter(Collision other)
    { 
        GameObject explosionPuff = Instantiate(explosion, this.transform.position, this.transform.rotation) as GameObject;
        Destroy(explosionPuff, 2);
        Destroy(this.gameObject);
    }
}
