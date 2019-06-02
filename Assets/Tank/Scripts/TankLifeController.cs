using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLifeController : MonoBehaviour
{
    public GameObject explosion;
    public string destoryer_tag= "Bullet";
    public int total_life = 1;

    private int life;

    void Start()
    {
       this.life = this.total_life;
    }


    void OnEnable()
    {
        this.life = this.total_life;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(this.destoryer_tag))
        {
            GameObject explosionPuff = Instantiate(explosion, other.transform.position, other.transform.rotation) as GameObject;
            Destroy(explosionPuff, 2);

            if (this.total_life > 0)
            {
                this.life -= 1;
                if (this.life == 0)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
