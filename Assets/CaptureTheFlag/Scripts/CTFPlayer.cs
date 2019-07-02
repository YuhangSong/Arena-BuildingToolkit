using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class CTFPlayer : MonoBehaviour
{
    public string TrigTag = "Capturedflag";

    private bool carryingflag;

    void
    Start()
    {
        carryingflag = false;
    }

    public bool hasflag(){
        return carryingflag;
    }

    private void setflag(int teamid){
        if (GetComponentInParent<ArenaTeam>().getTeamID() != teamid){
            this.GetComponentInChildren<CTFCarryFlag>().getcolor(teamid);
            this.GetComponentInChildren<CTFCarryFlag>().showflag();
            carryingflag = true;
        }
    }

    void
    OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(TrigTag)) {
            // ArenaAgent ArenaAgent_ = other.gameObject.GetComponentInParent<ArenaAgent>();
            CTFFlag curflag = other.gameObject.GetComponentInParent<CTFFlag>();
            this.setflag(curflag.Tellteamid());
        }
    }
}
