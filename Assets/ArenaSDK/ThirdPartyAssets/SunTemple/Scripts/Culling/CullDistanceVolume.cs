/// <summary>
/// Adjusts camera cull distances when player is inside trigger area. 
/// Good for aggressively culling faraway objects when player is in interior or in tight areas with limited visibility.
/// Objects on alwaysVisibleLayers will never be culled. Good for distant landmarks such as mountain peaks or towers or otherwise persistent objects
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SunTemple {

	public class CullDistanceVolume : MonoBehaviour {

		public float MaxDistance = 120.0f;
		public bool visualizeMaxDistance = true;
		public string playerTag = "Player";
		public int[] alwaysVisibleLayers = new int[] {31};
		private GameObject player;
		private Camera playerCamera;
		float[] storedCullDistances;

		private bool scriptIsEnabled = true;




		void Start () {

			player = GameObject.FindGameObjectWithTag (playerTag);
			if (!player) {
				Debug.LogWarning (this.GetType ().Name + ".cs on " + this.name + ", No object tagged with " + playerTag + " found in Scene", gameObject);
				scriptIsEnabled = false;
				return;
			}


			playerCamera = Camera.main; 
			if (!playerCamera) {
				Debug.LogWarning (this.GetType ().Name + ".cs on " + this.name + " No object tagged with MainCamera in Scene", gameObject);
				scriptIsEnabled = false;
				return;
			}

			storedCullDistances = playerCamera.layerCullDistances;
			scriptIsEnabled = true;
					
		}



		bool Contains(int x, int[] y){

			if (y.Length == 0) {
				return false;
			}

			for (int i = 0; i < y.Length; i++){
				if (y[i] == x){
					return true;
				}
			}

			return false;
		}






		void EditCullDistances(Camera cam){
			float[] distances = new float[32];

			for (int i = 0; i < distances.Length; i++) {
				if (Contains(i, alwaysVisibleLayers) == false){
					distances [i] = MaxDistance;	
				}								
			}

			cam.layerCullDistances = distances;
		}



		void OnTriggerEnter(Collider other){
			if (other.tag == playerTag && scriptIsEnabled) {
				EditCullDistances (playerCamera);
			}
		}


		
		void OnTriggerExit(Collider other){
			if (other.tag == playerTag && scriptIsEnabled) {
				playerCamera.layerCullDistances = storedCullDistances;
			}
		}



		#if UNITY_EDITOR

		// Draw gizmos in editor for convenience
		void OnDrawGizmosSelected() {
			if (visualizeMaxDistance) {
				Gizmos.color = Color.green;		
				Gizmos.DrawWireSphere (transform.position, MaxDistance);
			}
	
		}
		#endif




	}




}