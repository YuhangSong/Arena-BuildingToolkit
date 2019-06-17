using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;
using Arena;

public class ArenaTennisAgent : ArenaAgent
{
    public float MoveForce;
    public GameObject Player;
    private TransformReinitializor PlayerReinitializor;

    public override void
    InitializeAgent()
    {
        base.InitializeAgent();
        PlayerReinitializor = new TransformReinitializor(
            Player,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero);
    }

    public override void
    AgentReset()
    {
        base.AgentReset();
        PlayerReinitializor.Reinitialize();
    }

    override protected void
    DiscreteStep(int Action_)
    {
        base.DiscreteStep(Action_);
        switch (Action_) {
            case NoAction:
                Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                break;
            case Left:
                Player.GetComponent<Rigidbody>().AddForce(Player.transform.TransformVector(new Vector3(-MoveForce, 0,
                  0)));
                break;
            case Right:
                Player.GetComponent<Rigidbody>().AddForce(Player.transform.TransformVector(new Vector3(MoveForce, 0,
                  0)));
                break;
            default:
                break;
        }
    }
}
