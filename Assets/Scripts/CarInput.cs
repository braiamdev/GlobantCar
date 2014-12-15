using UnityEngine;
using System.Collections;

public class CarInput : MonoBehaviour {


	public string accelerationAxis = "Vertical";
	public string steeringAxis = "Horizontal";
	public string handbrakeButton = "Jump";
	public CarController carController;



	void Update () {
		carController.ApplyAcceleration(Input.GetAxis(accelerationAxis));
		carController.ApplySteering(Input.GetAxis(steeringAxis));
		carController.IsHandbraking = Input.GetButton(handbrakeButton);
	}
}
