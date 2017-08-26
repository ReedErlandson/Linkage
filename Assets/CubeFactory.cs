using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Cube factory creates the cube: handling the instantiation and properties of tiles
public class CubeFactory : MonoBehaviour {
	//linkage
	GameManager managerCall;


	//definitions
	public float spacerSlug; // padding between cubes
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

	// Draws a cube
	public void drawCube(CubeMap fedMap, bool isMenu) {
		for (int l = 1; l <= 6; l++) {
			drawFace (fedMap.dimension, spacerSlug, l, fedMap.fMapList, isMenu);
		}
	}

	// Draws a single cube face
	public void drawFace(int size, float spacer, int faceNum, List<FaceMap> faceList, bool isMenu) {
		scaleNum = (maxSize - ((size - 1)*spacerSlug)) / size;

        int currentLevelIndex = 1; //JOSE: possible alt setup

		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {

				GameObject newTile = Instantiate(tilePrefab, new Vector3 ((x * (spacer + scaleNum))+scaleNum*.5f, (y * (spacer + scaleNum))+scaleNum*.5f, 0f), Quaternion.identity) as GameObject;
				newTile.transform.SetParent(GameObject.Find("CubeFace"+faceNum.ToString()).transform, false);
				newTile.GetComponent<Tile>().tileType = faceList[faceNum - 1].gridArray[y,x];

				managerCall.tileObjArray.Add (newTile);
				managerCall.tileArray.Add(newTile.GetComponent<Tile>());

				newTile.GetComponent<Tile>().xPos = x;
				newTile.GetComponent<Tile>().yPos = y;
				newTile.GetComponent<Tile>().fNo = faceNum;
				newTile.GetComponent<Tile>().updateFlag = true; //update instantiated tile
				newTile.GetComponent<Tile>().gridDim = size;

                //spawns GUi numbers for tiles

                /*

                //Reed's original level select setup allows for 16 levels attached to one face
				if (isMenu && faceNum==1) {
					newTile.GetComponent<Tile>().index = (size*size-size+1)-y*4+x;
					GameObject newGuiNum = Instantiate (guiNumPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
					newGuiNum.transform.localPosition = new Vector3 (0, 0, -0.01f);
					newGuiNum.transform.SetParent (newTile.transform, false);
					newGuiNum.GetComponent<TextMesh> ().text = newTile.GetComponent<Tile> ().index.ToString();
				}

                /*/
                // Jose's setup. allows for up to 32 levels. Can make more flexible in the future
                if (isMenu) {                    
                    int index = (size * size - size + 1) - y * 4 + x - 1;
                    if (faceNum == 4) {
                        index += 16;
                    }
                    else if (faceNum == 5) {
                        index += 32;
                    }
                    if (index == 0 && faceNum == 1) {
                        newTile.GetComponent<Tile>().index = index;
                        GameObject newGuiNum = Instantiate(guiNumPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        newGuiNum.transform.localPosition = new Vector3(0, 0, -0.01f);
                        newGuiNum.transform.SetParent(newTile.transform, false);
                        newGuiNum.GetComponent<TextMesh>().text = "<--";
                    } else  if ((faceNum == 1 || faceNum == 4 || faceNum == 5) && managerCall.validLevel(index)) {
                        newTile.GetComponent<Tile>().index = index;
                        GameObject newGuiNum = Instantiate(guiNumPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        newGuiNum.transform.localPosition = new Vector3(0, 0, -0.01f);
                        newGuiNum.transform.SetParent(newTile.transform, false);
                        newGuiNum.GetComponent<TextMesh>().text = newTile.GetComponent<Tile>().index.ToString();

                        if (managerCall.checkLevelStatus(index)) {
                            newTile.GetComponent<Tile>().tileType = 2;
                            newTile.GetComponent<Tile>().updateFlag = true;
                            //Destroy(newTile.transform.GetChild(0).gameObject);
                        }

                    } else {
                        newTile.GetComponent<Tile>().tileType = 0;
                        newTile.GetComponent<Tile>().updateFlag = true;
                    }
                }
                //*/

                //gate array append
                if (newTile.GetComponent<Tile> ().tileType < 13 && newTile.GetComponent<Tile> ().tileType > 1) { //place gate Icon
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