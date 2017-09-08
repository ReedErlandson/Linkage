using UnityEngine;
using System.Collections;

public class CursorScript : MonoBehaviour {

	//editor changes
	Transform mainCam;

	public LineRenderer laser;
	public Tile focusedTile;
	public GameObject focusedGM;

    public GameObject pointerEnd;

	void Start () {
		mainCam = GameObject.Find ("Main Camera").GetComponent<Transform> ();


		Vector3[] initLaserPositions = new Vector3[ 2 ] { Vector3.zero, Vector3.zero };
		laser.SetPositions( initLaserPositions );
		laser.SetWidth( 0.05f, 0.05f );
	}

	void Update () {
		//platformSpec
		#if UNITY_ANDROID
		Quaternion ori = GvrController.Orientation;
		Vector3 v = GvrController.Orientation * Vector3.forward;
		#else
		Quaternion ori = mainCam.rotation;
		Vector3 v = ori * Vector3.forward;
		#endif

		gameObject.transform.rotation = ori;
		ShootLaserFromTargetPosition( transform.position, v, 200f );
		laser.enabled = true;
	}

	public void ShootLaserFromTargetPosition( Vector3 targetPosition, Vector3 direction, float length )
	{
		Ray ray = new Ray( targetPosition, direction );
		RaycastHit raycastHit;

		if( Physics.Raycast( ray, out raycastHit, length ) ) {
			GameObject focusedObject = raycastHit.transform.gameObject;
			focusedTile = focusedObject.GetComponent<Tile> ();
			focusedGM = raycastHit.transform.gameObject;

            focusedGM.SendMessage("hovered",SendMessageOptions.DontRequireReceiver);
		}

		Vector3 endPosition = raycastHit.point;
        pointerEnd.transform.position = endPosition;
		laser.SetPosition( 0, targetPosition );
		laser.SetPosition( 1, endPosition );
	}
}
