using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Using UnityEditor stuff for ease of setting up demo
public class DramaManager {
	private CharacterManager m_CharacterManager;

	public DramaManager(){
		m_CharacterManager = new CharacterManager ();

		//Add Characters to MapHandler
		foreach (Character character in m_CharacterManager.Characters) {
			int iconToUse = character.CharacterRole==CharacterRole.PRISONER ? MapHandler.PRISONER_ICON : MapHandler.GUARD_ICON;
			MapHandler.Instance.AddIndicator(character.CharacterName,iconToUse,character.Location);
		}

		//Add the things characters need for their goals TODO; WILL WANT TO HAVE THIS BE BASED ON EACH CHARACTER LATER
	}

	//TODO; REMOVE. FOR DEMO
	public Character GetPrisoner(){
		return m_CharacterManager.prisoner;
	}

	public Character GetGuard(){
		return m_CharacterManager.guard;
	}
}

