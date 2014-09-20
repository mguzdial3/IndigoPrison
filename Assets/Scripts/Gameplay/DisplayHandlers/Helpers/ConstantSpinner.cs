using UnityEngine;
using System.Collections;

public class ConstantSpinner : MonoBehaviour {
	public Vector3 rotateAngle = Vector3.up;
	public float rotateSpeed=5;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(rotateAngle*rotateSpeed*Time.deltaTime);
	}
}
