using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using Arena;

public class TennisGateController : MonoBehaviour
{
    void
    OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball")) {
            GetComponentInParent<GlobalManager>().KillTeam(
                GetComponentInParent<ArenaTennisAgent>().getTeamID()
            );
        }
    }
}
