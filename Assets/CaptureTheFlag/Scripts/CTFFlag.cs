using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class CTFFlag : MonoBehaviour
{
    void
    Start()
    {
        GetComponentInParent<GlobalManager>().ApplyTeamMaterial(
            GetComponentInParent<ArenaTeam>().getTeamID(),
            this.gameObject
        );
    }

    public int Tellteamid(){
        return GetComponentInParent<ArenaTeam>().getTeamID();
    }
}
