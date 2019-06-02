using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollaballAgent : Agent
{
	// necessary public reference
    public RollaballAgent Competitor;
    public RollaballGlobalManager globalManager;
    // customize public reference
    public GameObject player;

    // private conifg: action space
    private const int NoAction = 0;
    private const int Left = 1;
    private const int Right = 2;
    private const int Up = 3;
    private const int Down = 4;
    // private conifg: others
    private const float move_force = 30f;
    private const float force_increasing_speed = 0.5f;

    // private status
    private Vector3 player_position;
    private float force_increasing = 0.0f;
    private int last_action;
    private float force_revert = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        player_position = player.transform.position;
        if (tag == "AgentA")
        {
            force_revert = 1.0f;
        }else if (tag == "AgentB")
        {
            force_revert = -1.0f;
        }
    }

    private void shared_trig_win_loss()
    {
        // send done signal to me, competitor and global
        Done();
        Competitor.Done();
    }

    public void trig_tie()
    {
        Debug.Log(this.GetComponentInParent<RollaballAgent>().tag + " trig_tie");
        this.AddReward(0.0f);
        Competitor.AddReward(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_win()
    {
        Debug.Log(this.GetComponentInParent<RollaballAgent>().tag + " trig_win");
        this.AddReward(1.0f);
        Competitor.AddReward(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_loss()
    {
        Debug.Log(this.GetComponentInParent<RollaballAgent>().tag + " trig_loss");
        this.AddReward(0.0f);
        Competitor.AddReward(1.0f);
        this.shared_trig_win_loss();
    }
    
    public override void AgentReset()
    {
        Debug.Log(this.GetComponentInParent<RollaballAgent>().tag + " reset with reward " + this.GetReward());
        this.force_increasing = 0.0f;

        // player.transform.position = new Vector3(
        //     player_position.x + Random.Range(-1.5f, 1.5f),
        //     player_position.y,
        //     player_position.z + Random.Range(-3.0f, 3.0f));
        player.transform.position = new Vector3(
            player_position.x,
            player_position.y,
            player_position.z);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        globalManager.Reset();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = Mathf.FloorToInt(vectorAction[0]);

        if (action != NoAction)
        {
            if (action == last_action)
            {
                force_increasing += force_increasing_speed;
            }
            else
            {
                force_increasing = 0.0f;
            }
        }
        else
        {
            force_increasing = 0.0f;
        }

        switch (action)
        {
            case NoAction:
                break;
            case Down:
                player.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(-force_revert*(move_force + force_increasing),0.0f, 0.0f));
                break;
            case Up:
                player.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(force_revert*(move_force + force_increasing), 0.0f, 0.0f));
                break;
            case Right:
                player.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0.0f, 0.0f, force_revert*(move_force + force_increasing)));
                break;
            case Left:
                player.GetComponentInChildren<Rigidbody>().AddForce(new Vector3(0.0f, 0.0f, -force_revert*(move_force + force_increasing)));
                break;
            default:
                break;
        }

        last_action = action;
    }

}
