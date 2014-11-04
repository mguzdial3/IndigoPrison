using System.Collections;

namespace Indigo{
//Info Holder for line of dialogue
	public class DialogueLine {
		private string m_nameOfSpeaker; //Those who spoke the line
		private string m_lineOfDialogue; //Actual line of dialogue

		//Public references
		public string NameOfSpeaker {get {return m_nameOfSpeaker;}} 
		public string LineOfDialogue {get {return m_lineOfDialogue;}}


		public DialogueLine(string nameOfSpeaker, string lineOfDialogue){
			m_nameOfSpeaker = nameOfSpeaker;
			m_lineOfDialogue = lineOfDialogue;
		}

		public DialogueLine(string lineOfDialogue): this(DramaManager.PLAYER_NAME,lineOfDialogue){} //Can just use this one for when the player speaks

		public bool IsPlayerLine(){
			return m_nameOfSpeaker.Equals (DramaManager.PLAYER_NAME);
		}

		public DialogueLine Clone(){
			return new DialogueLine(m_nameOfSpeaker,m_lineOfDialogue);
		}

	}
}
