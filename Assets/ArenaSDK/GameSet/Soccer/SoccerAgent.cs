using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class SoccerAgent : ArenaAgent
{
    public GameObject Player;

    protected const int KickLeft  = CustomizeActionStartAt;
    protected const int KickRight = CustomizeActionStartAt + 1;

    private TransformReinitializor PlayerReinitializor;

    private float TurnForce = 3f;
    private float MoveForce = 50f;

    public override void
    InitializeAgent()
    {
        base.InitializeAgent();
        base.ApplyTeamMaterial(Player);
        PlayerReinitializor = new TransformReinitializor(
            Player,
            Vector3.zero, new Vector3(0.8f, 0f, 1f),
            Vector3.zero, new Vector3(0f, 20f, 0f),
            Vector3.zero, Vector3.zero);
        PlayerReinitializor.Reinitialize();
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

        if ((Action_ != TurnLeft) && (Action_ != TurnRight)) {
            Player.GetComponentInChildren<Rigidbody>().angularVelocity = Vector3.zero;
        }

        if ((Action_ != Forward) && (Action_ != Backward)) {
            Player.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
        }

        switch (Action_) {
            case NoAction:
                break;
            case TurnLeft:
                Player.GetComponentInChildren<Rigidbody>().AddTorque(new Vector3(0.0f, -TurnForce, 0.0f));
                break;
            case TurnRight:
                Player.GetComponentInChildren<Rigidbody>().AddTorque(new Vector3(0.0f, TurnForce, 0.0f));
                break;
            case Left:
                Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(new Vector3(0, 0,
                  MoveForce * 10f)));
                break;
            case Right:
                Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(new Vector3(0, 0,
                  -MoveForce * 10f)));
                break;
            case Forward:
                Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(new Vector3(
                      MoveForce, 0, 0)));
                break;
            case Backward:
                Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(new Vector3(-
                  MoveForce, 0, 0)));
                break;
            default:
                break;
        }
    } // Step
}
