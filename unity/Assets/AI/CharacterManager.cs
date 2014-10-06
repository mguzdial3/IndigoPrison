using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Using UnityEngine for ease of setting up demo, can change
//Manages the characters, seperating it from DramaManager at this stage so they can be worked on individually
public class CharacterManager {
	private List<Character> m_characters;
	public List<Character> Characters {get{return m_characters;}}

	//FOR DEMO
	public Character prisoner, guard;

	private string[] m_characterNameSegments = new string[] {
				"qa",
				"wer",
				"was",
				"ref",
				"dre",
				"tre",
				"ghu",
				"tyu",
				"huy",
				"jui",
				"iok",
				"lok",
				"pol",
				"kio",
				"zas",
				"xas",
				"mon",
				"bru",
				"vuh",
				"cas"
		};

	private const string PRISONER_NAME = "Prisoner ";
	private const string GUARD_NAME = "Guard ";

	//Create the characters for this one
	public CharacterManager(){
		Vector2 playerPos = new Vector2 (Screen.width / 2f, Screen.height / 2f);
		//Hardcode generation at the moment
		m_characters = new List<Character> ();

		string prisonerName = PRISONER_NAME + GetRandomName ();
		prisoner = new Character (prisonerName, Color.cyan, CharacterRole.PRISONER, playerPos + Vector2.right * 30);
		m_characters.Add (prisoner);

		string guardName = GUARD_NAME + GetRandomName ();
		guard = new Character (guardName, Color.red, CharacterRole.GUARD, playerPos - Vector2.right * Screen.width / 3);
		m_characters.Add (guard);
	}

	private string GetRandomNameSegment(){
		return m_characterNameSegments[Random.Range(0,m_characterNameSegments.Length)];
	}

	private string GetRandomName(){
		string toReturn = GetRandomNameSegment () + GetRandomNameSegment ();
		toReturn = char.ToUpper (toReturn [0]) + toReturn.Substring (1);
		return toReturn;
	}



}
