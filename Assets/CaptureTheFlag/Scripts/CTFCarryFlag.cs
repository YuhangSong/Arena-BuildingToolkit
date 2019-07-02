using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arena;

public class CTFCarryFlag : MonoBehaviour
{
    // Start is called before the first frame update
    // Update is called once per frame
    public void getcolor(int teamid)
    {
        GetComponentInParent<GlobalManager>().ApplyTeamMaterial(
            teamid,
            this.gameObject
        );
    }

    public void disboardflag(){
        this.gameObject.GetComponent<Renderer>().enabled = false;
    }

   public  void showflag(){
        this.gameObject.GetComponent<Renderer>().enabled = true;
    }
}
