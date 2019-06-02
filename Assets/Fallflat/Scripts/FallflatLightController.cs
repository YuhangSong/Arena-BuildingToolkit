using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallflatLightController : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.transform.position.x,this.transform.position.y,player.transform.position.z);
    }
}
