using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Arena;

public class BilliardsAgent : ArenaAgent
{
    public GameObject Player;
    public GameObject AimmingLine;
    public TurnBar TurnBar;
    public GameObject BallKLable;
    public GameObject BallSLable;
    public GameObject StickFront;
    public GameObject StickBack;
    private GameObject WhiteBall;

    private const float InitReleaseVelocity  = 2.0f;
    private const float ForceDisplayOnPlayer = 0.009f;
    private const float ForceCoefficient     = 5f;
    private const float ReleaseForceMax      = 5f;

    private float ReleaseVelocity;
    private string MyBall;
    private int MyScore;
    private int MyPreviousStepScore;

    private TransformReinitializor PlayerReinitializor;
    private Accumulator MoveForceAccumulator;

    public override void
    InitializeAgent()
    {
        base.InitializeAgent();
        PlayerReinitializor = new TransformReinitializor(
            Player,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero);
        MoveForceAccumulator = new Accumulator(0.02f, 0.1f);
        Player_ignore("BlackBall");
        Player_ignore("BallS");
        Player_ignore("BallK");
        Player_ignore("Table");
        Player_ignore("Table");
        Player_ignore("PlayerWall");
        Player_ignore("Gate");
        Player_ignore("Player");
        Player_ignore("ground");
        WhiteBall = GameObject.FindGameObjectWithTag("WhiteBall");
    }

    private void
    Player_ignore(string tagger)
    {
        foreach (GameObject each in GameObject.FindGameObjectsWithTag(tagger)) {
            Physics.IgnoreCollision(Player.GetComponent<Collider>(), each.GetComponent<Collider>());
        }
    }

    public void
    addMyScore()
    {
        MyScore++;
        globalManager.KeepATurn();
    }

    public void
    ResetMyScore()
    {
        MyScore = 0;
        MyPreviousStepScore = 0;
    }

    public int
    getMyScore()
    {
        return MyScore;
    }

    public void
    setMyBall(string MyBall_)
    {
        MyBall = MyBall_;
        if (MyBall == "BallK") {
            BallKLable.GetComponent<Renderer>().enabled = true;
            BallSLable.GetComponent<Renderer>().enabled = false;
        } else if (MyBall == "BallS") {
            BallSLable.GetComponent<Renderer>().enabled = true;
            BallKLable.GetComponent<Renderer>().enabled = false;
        } else if (MyBall == "None") {
            BallSLable.GetComponent<Renderer>().enabled = true;
            BallKLable.GetComponent<Renderer>().enabled = true;
        }
    }

    public string
    getMyBall()
    {
        return MyBall;
    }

    public override void
    AgentReset()
    {
        base.AgentReset();
        MoveForceAccumulator.Reset();
        PlayerReinitializor.Reinitialize();
        setMyBall("None");
        ResetMyScore();
        ReleaseVelocity = InitReleaseVelocity;
        AimmingLine.GetComponent<Renderer>().enabled = false;
    }

    private void
    Release()
    {
        Vector3 ReleaseForce = Vector3.forward * ReleaseVelocity * ForceCoefficient;

        Player.GetComponent<Rigidbody>().AddForce(Player.transform.TransformVector(ReleaseForce));
    }

    override protected void
    StepSwitchingOrNotTurn()
    {
        AimmingLine.GetComponent<Renderer>().enabled = false;
    }

    override protected void
    StepTurn()
    {
        base.StepTurn();
        AimmingLine.GetComponent<Renderer>().enabled = true;
        Player.transform.position = new Vector3(
            Player.transform.position.x,
            WhiteBall.transform.position.y,
            Player.transform.position.z
        );
        Vector3 Baseline = (StickBack.transform.position - StickFront.transform.position);
        //                          ball                           StickFront                        StickBack  additional
        Player.transform.position = WhiteBall.transform.position + Vector3.Normalize(Baseline) * 0.4f + Baseline
          + Vector3.Normalize(Baseline) * (ReleaseVelocity - InitReleaseVelocity);
    }

    override protected int
    CheckHistoryAction(int LastAction_, int Action_)
    {
        Action_ = base.CheckHistoryAction(LastAction_, Action_);
        if (((LastAction_ == Left) && (Action_ == Left)) || ((LastAction_ == Right) && (Action_ == Right))) {
            MoveForceAccumulator.Accumulate();
        } else {
            MoveForceAccumulator.Reset();
        }
        if ((LastAction_ == Hit) && (Action_ == Hit)) {
            Action_ = NoAction;
        }
        return Action_;
    }

    override protected void
    DiscreteStep(int Action_)
    {
        base.DiscreteStep(Action_);

        if (globalManager.getTurnState(getTeamID(), getAgentID()) == TurnStates.Turn) {
            TurnBar.UpdatePercentage(globalManager.getTurnPercentage());
            UpdateCanvas();
        }

        switch (Action_) {
            case NoAction:
                break;
            case Backward:
                ReleaseVelocity += 0.1f;
                if (ReleaseVelocity > ReleaseForceMax) {
                    ReleaseVelocity = ReleaseForceMax;
                }
                break;
            case Left:
                Player.transform.RotateAround(GameObject.FindGameObjectWithTag(
                      "WhiteBall").transform.position, Vector3.up, -MoveForceAccumulator.getCurrent());
                break;
            case Right:
                Player.transform.RotateAround(GameObject.FindGameObjectWithTag(
                      "WhiteBall").transform.position, Vector3.up, MoveForceAccumulator.getCurrent());
                break;
            case Hit:
                Release();
                globalManager.EndATurn();
                break;
            default:
                break;
        }

        AddReward(MyScore - MyPreviousStepScore);
        MyPreviousStepScore = MyScore;

        if (GetReward() > 0) {
            Debug.Log(tag + " step with reward " + GetReward());
        }
    } // Step
}
