using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
    
    private bool MouseClick = false;
    public float cameraRotateSpeed = 5f;
    public Transform RotateTarget;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //Rotation
        if (Input.GetMouseButtonDown(0)) MouseClick = true;
        if (Input.GetMouseButtonUp(0)) MouseClick = false;

        if(MouseClick) transform.RotateAround(RotateTarget.position, RotateTarget.up, Input.GetAxis("Mouse X") * cameraRotateSpeed);
    }
}
