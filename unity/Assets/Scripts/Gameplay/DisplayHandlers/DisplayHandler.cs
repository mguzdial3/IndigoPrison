using UnityEngine;
using System.Collections;

public class DisplayHandler : MonoBehaviour {
	private string m_displayName;
	public string DisplayName { get { return m_displayName; } }

	//Gets the things name for now, but can Init other stuff
	public virtual void Init(){
		if (string.IsNullOrEmpty (m_displayName)) {
			m_displayName = gameObject.name;		
		}
	}

	//Called before switching to display
	public virtual void SwitchToDisplay(){
		gameObject.SetActive (true);
	}

	//Called during display-- to be overwritten (sends back the string name of the next display to use)
	public virtual string UpdateDisplay(){
		return null;
	}

	//Called before switching from display
	public virtual void SwitchFromDisplay(){
		gameObject.SetActive (false);
	}

	public void HideDisplay(){
		gameObject.SetActive (false);
	}

}
