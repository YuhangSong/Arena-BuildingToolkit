using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongBallController : MonoBehaviour
{
    private string last_hit_agent;
    private string last_hit_object;

    void Start()
    {
        this.reset();
    }

    public void reset()
    {
        this.last_hit_agent = "None";
        this.last_hit_object = "None";
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Table"))
        {
            if (this.last_hit_agent == "None")
            {
                // if noone has hitted the ball
                this.last_hit_agent = other.gameObject.GetComponentInParent<PingPondAgent>().Competitor.tag;
                this.last_hit_object = "Player";
            }

            if (this.last_hit_agent == other.gameObject.GetComponentInParent<PingPondAgent>().tag)
            {
                // hit on the table of own side
                other.gameObject.GetComponentInParent<PingPondAgent>().trig_loss();
            }

            if (this.last_hit_object == "Table")
            {
                // double hit on table
                other.gameObject.GetComponentInParent<PingPondAgent>().trig_loss();
            }
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (this.last_hit_object == "Player")
            {
                // if hit ball after a hit
                other.gameObject.GetComponentInParent<PingPondAgent>().trig_loss();
            }
        }

        if (other.gameObject.CompareTag("Floor"))
        {
            if (this.last_hit_object != "Table")
            {
                // if hit the ground without hitting the table
                GameObject.FindGameObjectWithTag(this.last_hit_agent).GetComponentInChildren<PingPondAgent>().trig_loss();
            }
            if (this.last_hit_object == "Table")
            {
                // if hit the ground after hitting the table
                GameObject.FindGameObjectWithTag(this.last_hit_agent).GetComponentInChildren<PingPondAgent>().trig_win();
            }
        }

        if (other.gameObject.CompareTag("Player")||other.gameObject.CompareTag("Table"))
        {
            this.last_hit_object = other.gameObject.tag;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            this.last_hit_agent = other.gameObject.GetComponentInParent<PingPondAgent>().tag;
        }
    }
}
