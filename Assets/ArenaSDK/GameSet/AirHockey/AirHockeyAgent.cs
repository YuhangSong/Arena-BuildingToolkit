using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena
{
    public class AirHockeyAgent : BasicAgent {
        public override void
        InitializeAgent()
        {
            base.InitializeAgent();
            Utils.IgnoreCollision(Player, "Table");
            Utils.IgnoreCollision(Player, "Gate");
        }

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);
            switch (Action_) {
                case NoAction:
                    Player.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                    break;
                default:
                    break;
            }
        }
    }
}
