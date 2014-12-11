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
	private bool braked = false;
	private float currentSpeed;


	public Texture2D speedOMeterDial;
	public Texture2D speedOMeterPointer;


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
		Control();
		HandBrake();
	}


	void Update(){
		SyncWheelTransformsToColliderProperties();
	}


	void Control(){
		float steerInput = Input.GetAxis("Horizontal");
	

		currentSpeed = Mathf.Round(2.0f * Mathf.PI * wheelColliderFL.radius * wheelColliderFL.rpm * 60 / 1000);

		//Adding motor torque
		if(currentSpeed >= -topReverseSpeed && currentSpeed <= topForwardSpeed){
			wheelColliderRR.motorTorque = maxAccelTorque * Input.GetAxis("Vertical");
			wheelColliderRL.motorTorque = maxAccelTorque * Input.GetAxis("Vertical");
		} else {
			
			wheelColliderRR.motorTorque = 0;
			wheelColliderRL.motorTorque = 0;
		}

		//Drag deceleration
		if(!Input.GetButton("Vertical")){
			wheelColliderRR.brakeTorque = decelerationSpeed;
			wheelColliderRL.brakeTorque = decelerationSpeed;
		} else {
			wheelColliderRR.brakeTorque = 0;
			wheelColliderRL.brakeTorque = 0;
		}

		//Steering (pyhiscs)
		float speedFactor = rigidbody.velocity.magnitude / lowestSteerAtSpeed;
		float steerAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, speedFactor) * steerInput;
		wheelColliderFL.steerAngle = steerAngle;
		wheelColliderFR.steerAngle = steerAngle;
	}




	public void HandBrake(){
		if(Input.GetButton("Jump")){
			braked = true;
		} else {
			braked = false;
		}

		if(braked){
			wheelColliderRR.brakeTorque = maxBrakeTorque;
			wheelColliderRL.brakeTorque = maxBrakeTorque;
			wheelColliderFL.motorTorque = 0;
			wheelColliderFR.motorTorque = 0;
		}
	}


	void OnGUI(){

		//Drawing Dial
		float dialWidth = 300;
		float dialHeight = 150;

		Rect dialRect = new Rect(
			Screen.width - dialWidth,
			Screen.height - dialHeight,
			dialWidth,
			dialHeight
		);

		GUI.DrawTexture(dialRect, speedOMeterDial);


		//Drawing needle
		float speedFactor = Mathf.Abs(currentSpeed / topForwardSpeed);

		float rotationAngle = Mathf.Lerp(0, 180, speedFactor);

		float needleWidth = 300;
		float needleHeight = 300;

		Rect needleRect = new Rect(
			Screen.width - needleWidth,
			Screen.height - (needleHeight / 2),
			needleWidth,
			needleHeight
		);

		Vector2 pivotPoint = new Vector2(
			Screen.width - (needleWidth/2),
			Screen.height
		);
		GUIUtility.RotateAroundPivot(rotationAngle, pivotPoint);
		GUI.DrawTexture(needleRect, speedOMeterPointer);

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



	/// <summary>
	/// Syncs the wheel transforms to collider properties.
	/// </summary>
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
