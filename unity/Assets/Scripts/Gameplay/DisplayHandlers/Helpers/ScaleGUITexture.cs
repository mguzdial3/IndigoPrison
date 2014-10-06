using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUITexture))]
public class ScaleGUITexture : MonoBehaviour {
	public float oldScreenWidth = 520f;
	public float oldScreenHeight = 325f;

	public bool xDependent=true;

	// Use this for initialization
	void Start () {
		float xScalar = Screen.width / oldScreenWidth;
		float yScalar = Screen.height / oldScreenHeight;

		Rect oldPixelInset = guiTexture.pixelInset;


		if (xDependent) {//Width determines height
			yScalar = xScalar;
		}
		else{//Height determines width
			xScalar = yScalar;
		}

		guiTexture.pixelInset = new Rect (oldPixelInset.x * xScalar, oldPixelInset.y * yScalar, oldPixelInset.width * xScalar, oldPixelInset.height * yScalar);
	
		Destroy (this);
	}
}
