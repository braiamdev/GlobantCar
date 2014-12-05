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
	public float maxSteerAngle = 15f;
	public Vector3 centerOfMassOffset = new Vector3(0f,-0.9f, 0f);

	void Start () {
		rigidbody.centerOfMass = new Vector3(
			rigidbody.centerOfMass.x + centerOfMassOffset.x,
			rigidbody.centerOfMass.y + centerOfMassOffset.y,
			rigidbody.centerOfMass.z + centerOfMassOffset.z
		);
	}

	void FixedUpdate () {
		wheelRR.motorTorque = maxTorque * Input.GetAxis("Vertical");
		wheelRL.motorTorque = maxTorque * Input.GetAxis("Vertical");

		wheelFL.steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
		wheelFR.steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");

	}


	void Update(){
		wheelFLTransform.Rotate(wheelFL.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelFRTransform.Rotate(wheelFR.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelRLTransform.Rotate(wheelRL.rpm / 60*360*Time.deltaTime, 0, 0);
		wheelRRTransform.Rotate(wheelRR.rpm / 60*360*Time.deltaTime, 0, 0);
	}
}
