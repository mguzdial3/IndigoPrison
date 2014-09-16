using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUIText))]
public class ScaleGUIText : MonoBehaviour {
	public float oldScreenWidth = 480;
	public float oldScreenHeight = 320f;

	public bool xDependent=true;

	// Use this for initialization
	void Start () {
		float xScalar = Screen.width / oldScreenWidth;
		float yScalar = Screen.height / oldScreenHeight;
		
		Vector3 oldPixelOffset = guiText.pixelOffset;
		
		
		if (xDependent) {//Width determines height
			yScalar = xScalar;
		}
		else{//Height determines width
			xScalar = yScalar;
		}
		
		guiText.pixelOffset = new Vector2 (oldPixelOffset.x * xScalar, oldPixelOffset.y * yScalar);
		guiText.fontSize = (int)(guiText.fontSize * xScalar);

		Destroy (this);
	}
	

}
