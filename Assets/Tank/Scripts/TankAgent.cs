using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class TankAgent : Agent
{

    // public reference
    public GameObject player;
    public GameObject Bullet_Emitter;
    public GameObject Bullet;
    public TankAgent Competitor;
    public TankGlobalManager globalManager;
    public TankLifeBarController bulletBar;

    // priveta conifg
    private const int NoAction = 0;  // do nothing!
    private const int Forward = 1;
    private const int Backward = 2;
    private const int TurnLeft = 3;
    private const int TurnRight = 4;
    private const int Fire = 5;

    private float move_speed = 10.0f;
    private float bullet_speed = 500.0f;
    private float turning_speed = 1.0f;
    private const float num_bullet_per_load = 0.03f;
    private const float full_num_bullet = 1.0f;
    private const float place_range_z = 15.0f;
    private const float place_range_x = 10.0f;

    // private status
    private Vector3 player_position;
    private Quaternion player_rotation;
    private float num_bullet = full_num_bullet;
    private bool reloading = false;

    void Start()
    {
        player_position = player.transform.position;
        player_rotation = player.transform.rotation;
    }

    public void trig_win()
    {
        Debug.Log(this.tag + " trig win");
        this.AddReward(1.0f);
        Competitor.AddReward(0.0f);
        Done();
        Competitor.Done();
    }

    public void trig_loss()
    {
        Debug.Log(this.tag + " trig loss");
        this.AddReward(0.0f);
        Competitor.AddReward(1.0f);
        Done();
        Competitor.Done();
    }

    public override void AgentReset()
    {
        Debug.Log(this.tag + " reset with reward " + this.GetReward());
        // globalManager.Reset();
        this.reset_player_position();
    }

    protected void reset_player_position()
    {
        Vector3 position_temp = player_position;
        //position_temp.z = player_position.z + Random.Range(-place_range_z, place_range_z);
        //position_temp.x = player_position.x + Random.Range(-place_range_x, place_range_x);
        player.transform.position = position_temp;
        player.transform.rotation = player_rotation;
    }

    private void update_bullet_bar()
    {
        bulletBar.UpdatePercentage((float)this.num_bullet / (float)full_num_bullet);

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = Mathf.FloorToInt(vectorAction[0]);

        Vector3 palyer_velocity = player.transform.InverseTransformDirection(player.GetComponent<Rigidbody>().velocity);

        palyer_velocity.z = 0;
        palyer_velocity.x = 0;

        switch (action)
        {
            case NoAction:
                break;
            case Forward:
                palyer_velocity.x = -move_speed;
                break;
            case Backward:
                palyer_velocity.x = move_speed;
                break;
            case Fire:
                if ((this.num_bullet > 0) && !this.reloading)
                {
                    GameObject Temp_Bullet_Handeler;
                    Temp_Bullet_Handeler = Instantiate(Bullet, Bullet_Emitter.transform.position, Bullet_Emitter.transform.rotation) as GameObject;
                    Temp_Bullet_Handeler.GetComponent<Rigidbody>().AddForce(Bullet_Emitter.transform.up * bullet_speed);
                    this.num_bullet -= 1.0f;
                    this.update_bullet_bar();
                    if (this.num_bullet < 1.0f)
                    {
                        this.reloading = true;
                    }
                }
                break;
            case TurnLeft:
                player.transform.Rotate(0.0f, -this.turning_speed, 0.0f);
                break;
            case TurnRight:
                player.transform.Rotate(0.0f, this.turning_speed, 0.0f);
                break;
            default:
                break;
        }
        player.GetComponent<Rigidbody>().velocity = player.transform.TransformDirection(palyer_velocity);

        if (this.reloading){
            this.num_bullet += num_bullet_per_load;
            this.update_bullet_bar();
            if (this.num_bullet >= full_num_bullet)
            {
                this.reloading = false;
            }
        }
    }
}
