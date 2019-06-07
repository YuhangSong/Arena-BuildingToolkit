using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SunTemple{
	
	public class CursorManager : MonoBehaviour {

		public static CursorManager instance;

		public Sprite defaultCursor;
		public Sprite lockedCursor;
		public Sprite doorCursor;

		private UnityEngine.UI.Image img;


		void Awake () {
			instance = this;
			img = GetComponent<UnityEngine.UI.Image> ();						
		}


		public void SetCursorToLocked(){
			img.sprite = lockedCursor;
			
		}

		public void SetCursorToDoor(){
			img.sprite = doorCursor;
		}

		public void SetCursorToDefault(){
			img.sprite = defaultCursor; 
		}
		

	}
}
