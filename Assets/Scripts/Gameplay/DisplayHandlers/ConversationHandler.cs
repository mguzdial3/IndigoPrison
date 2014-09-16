using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Using a singleton structure for these too so that interactions with other displays can affect this one
public class ConversationHandler : DisplayHandler {
	public static ConversationHandler Instance; //Singleton reference

	//Dictionary of conversation histories from character name to a list of lines of dialogue
	private Dictionary<string, List<DialogueLine>> m_conversationHistories;

	//The name of the player
	public static readonly string PLAYER_NAME = "Player";

	//List of ConversationDisplayUnits
	private List<ConversationDisplayUnit> m_conversationDisplays;
	public ConversationDisplayUnit conversationDisplayPrefab;

	//Check to transfer to map
	private bool m_TransferToMap = false;
	//Check to transfer to chat
	private bool m_TransferToChat = false;

	private const float DISTANCE_BETWEEN_CONVERSATIONS = 0.2f;

	//Initialize Dependencies
	public override void Init (){
		base.Init ();
		
		if (Instance == null) {
			Instance = this;		
		}

		m_conversationHistories = new Dictionary<string,List<DialogueLine> > ();
		m_conversationDisplays = new List<ConversationDisplayUnit> ();
	
		//TODO; remove test add stuff
		AddLine ("Sally Fisher", Color.cyan, new DialogueLine ("Sally Fisher", Color.cyan, "This is a test of testy testness. I tested the test testily to test my testing."));
		AddLine ("Sally Fisher", Color.cyan, new DialogueLine (PLAYER_NAME, Color.white, "I got the thing you wanted. That one thing, the special thing. You know? The thing."));
		AddLine ("Sally Fisher", Color.cyan, new DialogueLine ("Sally Fisher", Color.cyan, "We have a problem."));
		AddLine ("Sally Fisher", Color.cyan, new DialogueLine (PLAYER_NAME, Color.white, "Yo what's up?"));
	}

	public override string UpdateDisplay ()
	{
		if (m_TransferToMap) {
			m_TransferToMap=false;
			return MapHandler.Instance.DisplayName;
		}

		if (m_TransferToChat) {
			m_TransferToChat=false;
			return ChatHandler.Instance.DisplayName;
		}

		return base.UpdateDisplay ();
	}

	//Add character dialogue lines through here by characterName and the current dialogue line in that conversation
	public void AddLine(string characterName, Color characterColor, DialogueLine dialogue){
		if (!m_conversationHistories.ContainsKey(characterName)) { //New conversation
			m_conversationHistories.Add (characterName, new List<DialogueLine>());

			AddConversationDisplay(characterName, characterColor,dialogue);
		}
		else{
			UpdateConversationDisplay(characterName,characterColor,dialogue);
		}

		m_conversationHistories [characterName].Add (dialogue);

		//If ChatHandler is currently up, update it
		if (ChatHandler.Instance.IsCurrentCharacter (characterName)) {
			ChatHandler.Instance.UpdateList(dialogue);		
		}

	}

	///////////////
	/// 
	/// Conversation Displays
	public void ArrowUp(UIButton arrow){
		if(m_conversationDisplays[m_conversationDisplays.Count-1].transform.position.y<=-1*DISTANCE_BETWEEN_CONVERSATIONS)
		foreach (ConversationDisplayUnit displayUnit in m_conversationDisplays) {
			displayUnit.transform.position+=Vector3.up*DISTANCE_BETWEEN_CONVERSATIONS;
		}
	}

	public void ArrowDown(UIButton arrow){
		if(m_conversationDisplays[0].transform.position.y>=DISTANCE_BETWEEN_CONVERSATIONS)
		foreach (ConversationDisplayUnit displayUnit in m_conversationDisplays) {
			displayUnit.transform.position-=Vector3.up*DISTANCE_BETWEEN_CONVERSATIONS;
		}
	}

	public void MoveToMap(){
		m_TransferToMap = true;
	}

	//Called to add a brand new dialogue display
	private void AddConversationDisplay(string characterName, Color characterColor, DialogueLine dialogue){
		GameObject conversationObj = Instantiate (conversationDisplayPrefab.gameObject) as GameObject;

		if (conversationObj != null) {
			ConversationDisplayUnit displayUnit = conversationObj.GetComponent<ConversationDisplayUnit>();

			displayUnit.SetUpDialogue(characterName,dialogue.LineOfDialogue,characterColor,dialogue.TextColor);
			m_conversationDisplays.Add (displayUnit);

			if(m_conversationDisplays.Count>1){
				displayUnit.transform.position=m_conversationDisplays[m_conversationDisplays.Count-2].transform.position- Vector3.up*DISTANCE_BETWEEN_CONVERSATIONS;
			}

			displayUnit.SetReadInConversation(TransferToChat);

			displayUnit.transform.parent=transform;
		}
	}

	//Update dialogue display
	private void UpdateConversationDisplay(string characterName, Color characterColor, DialogueLine dialogue){
		for(int i = 0; i<m_conversationDisplays.Count; i++){
			if(m_conversationDisplays[i].IsSameConversation(characterName)){

				m_conversationDisplays[i].SetUpDialogue(characterName, dialogue.LineOfDialogue,characterColor,dialogue.TextColor);
			}
		}
	}

	public void TransferToChat(ConversationDisplayUnit displayUnit){
		m_TransferToChat = true;
		ChatHandler.Instance.SetCurrentConversation(displayUnit.characterName.text, displayUnit.characterName.color, m_conversationHistories[displayUnit.characterName.text]);		
	}
}
