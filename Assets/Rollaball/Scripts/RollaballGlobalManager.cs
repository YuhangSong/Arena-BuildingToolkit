using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollaballGlobalManager : MonoBehaviour
{
    private List<GameObject> respawns = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // [customize] things to repspawn
        foreach (GameObject each in GameObject.FindGameObjectsWithTag("Pick Up"))
        {
            this.respawns.Add(each);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // called by the agent to reset the global
    public void Reset()
    {
        // things to be respawned at the reset
        foreach (GameObject each in this.respawns)
        {
            each.SetActive(true);
        }

        // [customize] things to be destryed at reset
        foreach (GameObject each in GameObject.FindGameObjectsWithTag("NONE"))
        {
            Destroy(each.gameObject);
        }
    }
}
