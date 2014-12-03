using UnityEngine;
using System.Collections;

//Singleton LocationHandler for getting real location on device [Only works on device]
public class LocationHandler : MonoBehaviour {
	private Vector2 m_location; //Internal storage of location vector
	private Vector2 m_lerpLocation; //Internal storage of lerp location vector
	private Vector2 m_origLocation; //Internal storage of original location vector
	private Vector2 m_prevLocation; //Internal storage of previous location vector
	public Vector2 Location { get { return m_location; } } //read-only reference to location
	public static LocationHandler Instance;

	private const float EDITOR_SCALE = 10f;

	private bool origSet=false;

	//For testing
	private string currPrint = "";

	public void Init(){
		//StartCoroutine_Auto (GetLocation ());

		if (Instance == null) {
			Instance = this;		
		}

		m_location = new Vector2 ();
		m_prevLocation = new Vector2 ();
		Input.gyro.enabled = true;
		//Input.gyro.updateInterval = 0.01F;
	}


	IEnumerator GetLocation() {
		if (!Input.location.isEnabledByUser)
			yield return new WaitForSeconds(1);


		Input.location.Start(5f,5f);

		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1) {
			yield return new WaitForSeconds(1);
		}
		if (Input.location.status == LocationServiceStatus.Failed) {
			yield return new WaitForSeconds(1);
		} else{
			m_location = new Vector2(Input.location.lastData.latitude,Input.location.lastData.longitude);
			if(!origSet && (m_location.x!=0 || m_location.y!=0)){
				m_origLocation = m_location;
				m_prevLocation =m_location;
				m_lerpLocation = m_location;
				origSet=true;


			}
		}
		Input.location.Stop();

		yield return true;
	}
	
	void Update(){
		//For testing in editor
		#if UNITY_EDITOR

		bool update = false;

		if (Input.GetKey (KeyCode.LeftArrow)) {
			m_location+=Vector2.right*EDITOR_SCALE*-1;	
			update = true;
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			m_location+=Vector2.right*EDITOR_SCALE;	
			update = true;
		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			m_location+=Vector2.up*EDITOR_SCALE;
			update = true;
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			m_location+=Vector2.up*EDITOR_SCALE*-1f;
			update = true;
		}

		#else
		//StartCoroutine (GetLocation ());
		/**
		int i = 0;
		while (i < Input.accelerationEventCount) {
			AccelerationEvent accEvent = Input.GetAccelerationEvent(i);
			Vector3 acc = accEvent.acceleration * accEvent.deltaTime;
			m_location.x+=acc.x/10f;
			m_location.y+=acc.z/10f;
			++i;


		}
		*/
		Vector3 gyro = Input.gyro.userAcceleration;
		if(gyro!=Vector3.zero){
			m_lerpLocation.x+=gyro.x*Time.deltaTime*100;
			m_lerpLocation.y+=gyro.z*Time.deltaTime*100;

				
		}

		#endif



		if (m_prevLocation != m_location) {
			/**
			if((m_location-m_prevLocation).magnitude>0.004f){
				m_prevLocation += (m_location-m_prevLocation).normalized*0.004f;
			}
			else{
			*/
				m_prevLocation = m_location;
			//}
			//SetPrint(""+GetPrisonPlayerLocation().x+","+GetPrisonPlayerLocation().y);


		}


		if (m_location != m_lerpLocation) {
			if((m_location-m_lerpLocation).magnitude<0.01f){
				m_lerpLocation = m_location;
			}
			else{
				m_lerpLocation = Vector2.Lerp(m_lerpLocation,m_location,Time.deltaTime*0.5f);
			}

			#if UNITY_EDITOR
			m_lerpLocation = m_location;
			#endif

			MapHandler.Instance.UpdateMap(GetPrisonPlayerLocation());		
		}

	}

	public Vector2 GetPrisonPlayerLocation(){
		return (m_lerpLocation);//m_lerpLocation-m_origLocation);*10;
	}

	public void SetPrint(string printVal){
		currPrint = printVal;
	}

	public void OnGUI(){
		GUI.Label (new Rect (Screen.width/2, 0, 100, 100), currPrint);
	}


}
