using UnityEngine;

[System.Serializable]
public class DialIndicatorVO {
	public Rect dimensions;
	
	public Texture2D dialTexture;
	public Texture2D needleTexture;
	
	public float angleSpan = 90;
	public float angleRotation = 0;

	public float value;
}