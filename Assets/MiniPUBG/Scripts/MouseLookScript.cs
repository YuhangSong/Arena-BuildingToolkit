using UnityEngine;
using System.Collections;


public class MouseLookScript : MonoBehaviour {

	[HideInInspector]
	public Transform myCamera;

    private StrikeAgent agent;

    private void Start()
    {
        this.agent = this.gameObject.GetComponentInParent<StrikeAgent>();
    }

    /*
	 * Hiding the cursor.
	 */
    void Awake(){
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
		myCamera = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<StrikeGlobalManager>().GetChildObject(this.transform,"MainCamera").transform;
    }

    /*
	* Locking the mouse if pressing L.
	* Triggering the headbob camera omvement if player is faster than 1 of speed
	*/
    void  Update(){

		MouseInputMovement();

		//if (Input.GetKeyDown (KeyCode.L)) {
		//	Cursor.lockState = CursorLockMode.Locked;

		//}
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

		if(GetComponent<PlayerMovementScript>().currentSpeed > 1)
			HeadMovement ();

	}

	[Header("Z Rotation Camera")]
	[HideInInspector] public float timer;
	[HideInInspector] public int int_timer;
	[HideInInspector] public float zRotation;
	[HideInInspector] public float wantedZ;
	[HideInInspector] public float timeSpeed = 2;

	[HideInInspector] public float timerToRotateZ;
	/*
	* Switching Z rotation and applying to camera in camera Rotation().
	*/
	void HeadMovement(){
		timer += timeSpeed * Time.deltaTime;
		int_timer = Mathf.RoundToInt (timer);
		if (int_timer % 2 == 0) {
			wantedZ = -1;
		} else {
			wantedZ = 1;
		}

		zRotation = Mathf.Lerp (zRotation, wantedZ, Time.deltaTime * timerToRotateZ);
	}
	[Tooltip("Current mouse sensivity, changes in the weapon properties")]
	public float mouseSensitvity = 0;
	[HideInInspector]
	public float mouseSensitvity_notAiming = 300;
	[HideInInspector]
	public float mouseSensitvity_aiming = 50;

/*
* FixedUpdate()
* If aiming set the mouse sensitvity from our variables and vice versa.
*/
void FixedUpdate(){

	/*
	 * Reduxing mouse sensitvity if we are aiming.
	 */
	if(this.agent.GetButton("Aim")){
		mouseSensitvity = mouseSensitvity_aiming;
	}
	else if(GetComponent<PlayerMovementScript>().maxSpeed > 5){
		mouseSensitvity = mouseSensitvity_notAiming;
	}
	else{
		mouseSensitvity = mouseSensitvity_notAiming;
	}


	ApplyingStuff();


}


private float rotationYVelocity, cameraXVelocity;
[Tooltip("Speed that determines how much camera rotation will lag behind mouse movement.")]
public float yRotationSpeed, xCameraSpeed;

[HideInInspector]
public float wantedYRotation;
[HideInInspector]
public float currentYRotation;

[HideInInspector]
public float wantedCameraXRotation;
[HideInInspector]
public float currentCameraXRotation;

[Tooltip("Top camera angle.")]
public float topAngleView = 60;
[Tooltip("Minimum camera angle.")]
public float bottomAngleView = -45;
/*
 * Upon mouse movenet it increases/decreased wanted value. (not actually moving yet)
 * Clamping the camera rotation X to top and bottom angles.
 */
void MouseInputMovement(){

	wantedYRotation += agent.GetAxis("Mouse X") * mouseSensitvity;

	wantedCameraXRotation -= agent.GetAxis("Mouse Y") * mouseSensitvity;

	wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);

}

/*
 * Smoothing the wanted movement.
 * Calling the waeponRotation form here, we are rotating the waepon from this script.
 * Applying the camera wanted rotation to its transform.
 */
void ApplyingStuff(){

	currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
	currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

	WeaponRotation();

	transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
	myCamera.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);

}

private Vector2 velocityGunFollow;
private float gunWeightX,gunWeightY;
[Tooltip("Current weapon that player carries.")]
[HideInInspector]
public GameObject weapon;
private GunScript gun;
/*
 * Rotating current weapon from here.
 * Checkig if we have a weapon, if we do, if its a gun it iwll fetch the gun and rotate it accordingly,
 * same goes for the sword.
 * Incase we dont have a weapon or gun or it didnt find it, it will write into the console that it cant find a weapon.
 */
void WeaponRotation(){
	if(!weapon){
		weapon = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<StrikeGlobalManager>().GetChildObject(agent.transform,"Weapon");
		if(weapon){
			if(weapon.GetComponent<GunScript>()){
				try{
					gun = GameObject.FindGameObjectWithTag("GlobalManager").GetComponent<StrikeGlobalManager>().GetChildObject(agent.transform,"Weapon").GetComponent<GunScript>();
				}catch(System.Exception ex){
					print("gun not found->"+ex.StackTrace.ToString());
				}
			}
		}
	}

}

float deltaTime = 0.0f;
[Tooltip("Shows FPS in top left corner.")]
public bool showFps = true;
/*
* Shows fps if its set to true.
*/
void OnGUI(){

	//if(showFps){
	//	FPSCounter();
	//}

}
/*
* Calculating real fps because unity status tab shows too much fps even when its not that mutch so i made my own.
*/
void FPSCounter(){
	int w = Screen.width, h = Screen.height;

	GUIStyle style = new GUIStyle();

	Rect rect = new Rect(0, 0, w, h * 2 / 100);
	style.alignment = TextAnchor.UpperLeft;
	style.fontSize = h * 2 / 100;
	style.normal.textColor = Color.white;
	float msec = deltaTime * 1000.0f;
	float fps = 1.0f / deltaTime;
	string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
	GUI.Label(rect, text, style);
}

}
