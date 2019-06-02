using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayerLossTrig : MonoBehaviour
{
    public TankAgent me;
    public string destoryer_tag = "Bullet";
    public int total_life = 1;

    private int life;

    void Start()
    {
        this.life = this.total_life;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(this.destoryer_tag))
        {

            this.life -= 1;
            if (this.life == 0)
            {
                me.trig_loss();
                this.life = this.total_life;
            }
        }
    }
}
