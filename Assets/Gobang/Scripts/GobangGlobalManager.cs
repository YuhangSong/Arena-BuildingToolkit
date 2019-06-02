using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobangGlobalManager : GlobalManagerOld
{
    public GameObject backgroundcell;

    public GameObject AgentAcolorB;
    public GameObject AgentAcolorW;
    public GameObject AgentBcolorB;
    public GameObject AgentBcolorW;


    public int size_num;
    // Start is called before the first frame update
    override protected void Start()
    {
        float center_x = 5.0f;
        float center_z = 5.0f;
        // Debug.Log("GobangGlobalManager Start");
        //Draw the background
        for(int i=0;i<size_num;i++){
            for(int j=0;j<size_num;j++){
                GameObject temp_cell;
                temp_cell = Instantiate(backgroundcell, new Vector3(center_x*i,0.0f,center_z*j), backgroundcell.transform.rotation) as GameObject;
            }
        }
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
        Debug.Log("GobangGlobalManager Reset");
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
