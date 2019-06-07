//2016 Spyblood Productions
//Use for non-commerical games only. do not sell comercially
//without permission first

using UnityEngine;
using System.Collections;



public enum DriveType
{
	RWD,
	FWD,
	AWD
};
[System.Serializable]
public class WC
{
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;
}
[System.Serializable]
public class WT
{
	public Transform wheelFL;
	public Transform wheelFR;
	public Transform wheelRL;
	public Transform wheelRR;
}
//[RequireComponent(typeof(AudioSource))]//needed audiosource
[RequireComponent(typeof(Rigidbody))]//needed Rigid body
public class CarControlCS : MonoBehaviour {

	public WC wheels;
	public WT tires;
	public WheelCollider[] extraWheels;
	public Transform[] extraWheelObjects;
	public DriveType DriveTrain = DriveType.RWD;
	public Vector3 centerOfGravity;//car's center of mass offset
	public float maxTorque = 1000f;//car's acceleration value
	public float maxReverseSpeed = 50f;//top speed for the reverse gear
	public float handBrakeTorque = 500f;//hand brake value
	public float maxSteer = 25f;//max steer angle
	public bool mobileInput = false;//do you want this to be a mobile game?
	public float[] GearRatio;//determines how many gears the car has, and at what speed the car shifts to the appropriate gear
	private int throttleInput;//read only
	private int steerInput;//read only
	private bool reversing;//read only
	private float currentSpeed;//read only
	public float maxSpeed = 150f;//how fast the vehicle can go
	private int gear;//current gear
	Vector3 localCurrentSpeed;
    private RealRaceAgent agent;

	// Use this for initialization
	void Start () {
		// Alter the center of mass for stability on your car
		GetComponent<Rigidbody>().centerOfMass = centerOfGravity;
        this.agent = this.gameObject.GetComponentInParent<RealRaceAgent>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (GetComponent<Rigidbody>().centerOfMass != centerOfGravity)
		GetComponent<Rigidbody>().centerOfMass = centerOfGravity;
		
		AllignWheels ();
		Drive ();
		//EngineAudio ();

		currentSpeed = GetComponent<Rigidbody>().velocity.magnitude * 2.23693629f;//convert currentspeed into MPH

		localCurrentSpeed = transform.InverseTransformDirection (GetComponent<Rigidbody> ().velocity);

		//if (currentSpeed > maxSpeed || (localCurrentSpeed.z*2.23693629f) < -maxReverseSpeed){

	}

	void AllignWheels()
	{
		//allign the wheel objs to their colliders

		Quaternion quat;
		Vector3 pos;
		wheels.wheelFL.GetWorldPose(out pos,out quat);
		tires.wheelFL.position = pos;
		tires.wheelFL.rotation = quat;

		wheels.wheelFR.GetWorldPose(out pos,out quat);
		tires.wheelFR.position = pos;
		tires.wheelFR.rotation = quat;

		wheels.wheelRL.GetWorldPose(out pos,out quat);
		tires.wheelRL.position = pos;
		tires.wheelRL.rotation = quat;

		wheels.wheelRR.GetWorldPose(out pos,out quat);
		tires.wheelRR.position = pos;
		tires.wheelRR.rotation = quat;

		for (int i = 0; i < extraWheels.Length; i++)
		{

			for (int k = 0; k < extraWheelObjects.Length; k++) {
			
				Quaternion quater;
				Vector3 vec3;

				extraWheels [i].GetWorldPose (out vec3, out quater);
				extraWheelObjects [k].position = vec3;
				extraWheelObjects [k].rotation = quater;
			
			}

		}
	}

