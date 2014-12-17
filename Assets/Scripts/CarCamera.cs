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


	void LateUpdate () {

		//Calculating car state
		Vector3 localVelocity = car.InverseTransformDirection(car.rigidbody.velocity);
		bool isReverseCam = localVelocity.z < reverseCamThreshold;

		//Caching current camera rotation
		Quaternion currentRotation = transform.rotation;

		//Calculating target camera rotation
		Quaternion targetRotation = Quaternion.LookRotation((car.forward * (isReverseCam? -1 : 1)), car.up);
		
		//Rotating camera
		transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, rotationDamping * Time.deltaTime);
		



		//Positioning target position in the center of the car
		Vector3 targetPosition = car.position;

		//displacing target position behind (or in front of) the car
		targetPosition = targetPosition - (transform.forward * distance );

		//displacing target position above the car
		targetPosition = targetPosition  + (transform.up * height);


		//Moving the camera position to the target position
		transform.position = targetPosition;





	}


	void FixedUpdate(){
		float acc = car.rigidbody.velocity.magnitude;
		camera.fieldOfView = defaultFOV + acc * zoomRatio;
	}

}
