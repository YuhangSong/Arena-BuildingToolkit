using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Arena;

namespace Arena
{
    public class KickBoxingAgent : BasicAgent
    {
        protected Animator anim;
        // public TankLifeBarController lifebar;
        // public TankLifeBarController powerbar;

        // priveta conifg
        new protected const int Forward = 1;
        new private const int Backward  = 2;
        private const int hp_straight       = 3;
        private const int hp_straight_right = 4;
        private const int ho_hook_left      = 5;
        private const int hp_upper_right    = 6;
        private const int hk_side_left      = 7;
        private const int hk_rh_right       = 8;
        private const int bp_upper_left     = 9;
        private const int bp_hook_right     = 10;
        private const int bk_push_left      = 11;
        private const int bk_rh_right       = 12;
        private const int lk_rh_left        = 13;
        private const int lk_rh_right       = 14;
        private const int hd_front = 15;
        private const int bd_front = 16;
        private const int ld_front = 17;

        private const float power_recover_speed = 0.005f;
        private const float hurting_coefficient = 1.1f;

        private float life;
        private float power;

        public override void
        InitializeAgent()
        {
            base.InitializeAgent();
            anim = GetComponent<Animator>();
        }

        public override void
        AgentReset()
        {
            base.AgentReset();
            this.life  = 1.0f;
            this.power = 1.0f;
            // this.lifebar.UpdatePercentage(this.life);
            // this.powerbar.UpdatePercentage(this.power);
        }

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);
            // if no power, no action
            if (this.power < 0.0f) {
                Action_ = NoAction;
            }

            // recover power
            this.recover_power(power_recover_speed);

            switch (Action_) {
                case NoAction:
                    anim.SetBool("move_forward", false);
                    anim.SetBool("move_backward", false);
                    anim.SetBool("hp_straight", false);
                    anim.SetBool("hp_straight_right", false);
                    anim.SetBool("ho_hook_left", false);
                    anim.SetBool("hp_upper_right", false);
                    anim.SetBool("hk_side_left", false);
                    anim.SetBool("hk_rh_right", false);
                    anim.SetBool("bp_upper_left", false);
                    anim.SetBool("bp_hook_right", false);
                    anim.SetBool("bk_push_left", false);
                    anim.SetBool("bk_rh_right", false);
                    anim.SetBool("lk_rh_left", false);
                    anim.SetBool("lk_rh_right", false);
                    anim.SetBool("hd_front", false);
                    anim.SetBool("bd_front", false);
                    anim.SetBool("ld_front", false);
                    break;
                case Forward:
                    anim.SetBool("move_forward", true);
                    this.tire(power_recover_speed);
                    break;
                case Backward:
                    anim.SetBool("move_backward", true);
                    this.tire(power_recover_speed);
                    break;
                case hp_straight:
                    anim.SetBool("hp_straight", true);
                    this.tire(power_recover_speed * 1.2f);
                    break;
                case hp_straight_right:
                    anim.SetBool("hp_straight_right", true);
                    this.tire(power_recover_speed * 1.2f);
                    break;
                case ho_hook_left:
                    anim.SetBool("ho_hook_left", true);
                    this.tire(power_recover_speed * 1.8f);
                    break;
                case hp_upper_right:
                    anim.SetBool("hp_upper_right", true);
                    this.tire(power_recover_speed * 1.4f);
                    break;
                case hk_side_left:
                    anim.SetBool("hk_side_left", true);
                    this.tire(power_recover_speed * 170f);
                    break;
                case hk_rh_right:
                    anim.SetBool("hk_rh_right", true);
                    this.tire(power_recover_speed * 140f);
                    break;
                case bp_upper_left:
                    anim.SetBool("bp_upper_left", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case bp_hook_right:
                    anim.SetBool("bp_hook_right", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case bk_push_left:
                    anim.SetBool("bk_push_left", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case bk_rh_right:
                    anim.SetBool("bk_rh_right", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case ld_front:
                    anim.SetBool("ld_front", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case lk_rh_right:
                    anim.SetBool("lk_rh_right", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case hd_front:
                    anim.SetBool("hd_front", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case bd_front:
                    anim.SetBool("bd_front", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                case lk_rh_left:
                    anim.SetBool("lk_rh_left", true);
                    this.tire(power_recover_speed * 14f);
                    break;
                default:
                    break;
            }
        } // AgentAction

        public void
        tire(float tiring)
        {
            this.power -= tiring;
            // this.powerbar.UpdatePercentage(this.power);
        }

        public void
        hurt(float hurting)
        {
            this.life -= hurting * hurting_coefficient;
            // this.lifebar.UpdatePercentage(this.life);
            if (this.life < 0.0f) {
                globalManager.KillAgent(getTeamID(), getAgentID());
            }
        }

        public void
        recover_power(float power)
        {
            this.power += power;
            if (this.power > 1.0f) {
                this.power = 1.0f;
            }
            // this.powerbar.UpdatePercentage(this.power);
        }
    }
}
