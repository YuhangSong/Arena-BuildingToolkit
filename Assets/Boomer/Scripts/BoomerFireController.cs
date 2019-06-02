using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class BoomerFireController : MonoBehaviour
{
    // for how long the fire lives, set to nagitive number to make it live forever
    public float fire_time;

    private float time_start;

    // Start is called before the first frame update
    void
    Start()
    {
        this.time_start = Time.time;
        // destroy self after fire_time
        if (this.fire_time > 0.0f) {
            Destroy(this.gameObject, this.fire_time);
        }
    }

    void
    OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Tree")) {
            other.gameObject.SetActive(false);
        } else if (other.gameObject.CompareTag("Player")) {
            ArenaAgent ArenaAgent_ = other.gameObject.GetComponentInParent<ArenaAgent>();
            GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<GlobalManager>().KillAgent(
                ArenaAgent_.getTeamID(), ArenaAgent_.getAgentID());
        }
        // destroy self when hit anything
        Destroy(this.gameObject);
    }
}
