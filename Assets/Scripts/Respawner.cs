using UnityEngine;
using System;

public class Respawner : MonoBehaviour
{
	public CarController carController;
	public GameObject respawnPoint;
	public string triggerButtonName = "Respawn";

	private bool respawning = false;

	void Update(){
		if(Input.GetButtonDown(triggerButtonName)){
			respawning = true;
		}
	}


	private void respawn(){

		carController.gameObject.transform.position = respawnPoint.transform.position;
		carController.gameObject.transform.rotation = respawnPoint.transform.rotation;
		carController.HardStop();
	}


	void FixedUpdate(){
		if(respawning){
			respawn();
			//carController.rigidbody.isKinematic = false;
			respawning = false;
		} else {
			//carController.rigidbody.isKinematic = true;
		}
	}
}


