using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Using a singleton structure for these too so that interactions with other displays can affect this one
public class MapHandler : DisplayHandler {
	public static MapHandler Instance;

	//Components of map
	public GUITexture map;
	public GUITexture shadow;
	public GUITexture playerIndicator;

	//Prefabs
	public GameObject prisonerIcon, guardIcon, itemIcon;
	//Prefab stuff
	public static int PRISONER_ICON = 0;
	public static int GUARD_ICON = 1;
	public static int ITEM_ICON = 2;

	//Dictionary of name to a GUITexture representative of other indicators
	private Dictionary<string, GUITexture> m_otherIndicators;
	//"Fog Of War" Image	
	private Texture2D m_fogOfWar;
	private Color m_fogOfWarColor = Color.black;

	private const float MAP_SCALE = 8000f; //Scale for shifting changes in longitude/latitude to map points
	private readonly Vector2 PLAYER_SIZE = new Vector2 (30, 30);
	private readonly Vector2 EXTRA_BUFFER = new Vector2(30,30);

	//Check for returning to conversation
	private bool m_TransferToConversation;

	//Player Pos
	public Vector2 PlayerPos { get; private set;}



	public override void Init (){
		base.Init ();

		if (Instance == null) {
			Instance = this;		
		}

		m_otherIndicators = new Dictionary<string,GUITexture > ();
		m_fogOfWar = new Texture2D (Screen.width, Screen.height);

		for (int i = 0; i<Screen.width; i++) {
			for (int j = 0; j<Screen.height; j++) {
				m_fogOfWar.SetPixel(i,j,m_fogOfWarColor);
			}
		}

		Vector2 playerPos = new Vector2 (playerIndicator.transform.position.x*Screen.width, playerIndicator.transform.position.y*Screen.height);
		RemoveFog (playerPos);

		m_fogOfWar.Apply (false);

		shadow.texture = (Texture)m_fogOfWar;
	}

	public override string UpdateDisplay (){
		if (m_TransferToConversation) {
			m_TransferToConversation=false;
			return ConversationHandler.Instance.DisplayName;		
		}

		return base.UpdateDisplay ();
	}

	///////////
	/// Indicator Stuff
	/// 
	/// 

	public bool HasIndicator(string nameOfIndicator){
		return m_otherIndicators.ContainsKey (nameOfIndicator);
	}

	//Called to add an indicator to the map from an int constance, and a vector in prison space
	public void AddIndicator(string nameOfIndicator, int indicator, Vector2 locationVector){
		GameObject toUse = null;

		if(indicator ==PRISONER_ICON){
			toUse=prisonerIcon;
		}
		else if(indicator ==GUARD_ICON){
			toUse = guardIcon;
		}
		else if(indicator ==ITEM_ICON){
			toUse = itemIcon;
		}

		if (toUse != null) {
			GameObject go = Instantiate(toUse) as GameObject;		
			GUITexture guiTexture = go.GetComponent<GUITexture>();

			if(guiTexture!=null){
				Vector3 newPos = GetPositionGivenMapAndTexture(locationVector,guiTexture);

				newPos.x/=Screen.width;
				newPos.y/=Screen.height;
				guiTexture.transform.position = newPos;

				guiTexture.transform.parent = transform;
				m_otherIndicators.Add (nameOfIndicator,guiTexture);
			}
			else{
				Debug.LogError ("[MapHandler] GameObject "+toUse.name+" did not have a GUITexture attached");
			}
		}
		else{
			Debug.LogError("[MapHandler] Attempted to Add Non-Constant Indicator");
		}
	}

	//Pass in name of character/item and its position to move it
	public void MoveIndicator(string indicatorName, Vector2 prisonPosition){
		Vector2 pixelPos = TransferPrisonVectorToMapVector (prisonPosition);

		if (m_otherIndicators.ContainsKey (indicatorName)) {
			GUITexture guiTex = m_otherIndicators[indicatorName];

			guiTex.transform.position = GetPositionGivenMapAndTexture(pixelPos,guiTex);
		}
		else{
			Debug.LogError("[MapHandler] Attempted to Move Indicator that did not exist");
		}
	}

	//Pass in name of character/item to destroy it
	public void DestroyIndicator(string indicatorName){

		if (m_otherIndicators.ContainsKey (indicatorName)) {
			Destroy(m_otherIndicators[indicatorName].gameObject);
			m_otherIndicators.Remove(indicatorName);
		}
		else{
			Debug.LogError("[MapHandler] Attempted to Destroy Indicator that did not exist");
		}
	}

	//Called when the player's position changes from wherever
	public void UpdateMap(Vector2 playerPos){
		Vector2 pixelPos = TransferPrisonVectorToMapVector (playerPos);

		Vector3 newPos = GetPositionGivenMapAndTexture (pixelPos, playerIndicator);

		pixelPos.x *= Screen.width;
		pixelPos.y *= Screen.height;
		PlayerPos = pixelPos;

		RemoveFog(pixelPos);

		m_fogOfWar.Apply (false);

		playerIndicator.transform.position = newPos;

	}

	private void RemoveFog(Vector2 pixelPos){
		Vector2 centerPlayer = new Vector2(pixelPos .x + PLAYER_SIZE.x / 2, pixelPos.y+PLAYER_SIZE.y/2);
		
		for(int i = (int)(pixelPos.x-EXTRA_BUFFER.x); i<pixelPos.x+PLAYER_SIZE.x+EXTRA_BUFFER.x; i++){
			for(int j = (int)(pixelPos.y-EXTRA_BUFFER.y); j<pixelPos.y+PLAYER_SIZE.y+EXTRA_BUFFER.y; j++){
				
				float dist = (new Vector2(i,j)-centerPlayer).magnitude;
				if(i>=0 && j>=0 && i<m_fogOfWar.width && j<m_fogOfWar.height){
					if(dist<PLAYER_SIZE.x/2.0f){
						m_fogOfWar.SetPixel(i,j,new Color(0,0,0,0));
					}
					else{
						Color c = m_fogOfWar.GetPixel(i,j);
						float alphaVal = PLAYER_SIZE.x/dist;
						alphaVal = 1.0f-alphaVal;
						//alphaVal*=2; Add this to get rid of the pixel-fog of war look
						if(c.a>alphaVal){
							c.a=alphaVal;
							m_fogOfWar.SetPixel(i,j,c);
						}
					}
				}
			}
			
		}
	}

	//Does what it says
	private Vector2 TransferPrisonVectorToMapVector(Vector2 locationVector){
		locationVector*= MAP_SCALE;
		Vector2 pixelPos = locationVector + new Vector2 (Screen.width / 2, Screen.height / 2);
		pixelPos.x /= Screen.width;
		pixelPos.y /= Screen.height;

		return pixelPos;
	}

	private Vector3 GetPositionGivenMapAndTexture(Vector2 locationVector, GUITexture guiTex){
		return new Vector3 (locationVector.x, locationVector.y, guiTex.transform.position.z);
	}

	public void MoveToConversation(UIButton button){
		m_TransferToConversation = true;
	}

}
