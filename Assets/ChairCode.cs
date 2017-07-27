using UnityEngine;
using System.Collections;

public class ChairCode : MonoBehaviour {

	GameObject mainCamera;
	Transform chairPivot;

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find("Main Camera");
		chairPivot = GameObject.Find ("ChairPivot").GetComponent<Transform> ();
	}

	// Update is called once per frame
	void Update () {
		Vector3 newTest = mainCamera.transform.rotation.eulerAngles;
		chairPivot.rotation = Quaternion.Euler(new Vector3 (0, newTest.y, 0));
	}
}