	void Drive()
	{
		if (mobileInput)
			return;
		//dont call this function if mobile input is checked in the editor
		float gasMultiplier = 0f;

		if (!reversing) {
			if (currentSpeed < maxSpeed)
				gasMultiplier = 1f;
			else
				gasMultiplier = 0f;

		} else {
			if (currentSpeed < maxReverseSpeed)
				gasMultiplier = 1f;
			else
				gasMultiplier = 0f;
		}

		//the car will be 4 wheel drive or else it will be slow or feel a little sluggish
		//no matter how much you increase the max torque.
		if (DriveTrain == DriveType.RWD)
		{
			wheels.wheelRR.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;
			wheels.wheelRL.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;

			if (localCurrentSpeed.z < -0.1f && wheels.wheelRL.rpm < 10) {//in local space, if the car is travelling in the direction of the -z axis, (or in reverse), reversing will be true
				reversing = true;
			} else {
				reversing = false;
			}
		}
		if (DriveTrain == DriveType.FWD)
		{
			wheels.wheelFL.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;
			wheels.wheelFR.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;

			if (localCurrentSpeed.z < -0.1f && wheels.wheelFL.rpm < 10) {//in local space, if the car is travelling in the direction of the -z axis, (or in reverse), reversing will be true
				reversing = true;
			} else {
				reversing = false;
			}
		}
		if (DriveTrain == DriveType.AWD)
		{

			wheels.wheelFL.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;
			wheels.wheelFR.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;
			wheels.wheelRL.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;
			wheels.wheelRR.motorTorque = maxTorque * this.agent.GetAxis("Vertical") * gasMultiplier;

			if (localCurrentSpeed.z < -0.1f && wheels.wheelRL.rpm < 10) {//in local space, if the car is travelling in the direction of the -z axis, (or in reverse), reversing will be true
				reversing = true;
			} else {
				reversing = false;
			}
		}

		
		wheels.wheelFL.steerAngle = maxSteer * this.agent.GetAxis("Horizontal");
		wheels.wheelFR.steerAngle = maxSteer * this.agent.GetAxis("Horizontal");
		if (this.agent.GetButton("Jump"))//pressing space triggers the car's handbrake
		{
			wheels.wheelFL.brakeTorque = handBrakeTorque;
			wheels.wheelFR.brakeTorque = handBrakeTorque;
			wheels.wheelRL.brakeTorque = handBrakeTorque;
			wheels.wheelRR.brakeTorque = handBrakeTorque;
		}
		else//letting go of space disables the handbrake
		{
			wheels.wheelFL.brakeTorque = 0f;
			wheels.wheelFR.brakeTorque = 0f;
			wheels.wheelRL.brakeTorque = 0f;
			wheels.wheelRR.brakeTorque = 0f;
		}
	}

	//void EngineAudio()
	//{
	//	//the function called to give the car basic audio, as well as some gear shifting effects
	//	//it's prefered you use the engine sound included, but you can use your own if you have one.
	//	//~~~~~~~~~~~~[IMPORTANT]~~~~~~~~~~~~~~~~
	//	//make sure your last gear value is higher than the max speed variable or else you will
	//	//get unwanted errors!!
		
	//	//anyway, let's get started
		
	//	for (int i = 0; i < GearRatio.Length; i++) {
	//		if (GearRatio [i] > currentSpeed) {
	//			//break this value
	//			break;
	//		}

	//		float minGearValue = 0f;
	//		float maxGearValue = 0f;
	//		if (i == 0) {
	//			minGearValue = 0f;
	//		} else {
	//			minGearValue = GearRatio [i];
	//		}
	//		maxGearValue = GearRatio [i+1];
		
	//		float pitch = ((currentSpeed - minGearValue) / (maxGearValue - minGearValue)+0.3f * (gear+1));
	//		GetComponent<AudioSource> ().pitch = pitch;
		
	//		gear = i;
	//	}
	//}

	//void OnGUI()
	//{
	//	//show the GUI for the speed and gear we are on.
	//	GUI.Box(new Rect(10,10,70,30),"MPH: " + Mathf.Round(GetComponent<Rigidbody>().velocity.magnitude * 2.23693629f));
	//	if (!reversing)
	//		GUI.Box(new Rect(10,70,70,30),"Gear: " + (gear+1));
	//	if (reversing)//if the car is going backwards display the gear as R
	//		GUI.Box(new Rect(10,70,70,30),"Gear: R");
	//}

}
