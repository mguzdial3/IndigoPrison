using UnityEngine;
using System.Collections;

public class MainMenuHandler : DisplayHandler {
	//Check for returning to conversation
	private bool m_TransferToConversation;

	public override string UpdateDisplay (){

		if (m_TransferToConversation) {
			m_TransferToConversation=false;
			return ConversationHandler.Instance.DisplayName;		
		}

		return base.UpdateDisplay ();
	}

	public void ButtonPress(UIButton button){
		m_TransferToConversation = true;
	}
}
