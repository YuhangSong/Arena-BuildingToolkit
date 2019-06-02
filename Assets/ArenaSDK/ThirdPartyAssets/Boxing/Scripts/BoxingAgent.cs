using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BoxingAgent : Agent
{
    // public reference
    public GameObject player;
    protected Animator anim;
    public BoxingAgent Competitor;
    public BoxingGlobalManager globalManager;
    public BoxingLifeBarController lifebar;
    public BoxingLifeBarController powerbar;

    // priveta conifg
    private const int NoAction = 0;  // do nothing!
    private const int Forward = 1;
    private const int Backward = 2;
    private const int Left = 3;
    private const int Right = 4;
    private const int Hit = 5;
    private const float power_recover_speed = 0.0002f;
    private const float hurting_coefficient = 1.0f;
    private const float move_speed = 1.0f;

    // private status
    private Vector3 player_position;
    private Quaternion player_rotation;
    private float life;
    private float power;

    void Start()
    {
        anim = GetComponent<Animator>();
        player_position = player.transform.position;
        player_rotation = player.transform.rotation;
    }

    public void trig_win()
    {
        Debug.Log(this.tag + " trig_win");
        this.AddReward(1.0f);
        Competitor.AddReward(0.0f);
        Done();
        Competitor.Done();
    }

    public void trig_loss()
    {
        Debug.Log(this.tag + " trig_loss");
        this.AddReward(0.0f);
        Competitor.AddReward(1.0f);
        Done();
        Competitor.Done();
    }

    public override void AgentReset()
    {
        Debug.Log(this.tag + " reset with reward " + this.GetReward());
        globalManager.Reset();
        this.reset_player_position();
        this.life = 1.0f;
        this.power = 1.0f;
        this.lifebar.UpdatePercentage(this.life);
        this.powerbar.UpdatePercentage(this.power);
        anim.SetBool("hit", false);
    }

    protected void reset_player_position()
    {
        Vector3 position_temp = player_position;
        //position_temp.z = player_position.z + Random.Range(-place_range_z, place_range_z);
        //position_temp.x = player_position.x + Random.Range(-place_range_x, place_range_x);
        player.transform.position = position_temp;
        player.transform.rotation = player_rotation;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = Mathf.FloorToInt(vectorAction[0]);

        // if no power, no action
        if (action == Hit)
        {
            if (this.power < 0.2f)
            {
                action = NoAction;
            }
        }
       
        Vector3 palyer_velocity = player.transform.InverseTransformDirection(player.GetComponent<Rigidbody>().velocity);
        palyer_velocity.z = 0;
        palyer_velocity.x = 0;

        // recover power
        this.recover_power(power_recover_speed);

        switch (action)
        {
            case NoAction:
                anim.SetBool("hit", false);
                break;
            case Forward:
                palyer_velocity.x = -move_speed;
                this.tire(power_recover_speed);
                break;
            case Backward:
                palyer_velocity.x = move_speed;
                this.tire(power_recover_speed);
                break;
            case Left:
                palyer_velocity.z = -move_speed;
                this.tire(power_recover_speed);
                break;
            case Right:
                palyer_velocity.z = move_speed;
                this.tire(power_recover_speed);
                break;
            case Hit:
                anim.SetBool("hit", true);
                this.tire(0.01f);
                break;
            default:
                break;
        }
        player.GetComponent<Rigidbody>().velocity = player.transform.TransformDirection(palyer_velocity);

    }

    public void tire(float tiring)
    {
        this.power -= tiring;
        this.powerbar.UpdatePercentage(this.power);
    }

    public void hurt(float hurting)
    {
        if (this.life > 0.0f)
        {
            this.life -= hurting * hurting_coefficient;
            this.lifebar.UpdatePercentage(this.life);
            if (this.life < 0.0f)
            {
                this.trig_loss();
            }
        }
        else
        {
            //Debug.Log("Waiting reset");
        }

    }

    public void recover_power(float power)
    {
        this.power += power;
        if (this.power > 1.0f)
        {
            this.power = 1.0f;
        }
        this.powerbar.UpdatePercentage(this.power);
    }
}
