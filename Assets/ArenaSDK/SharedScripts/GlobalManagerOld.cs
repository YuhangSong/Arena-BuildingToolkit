using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManagerOld : MonoBehaviour
{
    public List<string> respawns_tag = new List<string>();
    public List<string> destroy_tag  = new List<string>();

    public GameObject displayA;
    public GameObject displayB;

    private string log_tag;
    private List<GameObject> respawns = new List<GameObject>();

    private string turn;
    private int piece_color;

    // Start is called before the first frame update
    virtual protected void
    Start()
    {
        log_tag = "GlobalManager";
        Debug.Log(log_tag + " Start");
        tag = "GlobalManager";

        add_respawn_objects();
        this.reset_turn();
    }

    // called by the agent to reset the global
    virtual public void
    Reset()
    {
        Debug.Log(log_tag + " Reset");
        respawn_objects();
        destroy_objects();
        this.reset_turn();
    }

    public string
    get_turn()
    {
        return this.turn;
    }

    public int
    get_color()
    {
        return piece_color;
    }

    public void
    return_turn()
    {
        if (this.turn == "AgentA") {
            this.turn = "AgentB";
            if (piece_color == 1) {
                displayA.SetActive(false);
                displayB.SetActive(true);
            } else   {
                displayA.SetActive(true);
                displayB.SetActive(false);
            }
        } else if (this.turn == "AgentB")   {
            this.turn = "AgentA";
            if (piece_color == 1) {
                displayA.SetActive(true);
                displayB.SetActive(false);
            } else   {
                displayA.SetActive(false);
                displayB.SetActive(true);
            }
        } else   {
            Debug.Log("Error");
        }
    }

    private int
    random_sign()
    {
        return Random.Range(0, 2) * 2 - 1;
    }

    private void
    reset_turn()
    {
        displayA.SetActive(true);
        displayB.SetActive(false);
        if (this.random_sign() > 0) {
            this.turn   = "AgentA";
            piece_color = 1;
        } else   {
            this.turn   = "AgentB";
            piece_color = -1;
        }
    }

    private void
    destroy_objects()
    {
        // things to be destryed at reset
        foreach (string each_tag in destroy_tag) {
            foreach (GameObject each_gameobject in GameObject.FindGameObjectsWithTag(each_tag)) {
                Destroy(each_gameobject.gameObject);
            }
        }
    }

    private void
    respawn_objects()
    {
        // things to be respawned at the reset
        foreach (GameObject each in this.respawns) {
            each.SetActive(true);
        }
    }

    public static void
    Print2DArray<T>(T[,] matrix)
    {
        string to_print = "(" + matrix.GetLength(0) + "," + matrix.GetLength(1) + ")" + "\n";

        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                to_print += (matrix[i, j] + "\t");
            }
            to_print += '\n';
        }
        Debug.Log(to_print);
    }

    private void
    add_respawn_objects()
    {
        // things to repspawn
        foreach (string each_tag in respawns_tag) {
            foreach (GameObject each_gameobject in GameObject.FindGameObjectsWithTag(each_tag)) {
                this.respawns.Add(each_gameobject);
            }
        }
    }
}
