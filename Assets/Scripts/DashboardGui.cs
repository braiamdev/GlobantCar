using UnityEngine;
using System.Collections;

public class DashboardGui : MonoBehaviour {

	public CarController carController;

	public Rect speedometerDimensions;
	
	public Texture2D speedOMeterDial;
	public Texture2D speedOMeterPointer;

	public float speedometerAngleSpan = 90;
	public float speedometerRotation = 0;


	//
	// MonoBehaviour Overrides
	//
	
	void OnGUI(){
		DrawDashboard();
	}


	//
	// DelegateMethods
	//

	private void DrawDashboard(){
		
		//Drawing Dial

		
		Rect dialRect = new Rect(
			Screen.width - speedometerDimensions.width - speedometerDimensions.x,
			Screen.height - speedometerDimensions.height - speedometerDimensions.y,
			speedometerDimensions.width,
			speedometerDimensions.height
			);
		
		GUI.DrawTexture(dialRect, speedOMeterDial);
		
		
		//Drawing needle
		float speedFactor = Mathf.Abs(carController.currentSpeed / carController.topForwardSpeed);
		
		float rotationAngle = Mathf.Lerp(0, speedometerAngleSpan, speedFactor) + speedometerRotation;

		Rect needleRect = dialRect;
		
		Vector2 pivotPoint = new Vector2(
			dialRect.x + (dialRect.width / 2),
			dialRect.y + (dialRect.height / 2)
			);
		GUIUtility.RotateAroundPivot(rotationAngle, pivotPoint);
		GUI.DrawTexture(needleRect, speedOMeterPointer);

	}

}
