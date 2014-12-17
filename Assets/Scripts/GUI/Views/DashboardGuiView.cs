using UnityEngine;
using System.Collections;

public class DashboardGuiView : MonoBehaviour {

	public CarController carController;

	public DialIndicatorVO speedometer = new DialIndicatorVO();
	public DialIndicatorVO tachometer = new DialIndicatorVO();
	public Rect GearIndicatorRect;
	public Texture2D GearIndicatorTexture;

	public Texture2D StickyModeOnTexture;
	public Texture2D StickyModeOffTexture;
	public Rect StickyLightRect;


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

		//updating speedometer value and drawing it
		speedometer.value = Mathf.Abs(carController.currentSpeed / carController.topForwardSpeed);
		DrawDialIndicator(speedometer);

		//updating tachometer value and drawing it
		tachometer.value = Mathf.Abs(carController.EngineRPM / carController.maxEngineRPM);
		DrawDialIndicator(tachometer);


		//Drawing current gear number
		Rect rect = new Rect(
			Screen.width - GearIndicatorRect.width - GearIndicatorRect.x,
			Screen.height - GearIndicatorRect.height - GearIndicatorRect.y,
			GearIndicatorRect.width,
			GearIndicatorRect.height
		);
		GUI.DrawTextureWithTexCoords(
				rect,
				GearIndicatorTexture,
				new Rect(((float)carController.currentGear + 1)/10, 0, 0.1f, 1)
		);


		
		//Drawing sticky light indicator
		Rect stickyLightRect = new Rect(
			Screen.width - StickyLightRect.width - StickyLightRect.x,
			Screen.height - StickyLightRect.height - StickyLightRect.y,
			StickyLightRect.width,
			StickyLightRect.height
			);
		
		Texture2D stickyLightTexture = carController.isSticky ? StickyModeOnTexture : StickyModeOffTexture;
		GUI.DrawTexture(stickyLightRect, stickyLightTexture);

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
		GUI.matrix = Matrix4x4.identity;

	}


}
