/// <summary>
/// This will move object and it's children to specific layer when game is launched.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Sun_Temple{


	public class MoveToLayer : MonoBehaviour {

		public int layer = 31;
		public bool alsoMoveChildren = true;



		void Awake(){



			gameObject.layer = layer;

			if (alsoMoveChildren) {
				SetChildLayerRecursive (gameObject.transform, layer);
			}


		}






		void SetChildLayerRecursive(Transform t, int layer){
			
			t.gameObject.layer = layer;

			if (t.childCount <= 0) {
				return;
			}

			foreach (Transform child in t) {
				SetChildLayerRecursive (child, layer);
			}	
		}




	}



}
