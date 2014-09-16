using UnityEngine;
using System;

[RequireComponent (typeof(GUITexture))]
public class UIButton : MonoBehaviour {
	public GameObject target;
	public string OnButtonClicked="ButtonClick"; 

	public Texture pressedTexture;
	private Texture m_normalTexture;

	private bool wasOn;

	void Start(){
		//Grab the texture we currently have as normal
		m_normalTexture = guiTexture.texture;
	}

	void Update(){
		if (Input.GetMouseButtonDown (0) && guiTexture.HitTest(Input.mousePosition)) {
			//Pressed display
			if (pressedTexture != null) {
				guiTexture.texture = pressedTexture;		
			}
			else{
				Color pressedColor = guiTexture.color;
				pressedColor.a/=2;
				guiTexture.color = pressedColor;
			}
			wasOn=true;
		}

		if (Input.GetMouseButtonUp (0) && wasOn){
			wasOn=false;
			//Return to normal display
			if (m_normalTexture != null) {
				guiTexture.texture = m_normalTexture;		
			}
			if(pressedTexture==null){
				Color normalColor = guiTexture.color;
				normalColor.a*=2;
				guiTexture.color = normalColor;
			}

			if(guiTexture.HitTest(Input.mousePosition)){
				if (target != null) {
					target.SendMessage(OnButtonClicked,this, SendMessageOptions.DontRequireReceiver);		
				}
			}
		}
	}
}
