using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Indigo;

//Handles
public class GameplayManager : MonoBehaviour {
	private Dictionary<string, DisplayHandler> m_displays; //Hold all the displays for ease of access
	public string displayToStartWith;// For Testing
	
	private string m_currDisplay;

	private DramaManager m_dramaManager;
	private GameState currGameState;

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

		m_dramaManager = new DramaManager (Screen.width,Screen.height);
		currGameState = m_dramaManager.InitializeGameState ();

		//Start the display to start with
		if (m_displays.ContainsKey (displayToStartWith)) {
			m_currDisplay = displayToStartWith;
			m_displays[m_currDisplay].SwitchToDisplay();
		}
		else{
			Debug.LogError("GameplayManager Error: displayToStartWith did not match a Handler name"); 
		}

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

		//Make Changes to Our Internal GameState based on actual one
		Vector2 playerPosCurr = MapHandler.Instance.PlayerPos;
		currGameState.Player.SetLocation (playerPosCurr.x, playerPosCurr.y);

		currGameState = m_dramaManager.UpdateGameState (currGameState);

		UpdateDisplay ();

	}

	public void UpdateDisplay(){

		//CHARACTER DISPLAYS
		foreach (Character c in currGameState.Characters) {
			if(c.IsAlive() && !c.Hidden && !MapHandler.Instance.HasIndicator(c.Name)){
				int characterType = c.Name.Contains(DramaManager.GUARD_TITLE) ? MapHandler.GUARD_ICON: MapHandler.PRISONER_ICON;

				MapHandler.Instance.AddIndicator(c.Name,characterType,new Vector2(c.X,c.Y));
			}
			else if(((c.IsAlive() && c.Hidden) || (!c.IsAlive())) && MapHandler.Instance.HasIndicator(c.Name)){

				MapHandler.Instance.DestroyIndicator(c.Name);
			}
		}

		//ITEM DISPLAYS
		foreach (Item i in currGameState.Items) {
			if(i.Alive && !i.Hidden && !MapHandler.Instance.HasIndicator(i.Name)){
				MapHandler.Instance.AddIndicator(i.Name,MapHandler.ITEM_ICON,new Vector2(i.X,i.Y));
			}
			else if( ((i.Alive && i.Hidden) || (!i.Alive)) && MapHandler.Instance.HasIndicator(i.Name)){
				MapHandler.Instance.DestroyIndicator(i.Name);
			}
		}

		//CONVERSATION DISPLAYS
		foreach(KeyValuePair<string, List<DialogueLine>> kvp in currGameState.Conversations){
			if(ConversationHandler.Instance.HasConversation(kvp.Key)){
				int ourCount = ConversationHandler.Instance.GetConversationLength(kvp.Key);

				if(ourCount!=kvp.Value.Count){
					for(int i = ourCount; i<kvp.Value.Count; i++){
						ConversationHandler.Instance.AddLine(kvp.Key,kvp.Value[i]);
					}
				}

			}
			else{
				foreach(DialogueLine line in kvp.Value){
					ConversationHandler.Instance.AddLine(kvp.Key,line);
				}
			}
		}


	}

	private void SwitchDisplays(string nextDisplay){
		//TODO; Might want to add a nice fade in/out effect later
		m_displays [m_currDisplay].SwitchFromDisplay ();

		m_currDisplay = nextDisplay;
		m_displays [m_currDisplay].SwitchToDisplay ();
	}


}
