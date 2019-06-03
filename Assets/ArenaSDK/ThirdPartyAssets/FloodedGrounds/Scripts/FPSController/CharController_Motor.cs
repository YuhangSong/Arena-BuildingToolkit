using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunTemple{


	public class CharController_Motor : MonoBehaviour {

		public float speed = 10.0f;
		public float sensitivity = 60.0f;
		CharacterController character;
		public GameObject cam;
		float moveFB, moveLR;	
		float rotHorizontal, rotVertical;
		public bool webGLRightClickRotation = true;
		float gravity = -9.8f;

		//string debugText;


		void Start(){

			character = GetComponent<CharacterController> ();

			webGLRightClickRotation = false;

			if (Application.platform == RuntimePlatform.WebGLPlayer) {
				webGLRightClickRotation = true;
				sensitivity = sensitivity * 1.5f;
			}


		}





		void FixedUpdate(){
			moveFB = Input.GetAxis ("Horizontal") * speed;
			moveLR = Input.GetAxis ("Vertical") * speed;

			rotHorizontal = Input.GetAxisRaw ("Mouse X") * sensitivity;
			rotVertical = Input.GetAxisRaw ("Mouse Y") * sensitivity;


			Vector3 movement = new Vector3 (moveFB, gravity, moveLR);


			if (webGLRightClickRotation) {
				if (Input.GetKey (KeyCode.Mouse0)) {
					CameraRotation (cam, rotHorizontal, rotVertical);
				}
			} else if (!webGLRightClickRotation) {
				CameraRotation (cam, rotHorizontal, rotVertical);
			}

			movement = transform.rotation * movement;
			character.Move (movement * Time.fixedDeltaTime);
		}





	

		void CameraRotation(GameObject cam, float rotHorizontal, float rotVertical){	

			transform.Rotate (0, rotHorizontal * Time.fixedDeltaTime, 0);
			cam.transform.Rotate (-rotVertical * Time.fixedDeltaTime, 0, 0);



			if (Mathf.Abs (cam.transform.localRotation.x) > 0.7) {

				float clamped = 0.7f * Mathf.Sign (cam.transform.localRotation.x); 

				Quaternion adjustedRotation = new Quaternion (clamped, cam.transform.localRotation.y, cam.transform.localRotation.z, cam.transform.localRotation.w);
				cam.transform.localRotation = adjustedRotation;
			}


		}




	}



}