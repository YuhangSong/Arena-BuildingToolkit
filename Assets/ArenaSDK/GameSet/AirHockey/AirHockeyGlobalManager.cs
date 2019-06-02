using UnityEngine;
using Arena;

public class AirHockeyGlobalManager : GlobalManager {
    private TransformReinitializor BallReinitializor;

    public override void
    InitializeAcademy()
    {
        base.InitializeAcademy();
        Utils.IgnoreCollision("PlayerWall", "Ball");
        BallReinitializor = new TransformReinitializor(
            GameObject.FindGameObjectWithTag("Ball"),
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero,
            Vector3.zero, new Vector3(50f, 0f, 50f));
        BallReinitializor.Reinitialize();
    }

    public override void
    AcademyReset()
    {
        base.AcademyReset();
        BallReinitializor.Reinitialize();
    }
}
