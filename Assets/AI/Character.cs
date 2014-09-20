using UnityEngine;

//Info Holder for characters 
//for now using UnityEngine for ease of setting up demo, though that can change if desired
public class Character {
	//Private attributes
	private string m_characterName;
	private Color32 m_characterColor;
	private CharacterRole m_characterRole;
	private Vector2 m_Location;

	//Public access
	public string CharacterName{get{return m_characterName;}}
	public Color CharacterColor{get{return m_characterColor;}}
	public CharacterRole CharacterRole {get{return m_characterRole;}}
	public Vector2 Location{ get { return m_Location; } }
	

	//Constructor to set up the characters initial values
	public Character(string characterName, Color characterColor, CharacterRole characterRole){
		m_characterName = characterName;
		m_characterColor = characterColor;
		m_characterRole = characterRole;
	}

	//For instantiating with location
	public Character(string characterName, Color characterColor, CharacterRole characterRole, Vector2 location):this(characterName,characterColor,characterRole){
		SetLocation (location);
	}

	//Set the location of this character
	public void SetLocation(Vector2 loc){
		if (loc != default(Vector2)) {
			m_Location = loc;		
		}
	}
}

//We'll probably want more than this later
public enum CharacterRole{PRISONER, GUARD};
