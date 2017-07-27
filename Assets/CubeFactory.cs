using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeFactory : MonoBehaviour {
	//linkage
	GameManager managerCall;


	//definitions
	public float spacerSlug;
	public float maxSize;
	public static float scaleNum = 0f;
	public int[] faceOrder = { 1, 6, 2, 4, 3, 5 };
	public GameObject tilePrefab;
	public GameObject linkIconPrefab;
	public GameObject guiNumPrefab;
	Transform mainCam;

	// Use this for initialization
	void Start () {
		managerCall = GetComponent<GameManager> ();
		mainCam = GameObject.Find ("Main Camera").GetComponent<Transform> ();
	}

	// Update is called once per frame
	void Update () {

	}

	// Draws a cube
	public void drawCube(CubeMap fedMap, bool isMenu) {

		for (int l = 1; l <= 6; l++) {
			drawFace (fedMap.dimension, spacerSlug, l, fedMap.fMapList, isMenu);
		}
	}

	// Draws a single cube face
	public void drawFace(int size, float spacer, int faceNum, List<FaceMap> faceList, bool isMenu) {
		scaleNum = (maxSize - ((size - 1)*spacerSlug)) / size;

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {

				GameObject newTile = Instantiate(tilePrefab, new Vector3 ((x * (spacer + scaleNum))+scaleNum*.5f, (y * (spacer + scaleNum))+scaleNum*.5f, 0f), Quaternion.identity) as GameObject;
				newTile.transform.SetParent(GameObject.Find("CubeFace"+faceNum.ToString()).transform, false);
				newTile.GetComponent<Tile>().tileType = (int)char.GetNumericValue(faceList[faceNum - 1].gridArray[y,x]);
				managerCall.tileObjArray.Add (newTile);
				managerCall.tileArray.Add(newTile.GetComponent<Tile>());
				newTile.GetComponent<Tile>().xPos = x;
				newTile.GetComponent<Tile>().yPos = y;
				newTile.GetComponent<Tile>().fNo = faceNum;
				newTile.GetComponent<Tile>().updateFlag = true; //update instantiated tile
				newTile.GetComponent<Tile>().gridDim = size;

				if (isMenu && faceNum==1) {
					newTile.GetComponent<Tile>().index = (size*size-size+1)-y*4+x;
					GameObject newGuiNum = Instantiate (guiNumPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
					newGuiNum.transform.localPosition = new Vector3 (0, 0, -0.01f);
					newGuiNum.transform.SetParent (newTile.transform, false);
					newGuiNum.GetComponent<TextMesh> ().text = newTile.GetComponent<Tile> ().index.ToString();
				}

				//gate array append


				if (newTile.GetComponent<Tile> ().tileType < 10 && newTile.GetComponent<Tile> ().tileType > 1) {
					GameObject newLinkIcon = Instantiate(linkIconPrefab, new Vector3 (0,0,0), Quaternion.identity) as GameObject;
					linkNodeScript newLinkScript = newLinkIcon.GetComponent<linkNodeScript> ();
					newLinkScript.tilePos = newTile.GetComponent<Transform>().position;
					newLinkIcon.transform.SetParent (mainCam, false);

					managerCall.tileIconArray.Add (newLinkIcon);
					bool passNewTile = true;
					foreach (Tile GaT in managerCall.gateArray) {
						if (GaT.tileType == newTile.GetComponent<Tile> ().tileType) {
							passNewTile = false;
						}
					}
					if (passNewTile) {
						managerCall.gateArray.Add (newTile.GetComponent<Tile> ());
						managerCall.levelColorCount += 1;
					}
				}
			}
		}
	}

}