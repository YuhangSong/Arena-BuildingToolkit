using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;
using Arena;

public class Lifter_Agent : ArenaAgent
{
    public float MoveForce;
    public GameObject Arm1;
    public GameObject Arm2;
    private TransformReinitializor Arm1Reinitializor;
    private TransformReinitializor Arm2Reinitializor;

    public enum MoveTypes {
        Torque,
        AngularVelocity
    }
    public MoveTypes MoveType       = MoveTypes.Torque;
    public bool isApplyTeamMaterial = true;

    public override void
    InitializeAgent()
    {
        base.InitializeAgent();
        if (isApplyTeamMaterial) {
            base.ApplyTeamMaterial(Arm1);
            base.ApplyTeamMaterial(Arm2);
        }
        Arm1Reinitializor = new TransformReinitializor(
            Arm1,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero);
        Arm2Reinitializor = new TransformReinitializor(
            Arm2,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero);
    }

    public override void
    AgentReset()
    {
        base.AgentReset();
        Arm1Reinitializor.Reinitialize();
        Arm2Reinitializor.Reinitialize();
    }

    override protected void
    DiscreteStep(int Action_)
    {
        if (MoveType == MoveTypes.Torque) {
            // // torque control
            switch (Action_) {
                case NoAction:
                    Arm1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    Arm2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    break;
                case Left:
                    Arm1.GetComponent<Rigidbody>().AddTorque(Arm1.transform.TransformVector(new Vector3(6 * MoveForce,
                      0, 0)));
                    break;
                case Right:
                    Arm1.GetComponent<Rigidbody>().AddTorque(Arm1.transform.TransformVector(new Vector3(-6 * MoveForce,
                      0, 0)));
                    break;
                case Forward:
                    // default axis for joint anchor is always x-axis, even if the object is rotated
                    Arm2.GetComponent<Rigidbody>().AddTorque(Arm2.transform.TransformVector(new Vector3(3 * MoveForce,
                      0, 0)));
                    break;
                case Backward:
                    Arm2.GetComponent<Rigidbody>().AddTorque(Arm2.transform.TransformVector(new Vector3(-3 * MoveForce,
                      0, 0)));
                    break;
                default:
                    break;
            }
        } else if (MoveType == MoveTypes.AngularVelocity) {
            // angular velocity control
            switch (Action_) {
                case NoAction:
                    Arm1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    Arm2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    break;
                case Left:
                    Arm1.GetComponent<Rigidbody>().angularVelocity = new Vector3(MoveForce, 0, 0);
                    break;
                case Right:
                    Arm1.GetComponent<Rigidbody>().angularVelocity = new Vector3(-MoveForce, 0, 0);
                    break;
                case Forward:
                    // default axis for joint anchor is always x-axis, even if the object is rotated
                    Arm2.GetComponent<Rigidbody>().angularVelocity = new Vector3(MoveForce, 0, 0);
                    break;
                case Backward:
                    Arm2.GetComponent<Rigidbody>().angularVelocity = new Vector3(-MoveForce, 0, 0);
                    break;
                default:
                    break;
            }
        }
    } // Step
}
