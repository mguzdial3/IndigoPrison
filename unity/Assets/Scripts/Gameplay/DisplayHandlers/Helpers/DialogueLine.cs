using UnityEngine;
using System.Collections;

//Info Holder for line of dialogue
public class DialogueLine {
	private string m_nameOfSpeaker; //Those who spoke the line
	private string m_lineOfDialogue; //Actual line of dialogue
	private Color m_textColor; //Color of text (used to differentiate characters easily)

	//Public references
	public string NameOfSpeaker {get {return m_nameOfSpeaker;}} 
	public string LineOfDialogue {get {return m_lineOfDialogue;}}
	public Color TextColor {get {return m_textColor;}}


	public DialogueLine(string nameOfSpeaker, Color textColor, string lineOfDialogue){
		m_nameOfSpeaker = nameOfSpeaker;
		m_lineOfDialogue = lineOfDialogue;
		m_textColor = textColor;
	}

	public DialogueLine(string nameOfSpeaker, string lineOfDialogue): this(nameOfSpeaker,Color.white,lineOfDialogue){}//Default color to white

	public DialogueLine(string lineOfDialogue): this(ConversationHandler.PLAYER_NAME,lineOfDialogue){} //Can just use this one for when the player speaks

	public bool IsPlayerLine(){
		return m_nameOfSpeaker.Equals (ConversationHandler.PLAYER_NAME);
	}

}
