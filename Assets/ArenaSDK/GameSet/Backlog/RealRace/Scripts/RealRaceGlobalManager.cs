using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealRaceGlobalManager : MonoBehaviour
{
    private string World;
    private List<string> WorldList = new List<string>();
    private List<string> AgentList = new List<string>();

    private List<GameObject> respawns          = new List<GameObject>();
    private List<Vector3> respawns_position    = new List<Vector3>();
    private List<Quaternion> respawns_rotation = new List<Quaternion>();

    private int NumCheckPoints;

    // Start is called before the first frame update
    void
    Start()
    {
        WorldList.Add("Lakes");
        WorldList.Add("Sprint");
        WorldList.Add("Drift");
        WorldList.Add("Night");
        set_world("Night");

        AgentList.Add("AgentA");
        AgentList.Add("AgentB");
        if (World == "Lakes") {
            set_all_agents_max_steps(10000);
        } else if (World == "Sprint") {
            set_all_agents_max_steps(30000);
        } else if (World == "Drift") {
            set_all_agents_max_steps(10000);
        } else if (World == "Night") {
            set_all_agents_max_steps(30000);
        }

        // things to repspawn
        this.add_as_respawns("Player");

        this.NumCheckPoints = 0;
        foreach (GameObject each in GameObject.FindGameObjectsWithTag("CheckPoint")) {
            int temp = each.gameObject.GetComponent<CheckPoint>().Number + 1;
            if (temp > this.NumCheckPoints) {
                this.NumCheckPoints = temp;
            }
        }
    }

    private void
    set_all_agents_max_steps(int max_steps)
    {
        foreach (string each in AgentList) {
            GameObject.FindWithTag(each).GetComponent<RealRaceAgent>().agentParameters.maxStep = max_steps;
            Debug.Log(each + " set maxStep to " + max_steps);
        }
    }

    private void
    set_world(string tagger)
    {
        World = tagger;
        Debug.Log("Set world to " + World);
        // remove other worlds
        foreach (string each in WorldList) {
            if (each != World) {
                if (GameObject.FindWithTag(each) != null) {
                    GameObject.FindWithTag(each).gameObject.SetActive(false);
                }
            }
        }
    }

    public int
    GetNumCheckPoints()
    {
        return this.NumCheckPoints;
    }

    private void
    add_as_respawns(string tagger)
    {
        foreach (GameObject each in GameObject.FindGameObjectsWithTag(tagger)) {
            this.respawns.Add(each);
            this.respawns_position.Add(each.transform.position);
            this.respawns_rotation.Add(each.transform.rotation);
        }
    }

    private void
    add_as_destroy(string tagger)
    {
        foreach (GameObject each in GameObject.FindGameObjectsWithTag(tagger)) {
            Destroy(each.gameObject);
        }
    }

    // called by the agent to reset the global
    public void
    Reset()
    {
        // things to be respawned at the reset
        int i = 0;

        foreach (GameObject each in this.respawns) {
            each.SetActive(true);
            each.transform.position = respawns_position[i];
            each.transform.rotation = respawns_rotation[i];
            if (each.GetComponentsInChildren<Rigidbody>() != null) {
                foreach (Rigidbody eacheach in each.GetComponentsInChildren<Rigidbody>()) {
                    eacheach.velocity        = Vector3.zero;
                    eacheach.angularVelocity = Vector3.zero;
                }
            }
            i++;
        }

        // things to be destryed at reset
        this.add_as_destroy("NONE");
    }
}
