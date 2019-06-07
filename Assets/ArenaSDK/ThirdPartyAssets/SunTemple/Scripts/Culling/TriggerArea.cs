/// <summary>
/// Will enable/disable objects when object tagged as player enters/exits area
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunTemple{

	public class TriggerArea : MonoBehaviour {

		public GameObject targetObjects;
		public string playerTag = "Player";
		public bool startAsVisible = false;

		private MeshFilter meshFilter;

		private bool scriptIsEnabled = true;
		private bool screenshotMode = false;		// this is for taking screens, so that objects are not hidden in editor view


		void Start() {
			if (!targetObjects) {
				Debug.LogWarning (this.GetType ().Name + ".cs on " + gameObject.name + ", please assign target objects", gameObject);
				scriptIsEnabled = false;
				return;
			}

			if (screenshotMode) {
				scriptIsEnabled = false;
				return;
			}


			scriptIsEnabled = true;


			if (startAsVisible) {
				targetObjects.SetActive (true);
			} else {
				targetObjects.SetActive (false);
			}


		
		}




		void OnTriggerEnter(Collider other){
			if (other.tag == playerTag && scriptIsEnabled == true) {
				targetObjects.SetActive (true);	
			}		
		}


		void OnTriggerExit(Collider other){
			if (other.tag == playerTag && scriptIsEnabled == true) {
				targetObjects.SetActive (false);
			}		
		}




		#if UNITY_EDITOR

		// DRAW GIZMOS AND STUFF

		void OnDrawGizmos() {			// this one is always drawn, to make finding and selecting object a bit easier
			if (!screenshotMode) {
				Color gizmoColor = Color.yellow;
				gizmoColor.a = 0.7f;
				Gizmos.color = gizmoColor;

				Gizmos.DrawWireCube (transform.position, new Vector3(1f, 1f, 1f));
				Gizmos.DrawSphere (transform.position, 0.5f);
			}



		}


		void OnDrawGizmosSelected(){			
			if (targetObjects) {
				Gizmos.color = Color.red;
				Gizmos.DrawLine (transform.position, targetObjects.transform.position);
			}

		}
		#endif



	}

}
