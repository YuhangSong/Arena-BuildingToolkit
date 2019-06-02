using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallflatLifebarController : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+1, player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+1,player.transform.position.z);
    }
}
