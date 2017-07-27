using UnityEngine;
using System.Collections;

public class linkNodeScript : MonoBehaviour {

	public Vector3 tilePos;
	Vector3 objVector;
	Vector3 portVector;


	Transform mainCam;

	// Use this for initialization
	void Start () {
		mainCam = GameObject.Find("Main Camera").GetComponent<Transform>();
	}

	// Update is called once per frame
	void Update () {
		portVector = (Vector3.Lerp(tilePos,mainCam.position,0.001f));
		transform.position = portVector;
		transform.LookAt(mainCam);
	}
}
