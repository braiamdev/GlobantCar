using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour {

	//Wheel collider game objects
	public WheelCollider wheelColliderFL;
	public WheelCollider wheelColliderFR;
	public WheelCollider wheelColliderRL;
	public WheelCollider wheelColliderRR;


	//Wheel Transform game objects
	public Transform wheelTransformFL;
	public Transform wheelTransformFR;
	public Transform wheelTransformRL;
	public Transform wheelTransformRR;


	//Max torque
	public float maxAccelTorque = 50f;
	public float maxBrakeTorque =  100f;


	//Physics
	public Vector3 centerOfMassOffset = new Vector3(0f,-0.9f, 0f);
	public float decelerationSpeed = 30;


	//Steering
	public float lowestSteerAtSpeed = 50; //the speed at witch highSpeedSteerAngle will be reached
	public float lowSpeedSteerAngle = 15;
	public float highSpeedSteerAngle = 1;




	//Max speeds
	public float topForwardSpeed = 150;
	public float topReverseSpeed = 50;




	//State vars
	public float currentSpeed { get; private set;}

	private float inputAcceleration = 0;
	private float inputSteering = 0;
	private bool isHandBraking = false;


	private delegate void WheelsFunction(Transform wheelTransform, WheelCollider wheelCollider);


	//
	// Monobehaviour Overrides
	//

	void Start () {
		rigidbody.centerOfMass = new Vector3(
			rigidbody.centerOfMass.x + centerOfMassOffset.x,
			rigidbody.centerOfMass.y + centerOfMassOffset.y,
			rigidbody.centerOfMass.z + centerOfMassOffset.z
		);
	}

	void FixedUpdate () {
		UpdatePhysics();
	}


	void Update(){
		SyncWheelTransformsToColliderProperties();
	}





	//
	// Public API
	//

	public void ApplyAcceleration(float acceleration){
		this.inputAcceleration = acceleration;
	}


	public void ApplySteering(float steering){
		this.inputSteering = steering;
	}

	public void SetHandbrake(bool handBrake){
		this.isHandBraking = handBrake;
	}

	public void HardStop(){
		ApplyToAllWheels((wheelTransform, wheelCollider) => {
			wheelCollider.motorTorque = 0;
			wheelCollider.brakeTorque = 0;
		});
	}
	
	//
	// Delegate Methods
	//
	private void ApplyToAllWheels(WheelsFunction f){
		f(wheelTransformFL, wheelColliderFL);
		f(wheelTransformFR, wheelColliderFR);
		f(wheelTransformRL, wheelColliderRL);
		f(wheelTransformRR, wheelColliderRR);
	}
	
	private void ApplyToFrontWheels(WheelsFunction f){
		f(wheelTransformFL, wheelColliderFL);
		f(wheelTransformFR, wheelColliderFR);
	}
	
	private void ApplyToRearWheels(WheelsFunction f){
		f(wheelTransformRL, wheelColliderRL);
		f(wheelTransformRR, wheelColliderRR);
	}



	private void UpdatePhysics(){
		
		currentSpeed = Mathf.Round(2.0f * Mathf.PI * wheelColliderFL.radius * wheelColliderFL.rpm * 60 / 1000);
		//bool thing = currentSpeed >= -topReverseSpeed && currentSpeed <= topForwardSpeed;
		//Debug.Log(string.Format("currentSpeed={0}; {1}", currentSpeed, thing));

		//Adding motor torque
		if(currentSpeed >= -topReverseSpeed && currentSpeed <= topForwardSpeed){
			ApplyToRearWheels((wheelTransform, wheelCollider) => {
				wheelCollider.motorTorque = maxAccelTorque * this.inputAcceleration;
			});

		} else {
			ApplyToRearWheels((wheelTransform, wheelCollider) => {
				wheelCollider.motorTorque = 0;
			});
		}

		
		//Drag deceleration
		if(inputAcceleration == 0){
			ApplyToRearWheels((wheelTransform, wheelCollider) => wheelCollider.brakeTorque = decelerationSpeed);
		} else {
			ApplyToRearWheels((wheelTransform, wheelCollider) => wheelCollider.brakeTorque = 0);
		}
		
		//Steering (pyhiscs)
		float speedFactor = rigidbody.velocity.magnitude / lowestSteerAtSpeed;
		float steerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor) * this.inputSteering;
		ApplyToFrontWheels((wheelTransform, wheelCollider) => wheelCollider.steerAngle = steerAngle);
		
		//Handbrake
		if(isHandBraking){
			ApplyToFrontWheels((wheelTransform, wheelCollider) =>  wheelCollider.motorTorque = 0 );
			ApplyToRearWheels((wheelTransform, wheelCollider) => wheelCollider.brakeTorque = maxBrakeTorque );
		}
	}



	private void SyncWheelTransformsToColliderProperties(){

		//Rotating wheel transform according to wheel collider physics
		ApplyToAllWheels((wheelTransform, wheelCollider) => {
			wheelTransform.Rotate(wheelCollider.rpm / 60*360*Time.deltaTime, 0, 0);
		});
		
		
		//Steering (Visual)
		ApplyToFrontWheels((wheelTransform, wheelCollider) => {
			Vector3 temp = wheelTransform.localEulerAngles;
			temp.y = wheelCollider.steerAngle - wheelTransform.localEulerAngles.z;
			wheelTransform.localEulerAngles = temp;
		});
		
		
		//Wheel position (suspension)
		ApplyToAllWheels((wheelTransform, wheelCollider) => {
			RaycastHit hit;
			Vector3 wheelPos;
			
			if(Physics.Raycast(wheelCollider.transform.position, - wheelCollider.transform.up, out hit, wheelCollider.radius + wheelCollider.suspensionDistance)){
				wheelPos = hit.point + wheelCollider.transform.up * wheelCollider.radius;
			} else {
				wheelPos = wheelCollider.transform.position - wheelCollider.transform.up * wheelCollider.suspensionDistance;
			}
			
			wheelTransform.position = wheelPos;
		});
	}

}
