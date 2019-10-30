using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Arena;

namespace Arena
{
    public class RollingAgent : BasicAgent
    {
        override protected void
        CheckPlayerRotationSettings()
        { }

        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);
            switch (Action_) {
                case Backward:
                    Player.GetComponentInChildren<Rigidbody>().AddForce((new Vector3(0, 0,
                      -MoveAccumulator.getCurrent())));
                    break;
                case Forward:
                    Player.GetComponentInChildren<Rigidbody>().AddForce((new Vector3(0, 0,
                      MoveAccumulator.getCurrent())));
                    break;
                case Right:
                    Player.GetComponentInChildren<Rigidbody>().AddForce((new Vector3(
                          MoveAccumulator.getCurrent(), 0, 0)));
                    break;
                case Left:
                    Player.GetComponentInChildren<Rigidbody>().AddForce((new Vector3(
                          -MoveAccumulator.getCurrent(), 0, 0)));
                    break;
                default:
                    break;
            }
        }
    }
}
