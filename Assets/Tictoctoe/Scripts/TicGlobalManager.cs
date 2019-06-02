using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicGlobalManager : GlobalManagerOld
{

    public GameObject AgentAcolorB;
    public GameObject AgentAcolorW;
    public GameObject AgentBcolorB;
    public GameObject AgentBcolorW;
    // Start is called before the first frame update
    override protected void Start()
    {
        Debug.Log("TicGlobalManager Start");
        base.Start();

        if (this.get_color() == 1){
            AgentAcolorB.SetActive(true);
            AgentAcolorW.SetActive(false);
            AgentBcolorB.SetActive(false);
            AgentBcolorW.SetActive(true);
        }
        else if (this.get_color() == -1){
            AgentAcolorB.SetActive(false);
            AgentAcolorW.SetActive(true);
            AgentBcolorB.SetActive(true);
            AgentBcolorW.SetActive(false);
        }
        else{
            Debug.Log("Invalid color value: "+this.get_color());
        }
    }

    // called by the agent to reset the global
    override public void Reset()
    {
        Debug.Log("TicGlobalManager Reset");
        base.Reset();

        if (this.get_color() == 1){
            AgentAcolorB.SetActive(true);
            AgentAcolorW.SetActive(false);
            AgentBcolorB.SetActive(false);
            AgentBcolorW.SetActive(true);
        }
        else if (this.get_color() == -1){
            AgentAcolorB.SetActive(false);
            AgentAcolorW.SetActive(true);
            AgentBcolorB.SetActive(true);
            AgentBcolorW.SetActive(false);
        }
        else{
            Debug.Log("Invalid color value: "+this.get_color());
        }
    }
}
