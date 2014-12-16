using UnityEngine;
using System;

public class HelpGuiView : MonoBehaviour {
	public string helpText;
	void OnGUI(){
		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.Label(helpText);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

}


