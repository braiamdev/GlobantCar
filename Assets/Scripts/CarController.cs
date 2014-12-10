using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {

	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;
	
	public Transform wheelFLTransform;
	public Transform wheelFRTransform;
	public Transform wheelRLTransform;
	public Transform wheelRRTransform;

	public float maxTorque = 50f;
	public Vector3 centerOfMassOffset = new Vector3(0f,-0.9f, 0f);
	public float lowestSteerAtSpeed = 50; //the speed at witch highSpeedSteerAngle will be reached
	public float lowSpeedSteerAngle = 15;
	public float highSpeedSteerAngle = 1;

	public float decelerationSpeed = 30;

	public float currentSpeed;
	public float topSpeed = 150;

	void Start () {
		rigidbody.centerOfMass = new Vector3(
			rigidbody.centerOfMass.x + centerOfMassOffset.x,
			rigidbody.centerOfMass.y + centerOfMassOffset.y,
			rigidbody.centerOfMass.z + centerOfMassOffset.z
		);
	}

	void FixedUpdate () {
		Control();
	}


	void Update(){
		//Rotating wheel transform according to wheel collider physics
		wheelFLTransform.Rotate(wheelFL.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelFRTransform.Rotate(wheelFR.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelRLTransform.Rotate(wheelRL.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelRRTransform.Rotate(wheelRR.rpm / 60*360*Time.deltaTime, 0, 0);


		//Steering (Visual)
		Vector3 temp = wheelFLTransform.localEulerAngles;
		temp.y = wheelFL.steerAngle - wheelFLTransform.localEulerAngles.z;
		wheelFLTransform.localEulerAngles = temp;


		temp = wheelFRTransform.localEulerAngles;
		temp.y = wheelFR.steerAngle - wheelFRTransform.localEulerAngles.z;
		wheelFRTransform.localEulerAngles = temp;

	}


	void Control(){
		float steerInput = Input.GetAxis("Horizontal");
	

		currentSpeed = Mathf.Round(2.0f * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000);

		//Adding motor torque
		if(currentSpeed <= topSpeed){
			wheelRR.motorTorque = maxTorque * Input.GetAxis("Vertical");
			wheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");
		} else {
			
			wheelRR.motorTorque = 0;
			wheelRL.motorTorque = 0;
		}

		//Drag deceleration
		if(!Input.GetButton("Vertical")){
			wheelRR.brakeTorque = decelerationSpeed;
			wheelRL.brakeTorque = decelerationSpeed;
		} else {
			wheelRR.brakeTorque = 0;
			wheelRL.brakeTorque = 0;
		}

		//Steering (pyhiscs)
		float speedFactor = rigidbody.velocity.magnitude / lowestSteerAtSpeed;
		float steerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor) * steerInput;
		wheelFL.steerAngle = steerAngle;
		wheelFR.steerAngle = steerAngle;
	}
}
