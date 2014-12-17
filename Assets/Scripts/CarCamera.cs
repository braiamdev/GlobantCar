using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarCamera : MonoBehaviour {

	public Transform car;
	public float distance = 6.4f;
	public float height = 1.4f;
	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;
	public float zoomRatio = 0.5f;
	public float reverseCamThreshold = -0.5f;

	public float defaultFOV = 60;



	private Vector3 rotationVector;
	private Vector3 cameraOrientation;

	void Start(){
		cameraOrientation = Vector3.forward;
	}

	void LateUpdate () {


		//Calculating camera position behind the car
//		float currentYAngle = transform.eulerAngles.y;
//		float targetYAngle = rotationVector.y;
//		currentYAngle = Mathf.LerpAngle(currentYAngle, targetYAngle, rotationDamping * Time.deltaTime);


		//Calculating camera position above the car
		//float currentHeight = transform.position.y;
		//float targetHeight = car.position.y + height;

		//Calculating rotation
		//Quaternion currentRotation = Quaternion.Euler(0,currentYAngle,0);




		//targetPosition = Mathf.Lerp();


		Vector3 currentPosition = transform.position;

		//Positioning target position in the center of the car
		Vector3 targetPosition = car.position;

		//displacing  target position behind the car
		targetPosition = targetPosition - (transform.forward * distance);


		//displacing target position above the car
		targetPosition = targetPosition  + (transform.up * height);

		Vector3 finalPosition = targetPosition ;//Vector3.Slerp(currentPosition, targetPosition, heightDamping * Time.deltaTime);
		transform.position = finalPosition;// targetPosition;



		Quaternion currentRotation = transform.rotation;
		Quaternion targetRotation = Quaternion.LookRotation(car.forward, car.up);

		transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, rotationDamping * Time.deltaTime);
		//transform.rotation = targetRotation ;//Quaternion.Slerp(currentRotation, targetRotation, rotationDamping * Time.deltaTime);



	}

	void FixedUpdate(){
//		Vector3 localVelocity = car.InverseTransformDirection(car.rigidbody.velocity);
//		if(localVelocity.z < reverseCamThreshold){
//			rotationVector.y = car.eulerAngles.y + 180;
//		} else {
//			rotationVector.y = car.eulerAngles.y;
//		}
//
//		float acc = car.rigidbody.velocity.magnitude;
//		camera.fieldOfView = defaultFOV + acc * zoomRatio;
	}

	void Update(){
		if(Input.GetButtonDown("NextCam")){
			NextCam();
		}
	}

	void NextCam(){
		if(cameraOrientation == Vector3.forward)
			cameraOrientation = Vector3.left;
		else if(cameraOrientation == Vector3.left)
			cameraOrientation = Vector3.back;
		else if(cameraOrientation == Vector3.back)
			cameraOrientation = Vector3.right;
		else if(cameraOrientation == Vector3.right)
			cameraOrientation = Vector3.forward;

		

	}
}
