using UnityEngine;
using System.Collections;

//Singleton LocationHandler for getting real location on device [Only works on device]
public class LocationHandler : MonoBehaviour {
	private Vector2 m_location; //Internal storage of location vector
	private Vector2 m_origLocation; //Internal storage of original location vector
	private Vector2 m_prevLocation; //Internal storage of previous location vector
	public Vector2 Location { get { return m_location; } } //read-only reference to location
	public static LocationHandler Instance;
	
	private const float EDITOR_SCALE = 0.001f;

	private bool origSet=false;

	public void Init(){
		StartCoroutine_Auto (GetLocation ());

		if (Instance == null) {
			Instance = this;		
		}

		m_prevLocation = new Vector2 ();

	}


	IEnumerator GetLocation() {
		if (!Input.location.isEnabledByUser)
			yield return new WaitForSeconds(1);


		Input.location.Start();

		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1) {
			print("Timed out");
			yield return new WaitForSeconds(1);
		}
		if (Input.location.status == LocationServiceStatus.Failed) {
			print("Unable to determine device location");
			yield return new WaitForSeconds(1);
		} else{
			m_location = new Vector2(Input.location.lastData.latitude,Input.location.lastData.longitude);
			if(!origSet && (m_location.x!=0 || m_location.y!=0)){
				m_origLocation = m_location;
				m_prevLocation =m_location;
				origSet=true;


			}
		}
		Input.location.Stop();

		yield return true;
	}
	
	void Update(){
		//For testing in editor
		#if UNITY_EDITOR
		if (Input.GetKey (KeyCode.LeftArrow)) {
			m_location+=Vector2.right*EDITOR_SCALE*-1;		
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			m_location+=Vector2.right*EDITOR_SCALE;		
		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			m_location+=Vector2.up*EDITOR_SCALE;		
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			m_location+=Vector2.up*EDITOR_SCALE*-1f;
		}
		#else
		StartCoroutine (GetLocation ());
		#endif



		if (m_prevLocation != m_location) {
			if((m_location-m_prevLocation).magnitude>0.004f){
				m_prevLocation += (m_location-m_prevLocation).normalized*0.004f;
			}
			else{
				m_prevLocation = m_location;
			}

			MapHandler.Instance.UpdateMap(GetPrisonPlayerLocation());

		}
	}

	public Vector2 GetPrisonPlayerLocation(){
		return (m_prevLocation-m_origLocation);
	}


}
