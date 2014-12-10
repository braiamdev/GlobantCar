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

	[HideInInspector]
	public float currentSpeed;
	public float topSpeed = 150;
	public float maxReverseSpeed = 50;


	public bool braked = false;
	public float maxBrakeTorque =  100;


	void Start () {
		rigidbody.centerOfMass = new Vector3(
			rigidbody.centerOfMass.x + centerOfMassOffset.x,
			rigidbody.centerOfMass.y + centerOfMassOffset.y,
			rigidbody.centerOfMass.z + centerOfMassOffset.z
		);
	}

	void FixedUpdate () {
		Control();
		HandBrake();
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


		WheelPosition();

	}


	void Control(){
		float steerInput = Input.GetAxis("Horizontal");
	

		currentSpeed = Mathf.Round(2.0f * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000);

		//Adding motor torque
		if(currentSpeed >= -maxReverseSpeed && currentSpeed <= topSpeed){
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


	private void WheelPosition(){
		RaycastHit hit;
		Vector3 wheelPos;

		//FL
		if(Physics.Raycast(wheelFL.transform.position, -wheelFL.transform.up, out hit, wheelFL.radius + wheelFL.suspensionDistance)){
			wheelPos = hit.point + wheelFL.transform.up * wheelFL.radius;
		} else {
			wheelPos = wheelFL.transform.position - wheelFL.transform.up * wheelFL.suspensionDistance;
		}

		wheelFLTransform.position = wheelPos;


		//FR
		if(Physics.Raycast(wheelFR.transform.position, -wheelFR.transform.up, out hit, wheelFR.radius + wheelFR.suspensionDistance)){
			wheelPos = hit.point + wheelFR.transform.up * wheelFR.radius;
		} else {
			wheelPos = wheelFR.transform.position - wheelFR.transform.up * wheelFR.suspensionDistance;
		}
		
		wheelFRTransform.position = wheelPos;


		//RL
		if(Physics.Raycast(wheelRL.transform.position, -wheelRL.transform.up, out hit, wheelRL.radius + wheelRL.suspensionDistance)){
			wheelPos = hit.point + wheelRL.transform.up * wheelRL.radius;
		} else {
			wheelPos = wheelRL.transform.position - wheelRL.transform.up * wheelRL.suspensionDistance;
		}
		
		wheelRLTransform.position = wheelPos;



		//RR
		if(Physics.Raycast(wheelRR.transform.position, -wheelRR.transform.up, out hit, wheelRR.radius + wheelRR.suspensionDistance)){
			wheelPos = hit.point + wheelRR.transform.up * wheelRR.radius;
		} else {
			wheelPos = wheelRR.transform.position - wheelRR.transform.up * wheelRR.suspensionDistance;
		}
		
		wheelRRTransform.position = wheelPos;
	

	}


	public void HandBrake(){
		if(Input.GetButton("Jump")){
			braked = true;
		} else {
			braked = false;
		}

		if(braked){
			wheelRR.brakeTorque = maxBrakeTorque;
			wheelRL.brakeTorque = maxBrakeTorque;
			wheelFL.motorTorque = 0;
			wheelFR.motorTorque = 0;
		}
	}
}
