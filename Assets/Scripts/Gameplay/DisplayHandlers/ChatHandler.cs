using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Using a singleton structure for these too so that interactions with other displays can affect this one
public class ChatHandler : DisplayHandler {
	public static ChatHandler Instance; //Singleton reference
	private string m_keyboardInput=""; //Text for player input
	private TouchScreenKeyboard m_keyboard; //Mobile Keyboard reference

	//Current character and dialogue lines for display purposes
	private string m_currentCharacter;

	//Check for returning to conversation
	private bool m_TransferToConversation;

	//Current character public readonly reference
	public string CurrentCharacter{ get { return m_currentCharacter; } }

	public GUIText characterName;
	public GUIText otherText;
	public GUIText playerText;

	private const int MAX_LINE_LENGTH = 40;

	//For Editor Text Input
	private bool m_textInput;

	public override void Init (){
		base.Init ();
		
		if (Instance == null) {
			Instance = this;		
		}
	}

	public override void SwitchToDisplay (){
		base.SwitchToDisplay ();
		if(m_keyboard!=null){
			m_keyboard.active = false;
		}
	}

	public override void SwitchFromDisplay (){
		base.SwitchFromDisplay ();
		if(m_keyboard!=null){
			m_keyboard.active = false;
		}

		m_currentCharacter = null;
	}

	//Only for testing text stuff in editor
	#if UNITY_EDITOR
	void OnGUI(){
		if (m_textInput) {
			float height= Screen.height;
			m_keyboardInput = GUI.TextField(new Rect(0, height*0.9f, Screen.width,height*0.1f),m_keyboardInput);		
		}
	}
	#endif

	public override string UpdateDisplay (){
		if (m_TransferToConversation) {
			m_TransferToConversation=false;
			return ConversationHandler.Instance.DisplayName;		
		}

		#if UNITY_EDITOR
		if(m_textInput){
			if(Input.GetKeyUp(KeyCode.S) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Tab) || Input.GetKey(KeyCode.RightArrow)){
				m_textInput = false;
				ConversationHandler.Instance.AddLine(characterName.text,characterName.color,new DialogueLine(m_keyboardInput));
				m_keyboardInput="";
			}
		}
		#else
		if (m_keyboard != null && TouchScreenKeyboard.visible) {
			m_keyboardInput = m_keyboard.text;
			
			otherText.transform.position = Vector3.up*(TouchScreenKeyboard.area.height/Screen.height);
			playerText.transform.position = Vector3.right+Vector3.up*(TouchScreenKeyboard.area.height/Screen.height);
			
		}
		if(!TouchScreenKeyboard.visible && !string.IsNullOrEmpty(m_keyboardInput)){
			ConversationHandler.Instance.AddLine(characterName.text,characterName.color,new DialogueLine(m_keyboardInput));
			m_keyboardInput="";
		}

		#endif

		return base.UpdateDisplay ();
	}





	//Setter for current character and dialogue lines
	public void SetCurrentConversation(string currentCharacter, Color characterColor, List<DialogueLine> listOfLines){
		m_currentCharacter = currentCharacter;

		//Updat Display
		characterName.text = currentCharacter;
		characterName.color = characterColor;
		otherText.color = characterColor;

		otherText.text = "";
		playerText.text = "";


		foreach (DialogueLine line in listOfLines) {
			UpdateList(line);
		}


	}

	//Adds the new line, then needs to update the display
	public void UpdateList(DialogueLine line){
		string toAdd="";

		int linesBetween =1;
		
		if(line.LineOfDialogue.Length<MAX_LINE_LENGTH){
			toAdd+=line.LineOfDialogue;

		}
		else{
			int charsOnThisLine = 0;
			for(int i = 0; i<line.LineOfDialogue.Length; i++){
				if(line.LineOfDialogue[i]==' '){
					int charsExtra = 0; 
					
					while(charsExtra+i<line.LineOfDialogue.Length && line.LineOfDialogue[charsExtra+i]!=' '){
						charsExtra++;
					}
					
					if(charsExtra+i>line.LineOfDialogue.Length){
						//Do nothing, we'll just get to the end of the line
						toAdd+=line.LineOfDialogue[i];
						charsOnThisLine++;
					}
					else{
						if(charsExtra+charsOnThisLine>MAX_LINE_LENGTH){
							toAdd+=line.LineOfDialogue[i]+"\n";
							charsOnThisLine=0;
							linesBetween++;

						}
						else{
							toAdd+=line.LineOfDialogue[i];
							charsOnThisLine++;
						}
					}
					
				}
				else{
					toAdd+=line.LineOfDialogue[i];
					charsOnThisLine++;
				}
				
				
			}
		}
	
		if(line.IsPlayerLine()){//The Player Said This
			playerText.text+=toAdd+"\n";

			//Add spaces where appropriate
			for(int i = 0; i<linesBetween; i++){
				otherText.text+="\n";
			}
		}
		else{//The other character said this
			otherText.text+=toAdd+"\n";

			//Add spaces where appropriate
			for(int i = 0; i<linesBetween; i++){
				playerText.text+="\n";
			}
		}
		
	}
	
	public bool IsCurrentCharacter(string characterName){
		return !string.IsNullOrEmpty (characterName) && !string.IsNullOrEmpty (m_currentCharacter) && characterName.Equals (m_currentCharacter);
	}

	public void MoveToConversation(UIButton button){
		m_TransferToConversation = true;
	}

	public void OpenKeyboard(UIButton button){
		m_keyboard = TouchScreenKeyboard.Open(m_keyboardInput, TouchScreenKeyboardType.ASCIICapable);

		#if UNITY_EDITOR
		m_textInput=true;
		#endif
	}


}
