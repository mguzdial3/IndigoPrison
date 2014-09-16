using UnityEngine;
using System.Collections;

public class ConversationDisplayUnit : MonoBehaviour {
	public GUITexture background;
	public GUIText characterName;
	public GUIText dialogueLine;

	public delegate void ReadInConversationDisplay(ConversationDisplayUnit unit);
	private ReadInConversationDisplay m_readConversationDisplay; 

	//Max number of characters to show (otherwise truncate to ...)
	private const int MAX_CHARACTERS = 25;
	private const string TRUNCATED_END="...";

	public void SetUpDialogue(string character, string dialogue, Color colorOfCharacter, Color colorOfDialogue){
		characterName.text = character;
		characterName.color = colorOfCharacter;

		if (!string.IsNullOrEmpty (dialogue) && dialogue.Length > MAX_CHARACTERS) {
			dialogue = dialogue.Substring(0,MAX_CHARACTERS);
			dialogueLine.text = dialogue+TRUNCATED_END;
		}
		else{
			dialogueLine.text = dialogue;
		}

		dialogueLine.color = colorOfDialogue;
	}

	public bool IsSameConversation(string character){
		return !string.IsNullOrEmpty (characterName.text) && !string.IsNullOrEmpty (character) && character.Equals (characterName.text);
	}

	public void SetReadInConversation(ReadInConversationDisplay readConversation){
		m_readConversationDisplay = readConversation;
	}

	public void ButtonClick(UIButton buttonClicked){
		m_readConversationDisplay (this);
	}




}
