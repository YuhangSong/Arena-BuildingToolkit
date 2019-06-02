using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class TennisGlobalManager : GlobalManager
{
    private TransformReinitializor BallReinitializor;

    public override void
    InitializeAcademy()
    {
        base.InitializeAcademy();
        BallReinitializor = new TransformReinitializor(
            GameObject.FindGameObjectWithTag("Ball"),
            new Vector3(4f, 0f, 0f), new Vector3(5f, 0f, 0f),
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero);
        BallReinitializor.Reinitialize();
    }

    public override void
    AcademyReset()
    {
        base.AcademyReset();
        BallReinitializor.Reinitialize();
    }
}
