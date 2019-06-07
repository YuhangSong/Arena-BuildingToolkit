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
            this.GetComponentInChildren<CarryFlag>().getcolor(teamid);
            this.GetComponentInChildren<CarryFlag>().showflag();
            carryingflag = true;
        }
    }

    void
    OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(TrigTag)) {
            // ArenaAgent ArenaAgent_ = other.gameObject.GetComponentInParent<ArenaAgent>();
            Flag curflag = other.gameObject.GetComponentInParent<Flag>();
            this.setflag(curflag.Tellteamid());
        }
    }
}
