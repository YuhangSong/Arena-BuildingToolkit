using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RealRaceAgent : Agent
{
    // necessary public reference
    public RealRaceAgent Competitor;
    public RealRaceGlobalManager globalManager;
    // customize public reference

    // priveta conifg: action space
    private const int NoAction = 0;
    private const int Left = 1;
    private const int Right = 2;
    private const int Forward = 3;
    private const int Backward = 4;
    private const int HandBreak = 5;
    // priveta conifg: others
    private const float force_increasing_speed_v = 0.1f;
    private const float force_increasing_speed_h = 0.1f;

    // private status
    private float Vertical;
    private float Horizontal;
    private bool Jump;
    private int CurrentCheckPoint;
    private int LastCheckPoint;

    private string next_stage="none";

    public override void AgentReset()
    {
        Debug.Log(this.GetComponentInParent<RealRaceAgent>().tag + " reset with reward " + this.GetReward());
        this.Vertical = 0.0f;
        this.Horizontal = 0.0f;
        this.Jump = false;
        this.CurrentCheckPoint = 0;
        this.LastCheckPoint = 0;
        globalManager.Reset();
    }

    public void UpdateCurrentCheckPoint(int NewCheckPoint)
    {
        this.CurrentCheckPoint = NewCheckPoint;
    }

    public float GetAxis(string axis)
    {
        if (axis == "Vertical")
        {
            return this.Vertical;
        }
        else if (axis == "Horizontal")
        {
            return Horizontal;
        }
        else
        {
            return 0.0f;
        }
    }

    public bool GetButton(string key)
    {
        if (key == "Jump")
        {
            return this.Jump;
        }
        else
        {
            return false;
        }
    }

    public void receive_signal(string signal){
        // can be called out side step thread
        Debug.Log(this.GetComponentInParent<RealRaceAgent>().tag + " receive_signal "+signal);
        next_stage=signal;
    }

    private void trig_self(string signal){
        // can only be called at step
        Debug.Log(this.GetComponentInParent<RealRaceAgent>().tag + " trig_self "+signal);
        if (signal=="tie"){
            SetReward(0f);
            Done();
            Competitor.receive_signal("tie");
        }else if (signal=="win"){
            SetReward(1f);
            Done();
            Competitor.receive_signal("lost");
        }else if (signal=="lost"){
            SetReward(0f);
            Done();
            Competitor.receive_signal("win");
        }else{
            Debug.Log("Wrong signal: "+signal);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        this.SetReward(0f);

        if (next_stage=="none"){
            if (this.CurrentCheckPoint!=this.LastCheckPoint)
            {
                // if CurrentCheckPoint has been updated
                if (this.LastCheckPoint == (this.globalManager.GetNumCheckPoints() - 1))
                {
                    // if next checkpoint should be 0
                    if (this.CurrentCheckPoint == 0)
                    {
                        // run into right checkpoint and win
                        this.trig_self("win");
                    }
                    else
                    {
                        // run into wrong checkpoint
                        this.trig_self("loss");
                    }
                }
                else
                {
                    // if next checkpoint should be LastCheckPoint+1
                    if (this.CurrentCheckPoint == (this.LastCheckPoint + 1))
                    {
                        // run into right checkpoint
                        Debug.Log(this.tag + " CurrentCheckPoint update to " + this.CurrentCheckPoint);
                        this.SetReward(1.0f / (this.globalManager.GetNumCheckPoints() - 1.0f));
                    }
                    else
                    {
                        // run into wrong checkpoint
                        this.trig_self("loss");
                    }
                }
                this.LastCheckPoint = this.CurrentCheckPoint;
            }
        }else if (next_stage=="win"){
            SetReward(1f);
            Done();
        }else if (next_stage=="lost"){
            SetReward(0f);
            Done();
        }else{
            Debug.Log("Wrong signal: "+next_stage);
        }
        next_stage="none";

        int action = Mathf.FloorToInt(vectorAction[0]);

        switch (action)
        {
            case NoAction:
                this.Vertical = 0.0f;
                this.Horizontal = 0.0f;
                break;
            case Backward:
                if (this.Vertical > 0.0f){ this.Vertical = 0.0f; }
                this.Vertical -= force_increasing_speed_v;
                this.Horizontal = 0.0f;
                break;
            case Forward:
                if (this.Vertical < 0.0f) { this.Vertical = 0.0f; }
                this.Vertical += force_increasing_speed_v;
                this.Horizontal = 0.0f;
                break;
            case Left:
                if (this.Horizontal > 0.0f) { this.Horizontal = 0.0f; }
                this.Horizontal -= force_increasing_speed_h;
                break;
            case Right:
                if (this.Horizontal < 0.0f) { this.Horizontal = 0.0f; }
                this.Horizontal += force_increasing_speed_h;
                break;
            default:
                break;
        }

        if (action == HandBreak)
        {
            this.Jump = true;
        }
        else
        {
            this.Jump = false;
        }

        this.Vertical = Mathf.Clamp(this.Vertical, -1.0f, 1.0f);
        this.Horizontal = Mathf.Clamp(this.Horizontal, -1.0f, 1.0f);

        if (this.GetReward() > 0)
        {
            Debug.Log(this.tag + " step with reward " + this.GetReward());
        }
    }
}
