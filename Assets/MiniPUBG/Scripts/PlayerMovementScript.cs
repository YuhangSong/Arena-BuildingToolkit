using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementScript : MonoBehaviour {
	Rigidbody rb;

	[Tooltip("Current players speed")]
	public float currentSpeed;
	[Tooltip("Assign players camera here")]
	[HideInInspector]public Transform cameraMain;
	[Tooltip("Force that moves player into jump")]
	public float jumpForce = 500;
	[Tooltip("Position of the camera inside the player")]
	[HideInInspector]public Vector3 cameraPosition;

    private StrikeAgent agent;

    private void Start()
    {
        this.agent = this.gameObject.GetComponentInParent<StrikeAgent>();
    }

    /*
	 * Getting the Players rigidbody component.
	 * And grabbing the mainCamera from Players child transform.
	 */
    void Awake(){
		rb = GetComponent<Rigidbody>();
		cameraMain = transform.Find("Main Camera").transform;
		bulletSpawn = cameraMain.Find("BulletSpawn").transform;
		ignoreLayer = 1 << LayerMask.NameToLayer ("Player");

	}
	private Vector3 slowdownV;
	private Vector2 horizontalMovement;
	/*
	* Raycasting for meele attacks and input movement handling here.
	*/
	void FixedUpdate(){
		RaycastForMeleeAttacks ();

		PlayerMovementLogic ();
	}
	/*
	* Accordingly to input adds force and if magnitude is bigger it will clamp it.
	* If player leaves keys it will deaccelerate
	*/
	void PlayerMovementLogic(){
		currentSpeed = rb.velocity.magnitude;
		horizontalMovement = new Vector2 (rb.velocity.x, rb.velocity.z);
		if (horizontalMovement.magnitude > maxSpeed){
			horizontalMovement = horizontalMovement.normalized;
			horizontalMovement *= maxSpeed;    
		}
		rb.velocity = new Vector3 (
			horizontalMovement.x,
			rb.velocity.y,
			horizontalMovement.y
		);
		if (grounded){
			rb.velocity = Vector3.SmoothDamp(rb.velocity,
				new Vector3(0,rb.velocity.y,0),
				ref slowdownV,
				deaccelerationSpeed);
		}

		if (grounded) {
			rb.AddRelativeForce (this.agent.GetAxis ("Horizontal") * accelerationSpeed * Time.deltaTime, 0, this.agent.GetAxis ("Vertical") * accelerationSpeed * Time.deltaTime);
		} else {
			rb.AddRelativeForce (this.agent.GetAxis ("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0, this.agent.GetAxis ("Vertical") * accelerationSpeed / 2 * Time.deltaTime);

		}
		/*
		 * Slippery issues fixed here
		 */
		if (this.agent.GetAxis ("Horizontal") != 0 || this.agent.GetAxis ("Vertical") != 0) {
			deaccelerationSpeed = 0.5f;
		} else {
			deaccelerationSpeed = 0.1f;
		}
	}
	/*
	* Handles jumping and ads the force and sounds.
	*/
	void Jumping(){
		if (this.agent.GetButton ("Jump") && grounded) {
			rb.AddRelativeForce (Vector3.up * jumpForce);
			//if (_jumpSound)
			//	_jumpSound.Play ();
			//else
			//	print ("Missig jump sound.");
			//_walkSound.Stop ();
			//_runSound.Stop ();
		}
	}
	/*
	* Update loop calling other stuff
	*/
	void Update(){
		

		Jumping ();

		Crouching();

		//WalkingSound ();


	}//end update

	/*
	* Checks if player is grounded and plays the sound accorindlgy to his speed
	*/
	void WalkingSound(){
		if (_walkSound && _runSound) {
			if (RayCastGrounded ()) { //for walk sounsd using this because suraface is not straigh			
				if (currentSpeed > 1) {
					//				print ("unutra sam");
					if (maxSpeed == 3) {
						//	print ("tu sem");
						if (!_walkSound.isPlaying) {
							//	print ("playam hod");
							_walkSound.Play ();
							_runSound.Stop ();
						}					
					} else if (maxSpeed == 5) {
						//	print ("NE tu sem");

						if (!_runSound.isPlaying) {
							_walkSound.Stop ();
							_runSound.Play ();
						}
					}
				} else {
					_walkSound.Stop ();
					_runSound.Stop ();
				}
			} else {
				_walkSound.Stop ();
				_runSound.Stop ();
			}
		} else {
			print ("Missing walk and running sounds.");
		}

	}
	/*
	* Raycasts down to check if we are grounded along the gorunded method() because if the
	* floor is curvy it will go ON/OFF constatly this assures us if we are really grounded
	*/
	private bool RayCastGrounded(){
		RaycastHit groundedInfo;
		if(Physics.Raycast(transform.position, transform.up *-1f, out groundedInfo, 1, ~ignoreLayer)){
			Debug.DrawRay (transform.position, transform.up * -1f, Color.red, 0.0f);
			if(groundedInfo.transform != null){
				//print ("vracam true");
				return true;
			}
			else{
				//print ("vracam false");
				return false;
			}
		}
		//print ("nisam if dosao");

		return false;
	}

	/*
	* If player toggle the crouch it will scale the player to appear that is crouching
	*/
	void Crouching(){
		if(this.agent.GetButton("Crouch")){
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1,0.6f,1), Time.deltaTime * 15);
		}
		else{
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1,1,1), Time.deltaTime * 15);

		}
	}


	[Tooltip("The maximum speed you want to achieve")]
	public int maxSpeed = 5;
	[Tooltip("The higher the number the faster it will stop")]
	public float deaccelerationSpeed = 15.0f;


	[Tooltip("Force that is applied when moving forward or backward")]
	public float accelerationSpeed = 50000.0f;


	[Tooltip("Tells us weather the player is grounded or not.")]
	public bool grounded;
	/*
	* checks if our player is contacting the ground in the angle less than 60 degrees
	*	if it is, set groudede to true
	*/
	void OnCollisionStay(Collision other){
		foreach(ContactPoint contact in other.contacts){
			if(Vector2.Angle(contact.normal,Vector3.up) < 60){
				grounded = true;
			}
		}
	}
	/*
	* On collision exit set grounded to false
	*/
	void OnCollisionExit ()
	{
		grounded = false;
	}


	RaycastHit hitInfo;
	private float meleeAttack_cooldown;
	private string currentWeapo;
	[Tooltip("Put 'Player' layer here")]
	[Header("Shooting Properties")]
	private LayerMask ignoreLayer;//to ignore player layer
	Ray ray1, ray2, ray3, ray4, ray5, ray6, ray7, ray8, ray9;
	private float rayDetectorMeeleSpace = 0.15f;
	private float offsetStart = 0.05f;
	[Tooltip("Put BulletSpawn gameobject here, palce from where bullets are created.")]
	[HideInInspector]
	public Transform bulletSpawn; //from here we shoot a ray to check where we hit him;
	/*
	* This method casts 9 rays in different directions. ( SEE scene tab and you will see 9 rays differently coloured).
	* Used to widley detect enemy infront and increase meele hit detectivity.
	* Checks for cooldown after last preformed meele attack.
	*/


	public bool been_to_meele_anim = false;
	private void RaycastForMeleeAttacks(){




		if (meleeAttack_cooldown > -5) {
			meleeAttack_cooldown -= 1 * Time.deltaTime;
		}


		if (GetComponent<GunInventory> ().currentGun) {
			if (GetComponent<GunInventory> ().currentGun.GetComponent<GunScript> ()) 
				currentWeapo = "gun";
		}

		//middle row
		ray1 = new Ray (bulletSpawn.position + (bulletSpawn.right*offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace));
		ray2 = new Ray (bulletSpawn.position - (bulletSpawn.right*offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace));
		ray3 = new Ray (bulletSpawn.position, bulletSpawn.forward);
		//upper row
		ray4 = new Ray (bulletSpawn.position + (bulletSpawn.right*offsetStart) + (bulletSpawn.up*offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
		ray5 = new Ray (bulletSpawn.position - (bulletSpawn.right*offsetStart) + (bulletSpawn.up*offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
		ray6 = new Ray (bulletSpawn.position + (bulletSpawn.up*offsetStart), bulletSpawn.forward + (bulletSpawn.up * rayDetectorMeeleSpace));
		//bottom row
		ray7 = new Ray (bulletSpawn.position + (bulletSpawn.right*offsetStart) - (bulletSpawn.up*offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
		ray8 = new Ray (bulletSpawn.position - (bulletSpawn.right*offsetStart) - (bulletSpawn.up*offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
		ray9 = new Ray (bulletSpawn.position -(bulletSpawn.up*offsetStart), bulletSpawn.forward - (bulletSpawn.up * rayDetectorMeeleSpace));

		Debug.DrawRay (ray1.origin, ray1.direction, Color.cyan);
		Debug.DrawRay (ray2.origin, ray2.direction, Color.cyan);
		Debug.DrawRay (ray3.origin, ray3.direction, Color.cyan);
		Debug.DrawRay (ray4.origin, ray4.direction, Color.red);
		Debug.DrawRay (ray5.origin, ray5.direction, Color.red);
		Debug.DrawRay (ray6.origin, ray6.direction, Color.red);
		Debug.DrawRay (ray7.origin, ray7.direction, Color.yellow);
		Debug.DrawRay (ray8.origin, ray8.direction, Color.yellow);
		Debug.DrawRay (ray9.origin, ray9.direction, Color.yellow);

		if (GetComponent<GunInventory> ().currentGun) {
			if (GetComponent<GunInventory> ().currentGun.GetComponent<GunScript> ().meeleAttack == false) {
				been_to_meele_anim = false;
			}
			if (GetComponent<GunInventory> ().currentGun.GetComponent<GunScript> ().meeleAttack == true && been_to_meele_anim == false) {
				been_to_meele_anim = true;
				//	if (isRunning == false) {
				StartCoroutine ("MeeleAttackWeaponHit");
				//	}
			}
		}

	}

	/*
	 *Method that is called if the waepon hit animation has been triggered the first time via Q input
	 *and if is, it will search for target and make damage
	 */
	IEnumerator MeeleAttackWeaponHit(){
		if (Physics.Raycast (ray1, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast (ray2, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast (ray3, out hitInfo, 2f, ~ignoreLayer)
			|| Physics.Raycast (ray4, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast (ray5, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast (ray6, out hitInfo, 2f, ~ignoreLayer)
			|| Physics.Raycast (ray7, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast (ray8, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast (ray9, out hitInfo, 2f, ~ignoreLayer)) {
			//Debug.DrawRay (bulletSpawn.position, bulletSpawn.forward + (bulletSpawn.right*0.2f), Color.green, 0.0f);
			if (hitInfo.transform.tag=="Dummie") {
				Transform _other = hitInfo.transform.root.transform;
				if (_other.transform.tag == "Dummie") {
					print ("hit a dummie");
				}
				InstantiateBlood(hitInfo,false);
			}
		}
		yield return new WaitForEndOfFrame ();
	}

	[Header("BloodForMelleAttaacks")]
	RaycastHit hit;//stores info of hit;
	[Tooltip("Put your particle blood effect here.")]
	public GameObject bloodEffect;//blod effect prefab;
	/*
	* Upon hitting enemy it calls this method, gives it raycast hit info 
	* and at that position it creates our blood prefab.
	*/
	void InstantiateBlood (RaycastHit _hitPos,bool swordHitWithGunOrNot) {		

		if (currentWeapo == "gun") {
			GunScript.HitMarkerSound ();

			//if (_hitSound)
			//	_hitSound.Play ();
			//else
				//print ("Missing hit sound");
			
			if (!swordHitWithGunOrNot) {
				if (bloodEffect)
					Instantiate (bloodEffect, _hitPos.point, Quaternion.identity);
				else
					print ("Missing blood effect prefab in the inspector.");
			}
		} 
	}
	private GameObject myBloodEffect;


	[Header("Player SOUNDS")]
	[Tooltip("Jump sound when player jumps.")]
	public AudioSource _jumpSound;
	[Tooltip("Sound while player makes when successfully reloads weapon.")]
	public AudioSource _freakingZombiesSound;
	[Tooltip("Sound Bullet makes when hits target.")]
	public AudioSource _hitSound;
	[Tooltip("Walk sound player makes.")]
	public AudioSource _walkSound;
	[Tooltip("Run Sound player makes.")]
	public AudioSource _runSound;
}

