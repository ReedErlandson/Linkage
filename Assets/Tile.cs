using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour, IComparable<Tile>
	{
	//linkage
	GameManager managerCall;
	CubeFactory factoryCall;

	//assign X Y value to tile

	//vars
	public bool isActive;
	public bool isGate = false;
	public bool isLinked = false;
	public bool updateFlag;
	public int tileType;
	public static float zScale = 0.2f;
	public List<Tile> activeNeighbors;
	public int hardColor = 0;
	public int xPos;
	public int yPos;
	public int fNo;
	public int gridDim;
	string pingString;
	public bool contiguousLink = false;
	public int index = 99;
	public bool deadFace = false;

	void Start () {
		managerCall = GameObject.Find("Code").GetComponent<GameManager>();
		factoryCall = GameObject.Find ("Code").GetComponent<CubeFactory>();
		StartCoroutine ("wakeAnim");
	}

	void Update() {
		updateTile ();
	}

	public void updateTile() {
		if (this.updateFlag == true) {
			this.isActive = (this.tileType != 0);
			GetComponent<Renderer>().material.SetColor("_EmissionColor", managerCall.tileColorArray[this.tileType]);

			if (this.tileType > 1 && this.tileType < 26) {
				isGate = true;
				hardColor = this.tileType;
			} else if (this.tileType > 25) {
				hardColor = this.tileType - 24;
			} else {
				hardColor = 0;
			}

			if (managerCall.linkStateArray [hardColor] == 1 && contiguousLink) {
				GetComponent<Renderer> ().material.SetColor ("_EmissionColor", managerCall.tileColorArray [this.hardColor]);
			}

			this.updateFlag = false;
		}
	}

	public void pingNeighbors() {
		managerCall.pingedTiles.Add (this);
		foreach (Tile anT in activeNeighbors) {
			if (!managerCall.pingedTiles.Contains (anT)) {
				if (anT.isGate) {
					managerCall.linkStateArray [tileType - 24] = 1;
					foreach (Tile alT in managerCall.pingedTiles) {
						alT.contiguousLink = true;
					}
					return;
				}
				anT.pingNeighbors ();
			}
		}
	}

	public void getNeighbors() {

		activeNeighbors.Clear ();

		//get left neighbor
		int targetX = xPos -1;
		int targetY = yPos;
		int targetF = fNo;
		if (targetX < 0) {
			if (fNo == 3 ) {
				targetY = 0;
				targetX = yPos;
			} else if (fNo == 6) {
				targetY = gridDim - 1;
				targetX = gridDim - 1 - yPos;
			} else {
				targetY = yPos;
				targetX = gridDim-1;
			}
			targetF = managerCall.wrapPointerArray [(fNo - 1) * 4 + 0];
		}
		foreach (Tile parsedTile in managerCall.tileArray) {
			if (parsedTile.xPos == targetX && parsedTile.yPos == targetY && parsedTile.fNo == targetF && parsedTile.tileType>1 && (parsedTile.tileType == tileType || parsedTile.tileType == tileType-24 || parsedTile.tileType == tileType+24)) {
				activeNeighbors.Add (parsedTile);
				//Debug.Log ("Left active neighbor");
			}
		}

		//get right neighbor
		targetX = xPos +1;
		targetY = yPos;
		targetF = fNo;
		if (targetX > gridDim-1) {
			if (fNo == 3) {
				targetY = 0;
				targetX = gridDim - yPos - 1;
			} else if (fNo == 6) {
				targetY = gridDim - 1;
				targetX = yPos;

			} else {
				targetY = yPos;
				targetX = 0;
			}
			targetF = managerCall.wrapPointerArray [(fNo - 1) * 4 + 2];
		}
		foreach (Tile parsedTile in managerCall.tileArray) {
			if (parsedTile.xPos == targetX && parsedTile.yPos == targetY && parsedTile.fNo == targetF && parsedTile.tileType>1 && (parsedTile.tileType == tileType || parsedTile.tileType == tileType-24 || parsedTile.tileType == tileType+24)) {
				activeNeighbors.Add (parsedTile);
				//Debug.Log("Right active neighbor");
			}
		}

		//get up neighbor
		targetX = xPos;
		targetY = yPos + 1;
		targetF = fNo;
		if (targetY > gridDim - 1) {
			if (fNo == 1 || fNo == 3) {
				targetX = xPos;
				targetY = 0;
			} else if (fNo == 5 || fNo == 6) {
				targetX = gridDim - 1 - xPos;
				targetY = gridDim - 1;
			} else if (fNo == 2) {
				targetX = 0;
				targetY = gridDim - 1 - xPos;
			} else {
				targetX = gridDim - 1;
				targetY = xPos;
			}
			targetF = managerCall.wrapPointerArray [(fNo - 1) * 4 + 1];
		}
		foreach (Tile parsedTile in managerCall.tileArray) {
			if (parsedTile.xPos == targetX && parsedTile.yPos == targetY && parsedTile.fNo == targetF && parsedTile.tileType>1 && (parsedTile.tileType == tileType || parsedTile.tileType == tileType-24 || parsedTile.tileType == tileType+24)) {
				activeNeighbors.Add (parsedTile);
				//Debug.Log("Top active neighbor");
			}
		}

		//get down neighbor
		targetX = xPos;
		targetY = yPos - 1;
		targetF = fNo;
		if (targetY < 0) {
			if (fNo == 1 || fNo == 6) {
				targetX = xPos;
				targetY = gridDim - 1;
			} else if (fNo == 2) {
				targetX = 0;
				targetY = xPos;
			} else if (fNo == 3) {
				targetX = gridDim - 1 - xPos;
				targetY = 0;
			} else if (fNo == 4) {
				targetX = gridDim - 1;
				targetY = gridDim - 1 - xPos;
			} else { //face 5
				targetX = gridDim - 1 - xPos;
				targetY = 0;
			}
			targetF = managerCall.wrapPointerArray [(fNo - 1) * 4 + 3];
		}
		foreach (Tile parsedTile in managerCall.tileArray) {
			if (parsedTile.xPos == targetX && parsedTile.yPos == targetY && parsedTile.fNo == targetF && parsedTile.tileType > 1 && (parsedTile.tileType == tileType || parsedTile.tileType == tileType - 24
                || parsedTile.tileType == tileType+24)) {
				activeNeighbors.Add (parsedTile);
				//Debug.Log ("Bottom active neighbor");
			}
		}
	}

	IEnumerator wakeAnim() {
		float progress = 0;
		float TimeScale = 2f;
		while (progress <= 1) {
			transform.localScale = Vector3.Lerp(new Vector3 (0,0,0), new Vector3 (CubeFactory.scaleNum,CubeFactory.scaleNum,zScale), progress);
			progress += Time.deltaTime * TimeScale;
			yield return null;
		}
		transform.localScale = new Vector3 (CubeFactory.scaleNum,CubeFactory.scaleNum,zScale);
	}

    public int CompareTo(Tile other) {
        return this.tileType - other.tileType;
    }
}