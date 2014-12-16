using UnityEngine;
using System.Collections;

public class DashboardGuiView : MonoBehaviour {

	public CarController carController;

	public DialIndicatorVO speedometer = new DialIndicatorVO();
	public DialIndicatorVO tachometer = new DialIndicatorVO();


	//
	// MonoBehaviour Overrides
	//
	
	void OnGUI(){

		speedometer.value = Mathf.Abs(carController.currentSpeed / carController.topForwardSpeed);


		DrawDashboard();
	}


	//
	// DelegateMethods
	//

	private void DrawDashboard(){
		DrawDialIndicator(speedometer);
	}

	private void DrawDialIndicator(DialIndicatorVO dialIndicatorVO){

		
		Rect dialRect = new Rect(
			Screen.width - dialIndicatorVO.dimensions.width - dialIndicatorVO.dimensions.x,
			Screen.height - dialIndicatorVO.dimensions.height - dialIndicatorVO.dimensions.y,
			dialIndicatorVO.dimensions.width,
			dialIndicatorVO.dimensions.height
			);
		
		GUI.DrawTexture(dialRect, dialIndicatorVO.dialTexture);


		float rotationAngle = Mathf.Lerp(0, dialIndicatorVO.angleSpan, dialIndicatorVO.value) + dialIndicatorVO.angleRotation;

		Rect needleRect = dialRect;
		
		Vector2 pivotPoint = new Vector2(
			dialRect.x + (dialRect.width / 2),
			dialRect.y + (dialRect.height / 2)
			);
		GUIUtility.RotateAroundPivot(rotationAngle, pivotPoint);
		GUI.DrawTexture(needleRect,	 dialIndicatorVO.needleTexture);

	}

}
