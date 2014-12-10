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
	public float topSpeed = 50;
	public float lowSpeedSteerAngle = 15;
	public float highSpeedSteerAngle = 1;

	public float decelerationSpeed = 30;


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
		wheelFLTransform.Rotate(wheelFL.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelFRTransform.Rotate(wheelFR.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelRLTransform.Rotate(wheelRL.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelRRTransform.Rotate(wheelRR.rpm / 60*360*Time.deltaTime, 0, 0);

		Vector3 temp = wheelFLTransform.localEulerAngles;
		temp.y = wheelFL.steerAngle - wheelFLTransform.localEulerAngles.z;
		wheelFLTransform.localEulerAngles = temp;


		temp = wheelFRTransform.localEulerAngles;
		temp.y = wheelFR.steerAngle - wheelFRTransform.localEulerAngles.z;
		wheelFRTransform.localEulerAngles = temp;

	}


	void Control(){
		float steerInput = Input.GetAxis("Horizontal");
		
		wheelRR.motorTorque = maxTorque * Input.GetAxis("Vertical");
		wheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");
		
		if(!Input.GetButton("Vertical")){
			wheelRR.brakeTorque = decelerationSpeed;
			wheelRL.brakeTorque = decelerationSpeed;
		} else {
			wheelRR.brakeTorque = 0;
			wheelRL.brakeTorque = 0;
		}
		
		float speedFactor = rigidbody.velocity.magnitude / topSpeed;
		float steerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor) * steerInput;
		wheelFL.steerAngle = steerAngle;
		wheelFR.steerAngle = steerAngle;
	}
}
