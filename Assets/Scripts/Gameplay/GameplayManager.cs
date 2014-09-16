using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Handles
public class GameplayManager : MonoBehaviour {
	private Dictionary<string, DisplayHandler> m_displays; //Hold all the displays for ease of access
	public string displayToStartWith;// For Testing

	private string m_currDisplay;

	void Awake () {
		//Add all the necessary components
		LocationHandler locationHandler = gameObject.AddComponent<LocationHandler> ();
		locationHandler.Init ();

		//TODO; Instantiate the situation, characters and their initial goals


		//Set up displays
		m_displays = new Dictionary<string,DisplayHandler >();
		DisplayHandler[] displayChildren = gameObject.GetComponentsInChildren<DisplayHandler>();

		foreach (DisplayHandler display in displayChildren) {
			display.Init();
			display.HideDisplay();
			m_displays.Add(display.DisplayName,display);
		}

		//Start the display to start with
		if (m_displays.ContainsKey (displayToStartWith)) {
			m_currDisplay = displayToStartWith;
			m_displays[m_currDisplay].SwitchToDisplay();
		}
		else{
			Debug.LogError("GameplayManager Error: displayToStartWith did not match a Handler name"); 
		}
	}

		
	// Update is called once per frame
	void Update () {
		//Handles the Display
		if(m_displays.ContainsKey(m_currDisplay)){
			string nextDisplay = m_displays [m_currDisplay].UpdateDisplay ();

			if (!string.IsNullOrEmpty (nextDisplay) && !nextDisplay.Equals (m_currDisplay) && m_displays.ContainsKey(nextDisplay)) {
				SwitchDisplays(nextDisplay);
			}
		}

		//TODO; Determine what state we are in (Player Moving to Goal, Player Chatting, A.I. Replanning)

		//TODO; Implement waiting/timer for when player is moving to goal

		//TODO; Implement chatbot whatever

		//TODO; Implement A.I. Replanning and all content for plans
	}

	private void SwitchDisplays(string nextDisplay){
		//TODO; Might want to add a nice fade in/out effect later
		m_displays [m_currDisplay].SwitchFromDisplay ();

		m_currDisplay = nextDisplay;
		m_displays [m_currDisplay].SwitchToDisplay ();
	}



}
