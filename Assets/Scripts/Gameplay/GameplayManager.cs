using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Handles
public class GameplayManager : MonoBehaviour {
	private Dictionary<string, DisplayHandler> m_displays; //Hold all the displays for ease of access
	public string displayToStartWith;// For Testing


	private string m_currDisplay;

	private DramaManager m_dramaManager;

	//TODO; get rid of after demo
	public static GameplayManager Instance;

	void Awake () {
		//Add all the necessary components
		LocationHandler locationHandler = gameObject.AddComponent<LocationHandler> ();
		locationHandler.Init ();

		//Set up displays
		m_displays = new Dictionary<string,DisplayHandler >();
		DisplayHandler[] displayChildren = gameObject.GetComponentsInChildren<DisplayHandler>();

		foreach (DisplayHandler display in displayChildren) {
			display.Init();
			display.HideDisplay();
			m_displays.Add(display.DisplayName,display);
		}

		//TODO; Instantiate the situation, characters and their initial goals
		m_dramaManager = new DramaManager ();

		//Start the display to start with
		if (m_displays.ContainsKey (displayToStartWith)) {
			m_currDisplay = displayToStartWith;
			m_displays[m_currDisplay].SwitchToDisplay();
		}
		else{
			Debug.LogError("GameplayManager Error: displayToStartWith did not match a Handler name"); 
		}

		ConversationHandler.Instance.AddLine (m_dramaManager.GetPrisoner (), "Hey, can't explain, but I need you to help me escape.");
		ConversationHandler.Instance.AddLine (m_dramaManager.GetPrisoner (), "I've marked an info point on your map. Grab that for me, please");

		MapHandler.Instance.AddIndicator ("Prisoner Goal", MapHandler.ITEM_ICON, new Vector2 (Screen.width * 0.1f, Screen.height / 2));
		Instance = this;
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

	//TODO; REMOVE: THIS IS FOR TESTING
	private static int currDemo = 0;
	public void UpdateDemo(){
		currDemo++;
		if (currDemo == 1) {
			ConversationHandler.Instance.AddLine (m_dramaManager.GetGuard (), "I don't know what the prisoner told you. But you can't help them.");
			ConversationHandler.Instance.AddLine (m_dramaManager.GetGuard (), "We're on lockdown so I can't move. But you need to head to the other end of the prison");

			MapHandler.Instance.AddIndicator("Guard Goal", MapHandler.ITEM_ICON, new Vector2(Screen.width*0.9f,Screen.height/2));
		}
		else if(currDemo==2){
			MapHandler.Instance.DestroyIndicator("Guard Goal");
			MapHandler.Instance.DestroyIndicator("Prisoner Goal");

			ConversationHandler.Instance.AddLine (m_dramaManager.GetPrisoner (), "You got it! Bring it back to me!");
		}
		else if(currDemo==3){
			
			ConversationHandler.Instance.AddLine (m_dramaManager.GetPrisoner (), "I escaped! The end!");
		}


	}




}
