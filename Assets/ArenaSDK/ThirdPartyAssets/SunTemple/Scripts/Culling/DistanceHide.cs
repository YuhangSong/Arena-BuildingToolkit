/// <summary>
/// Will enable and disable objects as player goes in and out of range (distance)
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SunTemple {
	public class DistanceHide : MonoBehaviour {


		public int distance = 50;
		public bool visualizeDistance = true;
		public string playerTag = "Player";
		public GameObject[] objectsToDisable;
		private GameObject Player;


		private bool scriptIsEnabled = true;
		private bool screenshotMode = false;	// setting this to true will effectively disable script. So that all objects are visible when taking screenshots in editor.




		void Start () {
			Player = GameObject.FindGameObjectWithTag (playerTag);
			if (!Player) {
				Debug.LogWarning (this.GetType ().Name + ".cs on " + this.name + ", No object tagged with " + playerTag + " found in Scene", gameObject);
				scriptIsEnabled = false;
				return;
			}



			if (screenshotMode == false && scriptIsEnabled == true) {
				InvokeRepeating ("CheckForDistance", 1.0f, 0.1f * Random.Range (10, 20));	
			}		

		}	

	

		void CheckForDistance(){
			if (Mathf.Abs (Vector3.Distance (transform.position, Player.transform.position)) > distance) {
				foreach (GameObject obj in objectsToDisable) {
					if (obj && obj.transform) {
						obj.SetActive (false);
					}
				}
			} else {
				foreach (GameObject obj in objectsToDisable) {
					if (obj && obj.transform) {
						obj.SetActive (true);
					}
				}
			}

		}





		#if UNITY_EDITOR
		// Draw gizmos in editor for convenience
		void OnDrawGizmosSelected() {
			if (visualizeDistance) {
				Gizmos.color = Color.red;		
				Gizmos.DrawWireSphere (transform.position, distance);
			}

			if (objectsToDisable.Length > 0) {
				foreach(GameObject obj in objectsToDisable){
					if (obj != null) {
						if (obj.transform != null) {
							Gizmos.DrawLine (transform.position, obj.transform.position);
						}
					}
				}
			}
		}
		#endif




	}


}
