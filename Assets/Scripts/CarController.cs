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


	//Drivetrain
	public DrivetrainType drivetrainType = DrivetrainType.FWD;

	//Gears
	public float[] GearRatios;
	public int currentGear = 0;
	public float EngineRPM { get; private set;}
	
	public float maxEngineRPM = 3000f;
	public float minEngineRPM = 1000f;

	public float EngineTorque = 500f;
	

	//Max speeds
	public float topForwardSpeed = 150;
	public float topReverseSpeed = 50;




	//State vars
	public float currentSpeed { get; private set;}

	private float inputAcceleration = 0;
	private float inputSteering = 0;
	public bool IsHandbraking;


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
		this.IsHandbraking = handBrake;
	}


	public void HardStop(){
		ApplyToAllWheels((wheelTransform, wheelCollider) => {
			wheelCollider.motorTorque = 0;
			wheelCollider.brakeTorque = 0;
		});
	}


	public void ShiftGearUp(){
		if(currentGear < GearRatios.Length - 1)
			currentGear++;
	}

	public void ShiftGearDown(){
		if(currentGear > 0)
			currentGear--;
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

	private void ApplyToDrivetrainWheels(WheelsFunction f){
		switch(drivetrainType){
		case DrivetrainType.FWD:
			ApplyToFrontWheels(f);
			break;

		case DrivetrainType.RWD:
			ApplyToRearWheels(f);
			break;

		case DrivetrainType.AWD:
			ApplyToAllWheels(f);
			break;
		}
	}

	private void ApplyToNonDrivetrainWheels(WheelsFunction f){
		switch(drivetrainType){
		case DrivetrainType.FWD:
			ApplyToRearWheels(f);
			break;
			
		case DrivetrainType.RWD:
			ApplyToFrontWheels(f);
			break;
		}
	}


	private void UpdatePhysics(){
		
		currentSpeed = Mathf.Round(2.0f * Mathf.PI * wheelColliderFL.radius * wheelColliderFL.rpm * 60 / 1000);
		//bool thing = currentSpeed >= -topReverseSpeed && currentSpeed <= topForwardSpeed;
		//Debug.Log(string.Format("currentSpeed={0}; {1}", currentSpeed, thing));

		//Updating engine RPM
		UpdateEngineRPM();
		ShiftGears();

		//Adding motor torque
		ApplyToDrivetrainWheels((wheelTransform, wheelCollider) => {
			if(EngineRPM <= maxEngineRPM){
				wheelCollider.motorTorque = (EngineTorque / GearRatios[currentGear]) * this.inputAcceleration;
			} else {
				wheelCollider.motorTorque = 0;
			}
		});

		Debug.Log(string.Format("engineRPM={0}; currentGear={1}; currentSpeed={2}; motorTorque={3}", EngineRPM, currentGear, currentSpeed, wheelColliderFL.motorTorque));

		
		//Adding motor torque
//		if(currentSpeed >= -topReverseSpeed && currentSpeed <= topForwardSpeed){
//			ApplyToDrivetrainWheels((wheelTransform, wheelCollider) => {
//				wheelCollider.motorTorque = maxAccelTorque * this.inputAcceleration;
//			});
//
//		} else {
//			ApplyToDrivetrainWheels((wheelTransform, wheelCollider) => {
//				wheelCollider.motorTorque = 0;
//			});
//		}

		
		//Drag deceleration
		if(inputAcceleration == 0){
			ApplyToDrivetrainWheels((wheelTransform, wheelCollider) => wheelCollider.brakeTorque = decelerationSpeed);
		} else {
			ApplyToDrivetrainWheels((wheelTransform, wheelCollider) => wheelCollider.brakeTorque = 0);
		}
		
		//Steering (pyhiscs)
		float speedFactor = rigidbody.velocity.magnitude / lowestSteerAtSpeed;
		float steerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor) * this.inputSteering;
		ApplyToFrontWheels((wheelTransform, wheelCollider) => wheelCollider.steerAngle = steerAngle);
		
		//Handbrake
		if(IsHandbraking){
			ApplyToDrivetrainWheels((wheelTransform, wheelCollider) =>  wheelCollider.motorTorque = 0 );
			ApplyToRearWheels((wheelTransform, wheelCollider) => wheelCollider.brakeTorque = maxBrakeTorque );
		} else {
			ApplyToRearWheels((wheelTransform, wheelCollider) => wheelCollider.brakeTorque = 0 );
		}
	}

	private void UpdateEngineRPM(){
		//Calculating average drivetrain wheels rpm
		float avgWheelsRPM = 0;
		ApplyToDrivetrainWheels((wheelTransform, wheelCollider) => avgWheelsRPM += wheelCollider.rpm);
		avgWheelsRPM = avgWheelsRPM / (drivetrainType == DrivetrainType.AWD ? 4 : 2);

		//Calculating engine rpm
		EngineRPM = avgWheelsRPM * GearRatios[currentGear];
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


	private void ShiftGears() {
		if ( EngineRPM >= maxEngineRPM ) {
			for ( int i = 0; i < GearRatios.Length; i ++ ) {
				if ( wheelColliderFL.rpm * GearRatios[i] < maxEngineRPM ) {
					currentGear = i;
					break;
				}
			}
		} else if ( EngineRPM <= minEngineRPM ) {
			for (int i = GearRatios.Length-1; i >= 0; i--){
				if ( wheelColliderFL.rpm * GearRatios[i] > minEngineRPM ) {
					currentGear = i;
					break;
				}
			}
		}
	}
}
