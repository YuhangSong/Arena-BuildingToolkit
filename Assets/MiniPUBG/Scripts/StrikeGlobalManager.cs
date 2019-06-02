using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeGlobalManager : MonoBehaviour
{
    public GameObject LivingCenterLable;

    private string World;
    private List<string> WorldList = new List<string>();

    private List<string> AgentList = new List<string>();

    private GameObject edge_x_p;
    private GameObject edge_x_n;
    private GameObject edge_z_p;
    private GameObject edge_z_n;

    private Vector3 edge_x_p_position;
    private Vector3 edge_x_n_position;
    private Vector3 edge_z_p_position;
    private Vector3 edge_z_n_position;

    private List<GameObject> respawns          = new List<GameObject>();
    private List<Vector3> respawns_position    = new List<Vector3>();
    private List<Quaternion> respawns_rotation = new List<Quaternion>();

    private float player_position_range = 80f;

    private Vector3 living_center;
    private Vector3 range_vector;
    private int tick_count;
    private float progress;
    private float circle_progress;

    private float x_safe_p;
    private float x_safe_n;
    private float z_safe_p;
    private float z_safe_n;

    public float
    remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public float
    mid(float x1, float x2, float rate)
    {
        return (x2 - x1) * rate + x1;
    }

    public bool
    check_range(float x, float min, float max)
    {
        if (x > min && x < max) {
            return true;
        } else {
            return false;
        }
    }

    private void
    check_agent_circle(string agent_tag)
    {
        if (!(
              check_range(GameObject.FindWithTag(agent_tag).GetComponent<StrikeAgent>().player.transform.position.x,
              x_safe_n, x_safe_p) &&
              check_range(GameObject.FindWithTag(agent_tag).GetComponent<StrikeAgent>().player.transform.position.z,
              z_safe_n, z_safe_p)
        ))
        {
            GameObject.FindWithTag(agent_tag).GetComponent<StrikeAgent>().hurt(0.001f);
        }
    }

    public void
    tick()
    {
        this.tick_count += 1;
        this.progress    = this.tick_count / 2f
          / GameObject.FindWithTag("AgentA").GetComponent<StrikeAgent>().agentParameters.maxStep;

        this.circle_progress = remap(this.progress, 0f, 0.7f, 0f, 1f);
        if (this.circle_progress > 1f) this.circle_progress = 1f;
        x_safe_p = mid(edge_x_p_position.x, living_center.x, this.circle_progress);
        x_safe_n = mid(edge_x_n_position.x, living_center.x, this.circle_progress);
        z_safe_p = mid(edge_z_p_position.z, living_center.z, this.circle_progress);
        z_safe_n = mid(edge_z_n_position.z, living_center.z, this.circle_progress);

        edge_x_p.transform.position =
          new Vector3(x_safe_p, edge_x_p.transform.position.y, edge_x_p.transform.position.z);
        edge_x_n.transform.position =
          new Vector3(x_safe_n, edge_x_n.transform.position.y, edge_x_n.transform.position.z);
        edge_z_p.transform.position =
          new Vector3(edge_z_p.transform.position.x, edge_z_p.transform.position.y, z_safe_p);
        edge_z_n.transform.position =
          new Vector3(edge_z_n.transform.position.x, edge_z_n.transform.position.y, z_safe_n);

        foreach (string each in AgentList) {
            check_agent_circle(each);
        }
    }

    private Vector3
    update_range_vector(Vector3 range_vector, string tagger)
    {
        GameObject each = GameObject.FindGameObjectWithTag(tagger);

        range_vector += new Vector3(
            Mathf.Abs(each.transform.position.x),
            (each.transform.position.y),
            Mathf.Abs(each.transform.position.z)
        );
        return range_vector;
    }

    private void
    set_all_agents_max_steps(int max_steps)
    {
        foreach (string each in AgentList) {
            GameObject.FindWithTag(each).GetComponent<StrikeAgent>().agentParameters.maxStep = max_steps;
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

    // Start is called before the first frame update
    void
    Start()
    {
        WorldList.Add("Village");
        WorldList.Add("FloodedGround");
        WorldList.Add("WindridgeCity");
        WorldList.Add("Forest");
        WorldList.Add("SunTemple");
        set_world("SunTemple");

        AgentList.Add("AgentA");
        AgentList.Add("AgentB");
        if (World == "Village") {
            set_all_agents_max_steps(10000);
            player_position_range = 80f;
        } else if (World == "FloodedGround") {
            set_all_agents_max_steps(30000);
            player_position_range = 80f;
        } else if (World == "WindridgeCity") {
            set_all_agents_max_steps(30000);
            player_position_range = 400f;
        } else if (World == "Forest") {
            set_all_agents_max_steps(10000);
            player_position_range = 80f;
        } else if (World == "SunTemple") {
            set_all_agents_max_steps(10000);
            player_position_range = 100f;
        }

        // things to repspawn
        this.add_as_respawns("Player");

        edge_x_p = GameObject.FindWithTag("Edgexp");
        edge_x_n = GameObject.FindWithTag("Edgexn");
        edge_z_p = GameObject.FindWithTag("Edgezp");
        edge_z_n = GameObject.FindWithTag("Edgezn");

        range_vector    = Vector3.zero;
        range_vector    = update_range_vector(range_vector, "Edgexp");
        range_vector    = update_range_vector(range_vector, "Edgexn");
        range_vector    = update_range_vector(range_vector, "Edgezp");
        range_vector    = update_range_vector(range_vector, "Edgezn");
        range_vector.x /= 2f;
        range_vector.z /= 2f;
        range_vector.y /= 4f;

        edge_x_p_position = edge_x_p.transform.position;
        edge_x_n_position = edge_x_n.transform.position;
        edge_z_p_position = edge_z_p.transform.position;
        edge_z_n_position = edge_z_n.transform.position;

        this.Reset();
    } // Start

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
            if (each.CompareTag("Player")) {
                each.transform.position = new Vector3(
                    respawns_position[i].x + Random.Range(-player_position_range, player_position_range),
                    respawns_position[i].y,
                    respawns_position[i].z + Random.Range(-player_position_range, player_position_range)
                );
                each.transform.rotation = respawns_rotation[i];
            }
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

        this.living_center = new Vector3(
            Random.Range(-range_vector.x, range_vector.x),
            range_vector.y,
            Random.Range(-range_vector.z, range_vector.z)
        );
        LivingCenterLable.transform.position = new Vector3(
            living_center.x, LivingCenterLable.transform.position.y, living_center.z);

        this.tick_count      = 0;
        this.progress        = 0f;
        this.circle_progress = 0f;
    } // Reset

    public GameObject
    GetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag) {
                return child.gameObject;
            }
            if (child.childCount > 0) {
                return GetChildObject(child, _tag);
            }
        }
        return this.gameObject;
    }
}
