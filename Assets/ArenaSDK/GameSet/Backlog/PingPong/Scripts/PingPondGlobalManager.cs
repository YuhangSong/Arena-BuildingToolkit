using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPondGlobalManager : MonoBehaviour
{
    private List<GameObject> respawns = new List<GameObject>();

    private Vector3 ball_position;

    // Start is called before the first frame update
    void Start()
    {
        // things to repspawn
        foreach (GameObject each in GameObject.FindGameObjectsWithTag("NONE"))
        {
            this.respawns.Add(each);
        }

        foreach (GameObject each in GameObject.FindGameObjectsWithTag("PlayerWall"))
        {
            Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Ball").GetComponent<Collider>(), each.GetComponent<Collider>());
        }

        ball_position = GameObject.FindGameObjectWithTag("Ball").transform.position;

        reset_ball();
    }

    private int random_sign()
    {
        return Random.Range(0, 2) * 2 - 1;
    }

    private void reset_ball()
    {
        GameObject.FindGameObjectWithTag("Ball").GetComponentInChildren<PingPongBallController>().reset();

        Vector3 temp = ball_position;
        //temp.z += Random.Range(-0.1f, 0.1f);
        GameObject.FindGameObjectWithTag("Ball").transform.position = temp;
        GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>().velocity = Vector3.zero;

        Vector3 force = Vector3.zero;
        force.x = Random.Range(75.0f, 85.0f); //* random_sign();
        GameObject.FindGameObjectWithTag("Ball").GetComponentInChildren<Rigidbody>().AddForce(force);
    }

    // called by the agent to reset the global
    public void Reset()
    {
        // things to be respawned at the reset
        foreach (GameObject each in this.respawns)
        {
            each.SetActive(true);
        }

        // things to be destryed at reset
        foreach (GameObject each in GameObject.FindGameObjectsWithTag("NONE"))
        {
            Destroy(each.gameObject);
        }

        reset_ball();
    }
}
