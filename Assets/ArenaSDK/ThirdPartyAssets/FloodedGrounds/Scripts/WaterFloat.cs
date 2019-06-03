using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloat : MonoBehaviour {


	public float WaterHeight = 15.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.y < WaterHeight) {
			//Vector3 newposition = Vector3 (transform.position.x, WaterHeight, transform.position.y);
			//transform.position.y = newposition;

			transform.position = new Vector3 (transform.position.x, WaterHeight, transform.position.z);
		}
		
	}
}
