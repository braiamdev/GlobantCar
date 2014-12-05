using UnityEngine;
using System.Collections;

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
	
		float wantedAngle = rotationVector.y;
		float wantedHeight = car.position.y + height;
		float myAngle = transform.eulerAngles.y;
		float myHeight = transform.position.y;


		myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping * Time.deltaTime);
		myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping * Time.deltaTime);


		Quaternion currentRotation = Quaternion.Euler(0,myAngle,0);


		transform.position = car.position - (currentRotation * Vector3.forward * distance);
		transform.position = new Vector3(
			transform.position.x,
			myHeight,
			transform.position.z
		);


		transform.LookAt(car);


	}

	void FixedUpdate(){
		Vector3 localVelocity = car.InverseTransformDirection(car.rigidbody.velocity);
		if(localVelocity.z < reverseCamThreshold){
			rotationVector.y = car.eulerAngles.y + 180;
		} else {
			rotationVector.y = car.eulerAngles.y;
		}
		float acc = car.rigidbody.velocity.magnitude;
		camera.fieldOfView = defaultFOV + acc * zoomRatio;
	}
}
