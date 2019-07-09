using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class BilliardsGlobalManager : GlobalManager
{
    private const int ballTotal = 7;

    public int
    getBallTotal()
    {
        return ballTotal;
    }

    public string
    getAnotherBall(string theBall)
    {
        if (theBall == "BallK") {
            return "BallS";
        } else if (theBall == "BallS") {
            return "BallK";
        } else {
            return null;
        }
    }

    // override protected bool
    // isSwitchingTurn()
    // {
    //     return !Utils.isAllRigidbodySleepingInTags(new string[] { "WhiteBall", "BlackBall", "BallK", "BallS" });
    // }
}
