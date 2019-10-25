using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PingPondAgent : Agent
{
    // necessary public reference
    public PingPondAgent Competitor;
    public PingPondGlobalManager globalManager;
    // customize public reference
    public GameObject player;

    // priveta conifg: action space
    private const int NoAction = 0;
    private const int Left = 1;
    private const int Right = 2;
    private const int Up = 3;
    private const int Down = 4;
    private const int Forward = 5;
    private const int Backward = 6;
    // priveta conifg: others
    private const float move_force = 20f;
    private const float force_increasing_speed = 0.5f;

    // private status
    private Vector3 player_position;
    private float force_increasing = 0.0f;
    private int last_action;

    void Start()
    {
        player_position = player.transform.position;

        foreach (GameObject each in GameObject.FindGameObjectsWithTag("Table"))
        {
            Physics.IgnoreCollision(player.GetComponent<Collider>(), each.GetComponent<Collider>());
        }
    }

    private void shared_trig_win_loss()
    {
        // send done signal to me, competitor and global
        Done();
        Competitor.Done();
        globalManager.Reset();
    }

    public void trig_tie()
    {
        Debug.Log(this.GetComponentInParent<PingPondAgent>().tag + " trig_tie");
        this.AddReward(0.0f);
        Competitor.AddReward(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_win()
    {
        Debug.Log(this.GetComponentInParent<PingPondAgent>().tag + " trig_win");
        this.AddReward(1.0f);
        Competitor.AddReward(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_loss()
    {
        Debug.Log(this.GetComponentInParent<PingPondAgent>().tag + " trig_loss");
        this.AddReward(0.0f);
        Competitor.AddReward(1.0f);
        this.shared_trig_win_loss();
    }

    public override void AgentReset()
    {
        Debug.Log(this.GetComponentInParent<PingPondAgent>().tag + " reset with reward " + this.GetReward());
        this.force_increasing = 0.0f;

        player.transform.position = player_position;
        //new Vector3(
        //player_position.x + Random.Range(-0.1f, 0.1f),
        //player_position.y,
        //player_position.z + Random.Range(-0.1f, 0.1f));
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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

        Vector3 palyer_velocity = Vector3.zero;

        switch (action)
        {
            case NoAction:
                player.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                break;
            case Forward:
                palyer_velocity.x = -(move_force + force_increasing);
                break;
            case Backward:
                palyer_velocity.x = (move_force + force_increasing);
                break;
            case Right:
                palyer_velocity.z = -(move_force + force_increasing);
                break;
            case Left:
                palyer_velocity.z = (move_force + force_increasing);
                break;
            case Down:
                palyer_velocity.y = -(move_force + force_increasing);
                break;
            case Up:
                palyer_velocity.y = (move_force + force_increasing);
                break;
            default:
                break;
        }

        player.GetComponentInChildren<Rigidbody>().AddForce(
            this.transform.TransformDirection(palyer_velocity)
            );

        last_action = action;



    }
}