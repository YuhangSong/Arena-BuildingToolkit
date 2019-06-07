/// <summary>
/// This disables mesh renderer on object. Used for helper objects in scene that should be visible in editor but invisible when game is played
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunTemple {

public class DisableRenderer : MonoBehaviour {

	// Use this for initialization
	void Start () {
			GetComponent<MeshRenderer> ().enabled = false;
		
	}
	

}


}
