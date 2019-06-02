using System;
using UnityEngine;

public class TargetBall : MonoBehaviour {

	public float degreesPerSecond = 20f;  // rotation speed

	public Transform Target;

	void UpdateContinuous () {
		TimeSpan time = DateTime.Now.TimeOfDay;
		Target.localRotation =
			Quaternion.Euler(0f, (float)time.TotalSeconds * degreesPerSecond, 0f);
	}

	void Update () {

		UpdateContinuous();
	}
	
}
