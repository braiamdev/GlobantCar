using UnityEngine;
using System;

public class Respawner : MonoBehaviour
{
	public GameObject respawnPoint;
	public string triggerButtonName = "Fire1";

	void Update(){
		if(Input.GetButtonDown(triggerButtonName)){
			respawn();
		}
	}


	private void respawn(){
		gameObject.transform.position = respawnPoint.transform.position;
		gameObject.transform.rotation = respawnPoint.transform.rotation;
	}
}


