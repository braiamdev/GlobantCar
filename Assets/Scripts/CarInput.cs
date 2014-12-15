using UnityEngine;
using System.Collections;

public class CarInput : MonoBehaviour {


	public string accelerationAxis = "Vertical";
	public string steeringAxis = "Horizontal";
	public string handbrakeButton = "Handbrake";
	public string shiftGearUp = "ShiftGearUp";
	public string shiftGearDown = "ShiftGearDown";

	public CarController carController;



	void Update () {
		carController.ApplyAcceleration(Input.GetAxis(accelerationAxis));
		carController.ApplySteering(Input.GetAxis(steeringAxis));
		carController.IsHandbraking = Input.GetButton(handbrakeButton);

		if(Input.GetButtonDown(shiftGearUp)){
			carController.ShiftGearUp();
		}

		if(Input.GetButtonDown(shiftGearDown)){
			carController.ShiftGearDown();
		}

	}
}
