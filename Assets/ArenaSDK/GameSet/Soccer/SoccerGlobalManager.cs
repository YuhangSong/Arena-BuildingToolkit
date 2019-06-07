using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class SoccerGlobalManager : GlobalManager
{
    private TransformReinitializor BallReinitializor;

    public override void
    InitializeAcademy()
    {
        base.InitializeAcademy();
        BallReinitializor = new TransformReinitializor(
            GameObject.FindGameObjectWithTag("Ball"),
            Vector3.zero, new Vector3(0.4f, 0f, 0.4f),
            Vector3.zero, Vector3.zero,
            Vector3.zero, new Vector3(10f, 0f, 10f));
        BallReinitializor.Reinitialize();
        Utils.IgnoreCollision("BallWall", "Player");
    }

    public override void
    AcademyReset()
    {
        base.AcademyReset();
        BallReinitializor.Reinitialize();
    }
}
