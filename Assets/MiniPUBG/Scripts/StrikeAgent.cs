using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class StrikeAgent : Agent
{
    // necessary public reference
    public StrikeAgent Competitor;
    public StrikeGlobalManager globalManager;
    // customize public reference
    public TankLifeBarController BulletBar;
    public TankLifeBarController LifeBar;

    public GameObject player;

    // priveta conifg: action space
    private const int NoAction = 0;
    private const int Left = 1;
    private const int Right = 2;
    private const int Forward = 3;
    private const int Backward = 4;
    private const int ActionJump = 5;
    private const int ActionCrouch = 6;
    private const int ActionAim = 7;
    private const int ActionFire = 8;
    private const int ActionRun = 9;
    private const int MouseXInc = 10;
    private const int MouseXDec = 11;
    private const int MouseYInc = 12;
    private const int MouseYDec = 13;
    private const int ActionGunInc = 14;
    private const int ActionGunDec = 15;
    private const int ActionReload = 16;
    private const int ActionAttack = 17;

    // priveta conifg: others
    private const float increasing_speed_v = 0.1f;
    private const float increasing_speed_h = 0.1f;
    private const float increasing_speed_x = 0.01f;
    private const float increasing_speed_y = 0.01f;
    private const float limit_speed_v = 1f;
    private const float limit_speed_h = 1f;
    private const float limit_speed_x = 2f;
    private const float limit_speed_y = 2f;
    private const float life_total = 3f;

    // private status
    private float Vertical;
    private float Horizontal;
    private float MouseX;
    private float MouseY;
    private bool Jump;
    private bool Crouch;
    private bool Aim;
    private bool Fire;
    private bool PreviousFire;
    private bool Run;
    private bool PreviousRun;
    private bool GunInc;
    private bool PreviousGunInc;
    private bool GunDec;
    private bool PreviousGunDec;
    private bool Reload;
    private bool PreviousReload;
    private bool Attack;
    private bool PreviousAttack;
    private float life;

    public override void AgentReset()
    {
        Debug.Log(this.GetComponentInParent<StrikeAgent>().tag + " reset with reward " + this.GetReward());
        this.Vertical = 0.0f;
        this.Horizontal = 0.0f;
        this.MouseX = 0.0f;
        this.MouseY = 0.0f;
        this.Jump = false;
        this.Crouch = false;
        this.Aim = false;
        this.Fire = false;
        this.PreviousFire = false;
        this.Run = false;
        this.PreviousRun = false;
        this.GunInc = false;
        this.PreviousGunInc = false;
        this.GunDec = false;
        this.PreviousGunDec = false;
        this.Reload = false;
        this.PreviousReload = false;
        this.Attack = false;
        this.PreviousAttack = false;

        reward_to_add = 0f;
        to_done = false;

        this.life = life_total;

        this.UpdateLifeBar();
    }

    public void hurt(float hurting)
    {
        if (this.life > 0)
        {
            this.life -= hurting;
            this.UpdateLifeBar();
            if (this.life <= 0f)
            {
                this.trig_loss();
            }
        }
    }

    private void UpdateLifeBar()
    {
        this.LifeBar.UpdatePercentage(
            this.life/life_total
            );
    }

    public float GetAxis(string axis)
    {
        if (axis == "Vertical")
        {
            return this.Vertical;
        }
        else if (axis == "Horizontal")
        {
            return this.Horizontal;
        }
        if (axis == "Mouse X")
        {
            return this.MouseX;
        }
        else if (axis == "Mouse Y")
        {
            return this.MouseY;
        }
        else
        {
            return 0.0f;
        }
    }

    public bool GetButtonDown(string key)
    {
        bool to_return=false;

        if (key == "Fire")
        {
            if ((!this.PreviousFire) && this.Fire)
            {
                to_return = true;
            }
            this.PreviousFire = this.Fire;
        }
        else if (key == "Run")
        {
            if ((!this.PreviousRun) && this.Run)
            {
                to_return = true;
            }
            this.PreviousRun = this.Run;
        }
        else if (key == "GunInc")
        {
            if ((!this.PreviousGunInc) && this.GunInc)
            {
                to_return = true;
            }
            this.PreviousGunInc = this.GunInc;
        }
        else if (key == "GunDec")
        {
            if ((!this.PreviousGunDec) && this.GunDec)
            {
                to_return = true;
            }
            this.PreviousGunDec = this.GunDec;
        }
        else if (key == "Reload")
        {
            if ((!this.PreviousReload) && this.Reload)
            {
                to_return = true;
            }
            this.PreviousReload = this.Reload;
        }
        else if (key == "Attack")
        {
            if ((!this.PreviousAttack) && this.Attack)
            {
                to_return = true;
            }
            this.PreviousAttack = this.Attack;
        }

        return to_return;
    }

    public bool GetButton(string key)
    {
        if (key == "Jump")
        {
            return this.Jump;
        }else if (key == "Crouch")
        {
            return this.Crouch;
        }
        else if (key == "Aim")
        {
            return this.Aim;
        }
        else if (key == "Fire")
        {
            return this.Fire;
        }
        else if (key == "Run")
        {
            return this.Run;
        }
        else
        {
            return false;
        }
    }

    void Start()
    {
    }

    private void shared_trig_win_loss()
    {
        // send done signal to global
        globalManager.Reset();
    }

    private float reward_to_add;
    private bool to_done;

    public void TrigSetRewardDone(float reward)
    {
        reward_to_add = reward;
        to_done = true;
    }

    public void trig_tie()
    {
        Debug.Log(this.GetComponentInParent<StrikeAgent>().tag + " trig_tie");
        TrigSetRewardDone(0.0f);
        Competitor.TrigSetRewardDone(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_win()
    {
        Debug.Log(this.GetComponentInParent<StrikeAgent>().tag + " trig_win");
        TrigSetRewardDone(1.0f);
        Competitor.TrigSetRewardDone(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_loss()
    {
        Debug.Log(this.GetComponentInParent<StrikeAgent>().tag + " trig_loss");
        TrigSetRewardDone(0.0f);
        Competitor.TrigSetRewardDone(1.0f);
        this.shared_trig_win_loss();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if (reward_to_add != 0f)
        {
            SetReward(reward_to_add);
            reward_to_add = 0f;
        }
        else
        {
            SetReward(0f);
        }

        if (to_done)
        {
            Done();
            to_done = false;
        }

        int action = Mathf.FloorToInt(vectorAction[0]);

        switch (action)
        {
            case NoAction:
                this.Vertical = 0.0f;
                this.Horizontal = 0.0f;
                this.MouseX = 0.0f;
                this.MouseY = 0.0f;
                this.Jump = false;
                this.Fire = false;
                this.Run = false;
                this.GunInc = false;
                this.GunDec = false;
                this.Reload = false;
                this.Attack = false;
                break;
            case Backward:
                if (this.Vertical > 0.0f) { this.Vertical = 0.0f; }
                this.Vertical -= increasing_speed_v;
                break;
            case Forward:
                if (this.Vertical < 0.0f) { this.Vertical = 0.0f; }
                this.Vertical += increasing_speed_v;
                break;
            case Left:
                if (this.Horizontal > 0.0f) { this.Horizontal = 0.0f; }
                this.Horizontal -= increasing_speed_h;
                break;
            case Right:
                if (this.Horizontal < 0.0f) { this.Horizontal = 0.0f; }
                this.Horizontal += increasing_speed_h;
                break;
            case MouseXInc:
                if (this.MouseX < 0.0f) { this.MouseX = 0.0f; }
                this.MouseX += increasing_speed_x;
                break;
            case MouseXDec:
                if (this.MouseX > 0.0f) { this.MouseX = 0.0f; }
                this.MouseX -= increasing_speed_x;
                break;
            case MouseYInc:
                if (this.MouseY < 0.0f) { this.MouseY = 0.0f; }
                this.MouseY += increasing_speed_y;
                break;
            case MouseYDec:
                if (this.MouseY > 0.0f) { this.MouseY = 0.0f; }
                this.MouseY -= increasing_speed_y;
                break;
            case ActionJump:
                this.Jump = true;
                break;
            case ActionCrouch:
                this.Crouch = !this.Crouch;
                break;
            case ActionAim:
                this.Aim = !this.Aim;
                break;
            case ActionFire:
                this.UpdateBulletBar();
                this.Fire = true;
                break;
            case ActionRun:
                this.Run = true;
                break;
            case ActionGunInc:
                this.GunInc = true;
                break;
            case ActionGunDec:
                this.GunDec = true;
                break;
            case ActionReload:
                this.Reload = true;
                break;
            case ActionAttack:
                this.Attack = true;
                break;
            default:
                break;
        }

        if (action !=MouseXInc && action != MouseXDec)
        {
            this.MouseX = 0f;
        }
        if (action != MouseYInc && action != MouseYDec)
        {
            this.MouseY = 0f;
        }

        this.Vertical = Mathf.Clamp(this.Vertical, -limit_speed_v, limit_speed_v);
        this.Horizontal = Mathf.Clamp(this.Horizontal, -limit_speed_h, limit_speed_h);
        this.MouseX = Mathf.Clamp(this.MouseX, -limit_speed_x, limit_speed_x);
        this.MouseY = Mathf.Clamp(this.MouseY, -limit_speed_y, limit_speed_y);

        if (this.GetReward() > 0)
        {
            Debug.Log(this.tag + " step with reward " + this.GetReward());
        }

        this.globalManager.tick();
    }

    private void UpdateBulletBar()
    {
        this.BulletBar.UpdatePercentage(
            gameObject.GetComponentInChildren<GunScript>().bulletsInTheGun / gameObject.GetComponentInChildren<GunScript>().amountOfBulletsPerLoad
            );
    }
}
