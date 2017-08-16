using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
	//linkage
	CubeFactory factoryCall;
	CsvReader csvrCall;
	TxtReader txtCall;
	CursorScript cursorCall;

	LineRenderer remoteLR;

	//tools
	bool paintDebug = false;

    //audio
    [Header("Audio")]
    public AudioClip tileClick;
    public AudioClip gateClick;
    AudioSource audioSrc;


    //definitions
    
    public List<CubeMap> levelArray;
	public List<List<CubeMap>> NodeArray;
	char[,] LFCharray;
	public FaceMap lockedFace;
	public FaceMap blankFace;
	public CubeMap uiCube;

    [Space()]
    public Color[] tileColorArray;

	//tiles
	Tile clickTarget;
	int paintSlug = 1;

    //ui
    [Header("UI")]
    public GameObject uiBoard;
	public GameObject uiHeader;
	public Transform uiPointer;
	public List<GameObject> uiObjects;
	GameObject computron;
	public List<GameObject> nodeKits;
	public List<GameObject> activeKits;

    //logic
    [Header("Tile Type Mangers")]
    public List<Tile> tileArray;
	public List<GameObject> tileObjArray;
	public List<Tile> gateArray;
	public int[] wrapPointerArray; //left, up, right, down
	public int[] linkStateArray;
	public List<Tile> pingedTiles;
	public List<GameObject> tileIconArray;

    //hints
    [Header("Hint Manager")]
	public float hintTimer = 1.5f;
	public float downTime, upTime, pressTime = 0;
	public bool hintReady = false;
    public List<SolutionMap> solutionArray;

    //levels
    [Header("Level Manager")]
    public int nodeCount = 2;
	public int currentLevel = 0;
	public int currentNode = 0;
	public int levelColorCount = 0;
	bool levelCompleted = false;
	bool nodeSelect = false;
	bool swiping = false;

	//loading progress
	public List<bool[]> levelTracker;

    //file path stuff
    [Header("File pathing")]
    public string levelPath;
	public string result = "";
	public string solutionPath;
	public string solutionResult = "";

    [Header("Tile Editor")]
    public bool editMode;

	//light
    [Space()]
	public Light ceilingLight;

	//Debug vars
	bool activeCubeFlag = false;

	// Use this for initialization
	void Start()
	{
		factoryCall = GetComponent<CubeFactory>();
		csvrCall = GetComponent<CsvReader>();
		txtCall = GetComponent<TxtReader>();
		cursorCall = GameObject.Find("CursorCube").GetComponent<CursorScript>();

		//ui
		uiPointer = GameObject.Find("CubeUI").GetComponent<Transform>();
		computron = GameObject.Find("Computer_Table_");
		List<GameObject> activeKits = new List<GameObject>();

		//editor
		Screen.lockCursor = true;

		remoteLR = GameObject.Find("CursorCube").GetComponent<LineRenderer>();

		audioSrc = GetComponent<AudioSource>();

        //solutions
        solutionArray = new List<SolutionMap>();

		//light
		ceilingLight = GameObject.Find("CeilingLight").GetComponent<Light>();

		//path biz
		levelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "csv1.csv");
		solutionPath = System.IO.Path.Combine(Application.streamingAssetsPath, "solutions.txt");

		int[,] LFCharray = { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
		lockedFace = new FaceMap(LFCharray);
		int[,] AFCharray = { { 1, 1,1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 }, { 1, 1, 1, 1 } };
		blankFace = new FaceMap(AFCharray);
		List<FaceMap> uiCubeFML = new List<FaceMap>();

		//uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace);
        for (int i = 0; i < 6; i++) {
            uiCubeFML.Add(blankFace);
        }
		uiCube = new CubeMap(4, 6, uiCubeFML);

		levelArray = new List<CubeMap>();
		NodeArray = new List<List<CubeMap>>();


        StartCoroutine("pathEnum");
		StartCoroutine("solutionPathEnum");

        //load
        levelTracker = new List<bool[]>();
        SaveLoad.Load(levelTracker);
    }

	// Update is called once per frame
	void Update() {
		mouseAudit();
		swipeAudit();

        if (Input.GetKeyDown(KeyCode.E)) {
            editMode = !editMode;
            print("edit mode " + (editMode ? "ON" : "OFF"));
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            resetLevelTracker();
        }

        if (editMode)
		    toolAudit ();
	}

	//path ienumerator
	IEnumerator pathEnum()
	{
        string result = "";
		if (levelPath.Contains("://"))
		{
			WWW www = new WWW(levelPath);
			yield return www;
			result = www.text;
		} else {
			result = System.IO.File.ReadAllText(levelPath);
		}
        csvrCall.readCSV(result);
    }

	//solution path ienumerator
	IEnumerator solutionPathEnum() {
        string solutionResult = "";
		if (solutionPath.Contains("://")) {
			WWW swww = new WWW(solutionPath);
			yield return swww;
			solutionResult = swww.text;
			//txtCall.readTXT(solutionResult, solutionArray);
		}
		else
		{
			solutionResult = System.IO.File.ReadAllText(solutionPath);
			//txtCall.readTXT(solutionResult, solutionArray);
		}
        txtCall.readTXT(solutionResult, solutionArray);
    }

    void toolAudit(){
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            int targetColor = paintSlug + (int)Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
            if (targetColor < 0)
                targetColor += 13;

            targetColor %= 13;
            paintToolLoad(targetColor);
            paintDebug = true;
        }

        if (editMode && Input.GetKeyDown(KeyCode.S)) {
            printSolutions();
        }

        if (editMode && Input.GetKeyDown(KeyCode.M)) {
			string levelCode = "";
			List<Tile> tileSortList = new List<Tile> ();

			foreach (GameObject aTile in tileObjArray) {
				//if (aTile.GetComponent<Tile> ().fNo <= 3) {
					tileSortList.Add (aTile.GetComponent<Tile> ());
				//}
			}

            //tileSortList = tileSortList.OrderByDescending (x => x.fNo).ToList();

            //TODO: JOSE this needs to become more flexible to accomedate for more faces
            int numFaces = NodeArray[currentNode][currentLevel].dimension;
            print(numFaces);

			for (int i = 0; i < tileSortList.Count; i++) {
				levelCode += (char) (tileSortList [i].tileType + 97);

				if (i!=0 && (i+1) % Mathf.Pow(numFaces, 2) == 0) {
					levelCode += ",";
				}
				else if (i!=0 && i != tileSortList.Count - 1 && (i+1)% numFaces == 0) {
					levelCode += ".";
				}

			}

			Debug.Log (levelCode);
		}
	}

	void paintToolLoad(int fedInt) {
		paintSlug = fedInt;
		remoteLR.material.SetColor("_EmissionColor", tileColorArray[fedInt]);
		audioSrc.PlayOneShot(gateClick, 0.8F);

	}

    // allows you to spin the cube horizontally to change the face in front of you
	void swipeAudit()
	{
		if (nodeSelect && !swiping)
		{
			if (Input.GetKeyDown("left"))
			{
				StartCoroutine(translateNodeBills(true));
				swiping = true;
			}
			else if (Input.GetKeyDown("right"))
			{
				StartCoroutine(translateNodeBills(false));
				swiping = true;
			}
		}
	}

    // manages face rotation on swipe
	public IEnumerator translateNodeBills(bool isLeft)
	{
        // JOSE TODO: Switched everything so its in arrays. There is probably an even better way to do this is I can find a pattern between 
        // each coordinate, but I'll tackle that another time

        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(10, 10, -5);
        positions[1] = new Vector3(0, 10, 5);
        positions[2] = new Vector3(-10, 10, -5);     
        positions[3] = new Vector3(0, 10, -10);

        float progress = 0f;
		float movespeed = 1f;
		float startTime = Time.time;
		int indexTarget;
        
		if (isLeft) {
			indexTarget = currentNode+3;
            indexTarget %= nodeKits.Count;
			GameObject newNodeBill4 = Instantiate(nodeKits[indexTarget], positions[3], Quaternion.Euler(0, -180, 0)) as GameObject;
			activeKits.Add(newNodeBill4);
            Destroy(activeKits[0]);
        } else {
			indexTarget = currentNode-1;

            if (indexTarget < 0)
                indexTarget += nodeKits.Count;

            indexTarget %= nodeKits.Count;
            GameObject newNodeBill4 = Instantiate(nodeKits[indexTarget], positions[3], Quaternion.Euler(0, -180, 0)) as GameObject;
			activeKits.Insert(0, newNodeBill4);
            Destroy(activeKits[3]);
        }
        
		while (progress < 1) {
			progress = Time.time - startTime;
			if (isLeft) {
                for (int i = 1; i < 4; i++)  {
                    activeKits[i].transform.position = Vector3.Lerp(positions[i], positions[i - 1], progress);
                    activeKits[i].transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 90 - 90 * (i), 0), Quaternion.Euler(0, 90 - 90 * (i - 1), 0), progress);
                }

            } else {
                for (int i = 0; i < 3; i++)  {
                    int index = i - 1;
                    if (index == -1) {
                        index += 4;
                    }

                    activeKits[i].transform.position = Vector3.Lerp(positions[index], positions[i], progress);
                    activeKits[i].transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 90 - 90 * index, 0), Quaternion.Euler(0, 90 - 90 * (i), 0), progress);
                }
            }
			yield return null;
		}
        
        if (isLeft) {                   
            for (int i = 1; i < 4; i++) {
                activeKits[i].transform.position = positions[i - 1];
                activeKits[i].transform.rotation = Quaternion.Euler(0, 90 - 90 * (i - 1), 0);
            }    
            activeKits.RemoveAt(0);
            currentNode -= 1;

        } else {
            for (int i = 0; i < 3; i++) {
                activeKits[i].transform.position = positions[i];
                activeKits[i].transform.rotation = Quaternion.Euler(0, 90 - 90 * i, 0);
            }        
            activeKits.RemoveAt(3);
            currentNode += 1;
        }

        if (currentNode == -1)
            currentNode += nodeKits.Count;

        currentNode %= nodeKits.Count;
        for (int i = 0; i < 3; i++) {
            activeKits[i].GetComponent<nodeBillScript>().isActive = i == 1;
        }
        swiping = false;
	}

	void mouseAudit() {
		#if UNITY_ANDROID
		if (GvrController.ClickButton == true) { 		//touch pad platformSpec
		#else
		if (Input.GetMouseButton(0)) { 		//editor mouse alt
		#endif           
			clickTarget = cursorCall.focusedTile;
			//ui check
			#if UNITY_ANDROID
			if (levelCompleted && GvrController.ClickButtonDown) {
			#else
			if (levelCompleted && Input.GetMouseButtonDown (0)) {
			#endif
				levelJanitor ();
			}

            if (clickTarget == null) {

#if UNITY_ANDROID
                if (GvrController.ClickButtonDown)
#else
                if(Input.GetMouseButtonDown(0))
#endif
                {
                    // if you click the globe podium, open up pack select // JOSE: something I added, just so it doesnt throw an error if the focusedGM is deleted
                    if (cursorCall.focusedGM != null) {
                        if (cursorCall.focusedGM.name == "Compy" && !activeCubeFlag) {
                            packSelectDraw();
                        }

                        // if its a pack. open up level select
                        if (cursorCall.focusedGM.name == "Bill" && nodeSelect && cursorCall.focusedGM.GetComponentInParent<nodeBillScript>().isActive == true) {
                            levelSelectDraw();
                        }

                        // if you click the globe podium, open up pack select
                        if (cursorCall.focusedGM.GetComponent<VRUIButton>() != null) {
                            cursorCall.focusedGM.GetComponent<VRUIButton>().onUIDown.Invoke();
                        }
                    }
                }
            }

            else if (clickTarget.isActive || editMode) {
                /*
                //Reed's code
                if (clickTarget.index!= 99 && clickTarget.index != 0 && levelTracker[clickTarget.fNo-1][clickTarget.index-1]==true || clickTarget.index==1) {//selected valid level
					currentLevel = clickTarget.index-2;
                    
					levelJanitor ();
				}
                /*/

                //go back to pack select
                if (clickTarget.index == 0) {
                    packSelectDraw();
                }
                //Jose's new code. A bit sloppy for now, but just so I can have more than 16 levels. 
                else if (clickTarget.index != 99 && clickTarget.index != 0 || clickTarget.index == 1) {//selected valid level
                    currentLevel = clickTarget.index - 2;
                    levelJanitor();
                }
                //*/

                else if (clickTarget.tileType < 26 && clickTarget.tileType > 1) {//is gate
                    if (paintDebug) {
                        updateClickedTile(clickTarget);
                        if (paintSlug > 1)
                        {
                            paintDebug = false;
                            paintToolLoad(1);
                        }
                    } else if (clickTarget.tileType > 1) {
                        if (clickTarget.tileType != paintSlug - 24) {
                            updatePaintSlug();
                        }
                        //is gate, hint code
                        if (hintReady == false) {
                            downTime = Time.time;
                            pressTime = downTime + hintTimer;
                            hintReady = true;
                        }
                        if (Time.time >= pressTime && hintReady == true) {
                            paintHint(clickTarget.tileType, currentLevel);
                            hintReady = false;
                        }
                    }
                }
                else
                { //not gate
                    hintReady = false;
                    if (clickTarget.tileType != paintSlug && (clickTarget.tileType != 0 || paintSlug <= 24)) {
                        updateClickedTile(clickTarget);
                    }
                }
                
            }
		}

#if UNITY_ANDROID
		if (GvrController.AppButtonDown && !levelCompleted) { //right click clear cube
#else
		if (Input.GetMouseButtonDown (1) && !levelCompleted) { //right click clear cube
#endif
			paintSlug = 1;
			remoteLR.material.SetColor("_EmissionColor", tileColorArray[1]);
			foreach (Tile tileTarget in tileArray) {
				if (tileTarget.isActive && tileTarget.tileType > 25) {
					tileTarget.tileType = 1;
					tileTarget.updateFlag = true;
				}
			}
		}

#if UNITY_ANDROID
		if (GvrController.ClickButtonUp) {
#else
		if (Input.GetMouseButtonUp (0)) {
#endif
			hintReady = false;
		}
	}

	void updatePaintSlug() {
		paintSlug = clickTarget.tileType+24;
		remoteLR.material.SetColor("_EmissionColor", tileColorArray[clickTarget.tileType + 24]);
		audioSrc.PlayOneShot(gateClick, 0.8F);
	}

	void updateClickedTile(Tile tilePointer) {
		tilePointer.tileType = 	paintSlug;
		audioSrc.PlayOneShot(tileClick, 0.8F);
		//update neighbors
		foreach (Tile aTile in tileArray) {
			aTile.getNeighbors();
			aTile.contiguousLink = false;
		}
		//check links
		for (int i = 0; i<linkStateArray.Length; i++) {
			linkStateArray [i] = 0;
		}
			
		foreach (Tile gateTile in gateArray) {
			pingedTiles.Clear ();
			gateTile.pingNeighbors ();
		}

		//update tiles
		foreach (Tile eaTile in tileArray) {
			eaTile.updateFlag = true;
		}

		//check level complete
		if (!levelCompleted && !paintDebug && !editMode) {
			for (int i = 2; i < levelColorCount+2; i++) {
				if (linkStateArray [i] != 1) {
					return;
				}
			}
			levelBookend ();
			//levelJanitor ();
		}
	}

	void levelBookend() {
		levelCompleted = true;
        printSolutions();
        //save level win. Need to Figure this out...
        levelTracker[currentNode][currentLevel+1]=true;
		SaveLoad.Save(levelTracker);

		foreach (Tile eaTile in tileArray) {
			eaTile.isActive = false;
		}
		GameObject newBoard = Instantiate(uiBoard, new Vector3 (0,0,0), Quaternion.identity) as GameObject;
		GameObject newHeader = Instantiate(uiHeader, new Vector3 (0,0,0), Quaternion.identity) as GameObject;
		uiObjects.Add (newHeader);
		uiObjects.Add (newBoard);
		newBoard.transform.SetParent (uiPointer, false);
		newHeader.transform.SetParent (uiPointer, false);
		newBoard.transform.localPosition = new Vector3 (5, 5.5f, 0);
		newHeader.transform.localPosition = new Vector3 (5, 7.8f, -0.7f);
		foreach (GameObject tileIcon in tileIconArray) {
			Destroy (tileIcon);
		}
		tileIconArray.Clear ();

	}

    // clears old level and opens new level
	void levelJanitor() {
		currentLevel += 1;
		levelColorCount = 0;
        clearTiles();

        //load new level
        factoryCall.drawCube (NodeArray[currentNode][currentLevel], false);
		ceilingLight.color = Color.white;
		ceilingLight.intensity = 1.3f;
		activeCubeFlag = true;	
	}

	void paintHint(int colorPointer, int levelNo) {
		foreach (int coOrdSet in solutionArray[levelNo].coOrdMap[colorPointer-2].coordinates) {
			//Debug.Log (coOrdSet);
			string coOrdSetString = coOrdSet.ToString();
			int fPoint = (int)char.GetNumericValue (coOrdSetString [0]);
			int xPoint = (int)char.GetNumericValue(coOrdSetString [1]);
			int yPoint = (int)char.GetNumericValue(coOrdSetString [2]);
			foreach (Tile tPoint in tileArray) {
				if (tPoint.fNo == fPoint && tPoint.xPos == xPoint && tPoint.yPos == yPoint) {
					tPoint.tileType = colorPointer + 24;
					updateClickedTile (tPoint);
				}
			}
		}
	}

	void packSelectDraw() {
        //JOSE TODO: (Possible Improvement) Destroying computron throws a null reference error. Meaning it is probably referenced somwhere else in the code
        //On top of that, if we ever wnat to get back to computron, we wont be able to (might have solved this on line 377)
        Destroy(computron);
        activeKits.Clear();
        clearTiles();
        GameObject newNodeBill = Instantiate(nodeKits[0], new Vector3 (-10,10,-5), Quaternion.identity) as GameObject;
		GameObject newNodeBill2 = Instantiate (nodeKits[1], new Vector3 (0, 10, 5), Quaternion.identity) as GameObject;
		GameObject newNodeBill3 = Instantiate(nodeKits[2], new Vector3(10, 10, -5), Quaternion.identity) as GameObject;
		newNodeBill.transform.Rotate(0, -90, 0);
		newNodeBill3.transform.Rotate (0, 90, 0);
		activeKits.Add (newNodeBill);
		activeKits.Add (newNodeBill2);
		activeKits.Add(newNodeBill3);
		newNodeBill2.GetComponent<nodeBillScript> ().isActive = true;
		nodeSelect = true;
    }

	void levelSelectDraw() {
        //JOSE TODO: (Possible Improvement) Destroying level select draw seems to throw an error. (might have solved this on line 377)
        foreach (GameObject aObj in activeKits) {
			Destroy (aObj);
		}
        clearTiles();

		factoryCall.drawCube (uiCube, true);
		ceilingLight.color = Color.white;
		ceilingLight.intensity = 1.3f;
		nodeSelect = false;
		activeCubeFlag = true;
	}

    public void clearTiles() {
        gateArray.Clear();
        tileArray.Clear();
        foreach (GameObject eachTile in tileObjArray)
        {
            Destroy(eachTile);
        }
        tileObjArray.Clear();
        foreach (GameObject eachObj in uiObjects)
        {
            Destroy(eachObj);
        }
        uiObjects.Clear();
        for (int i = 0; i < linkStateArray.Length; i++)
        {
            linkStateArray[i] = 0;
        }

        paintSlug = 1;
        remoteLR.material.SetColor("_EmissionColor", tileColorArray[1]);
        activeCubeFlag = false;
        levelCompleted = false;
    }

    public void resetLevelTracker() {
        levelTracker = new List<bool[]>();
        for (int i = 0; i < NodeArray.Count; i++) {
            if (NodeArray[i].Count > 0) {
                levelTracker.Add(new bool[NodeArray[i].Count]);
            }
        }
        SaveLoad.Save(levelTracker);
    }

    // being able to update the level tracker's size without messing with the current tracker. Still is a work in progress
    public void updateLevelTracker() {
        for (int i = 0; i < NodeArray.Count; i++) {
            if (i >= levelTracker.Count) {
                levelTracker.Add(new bool[NodeArray[i].Count]);
            } else {
                
            }
        }
        SaveLoad.Save(levelTracker);
    }

    public void printSolutions() {

        string solution = "";
        for (int i = 0; i < gateArray.Count; i++) {
            solution += gateArray[i].tileType + ":";

            bool firstOne = true;
            for (int j = 0; j < tileArray.Count; j++) { //there is probably a waaaaaay better way of doing this. 
                if (tileArray[j].tileType == gateArray[i].tileType + 24) {
                    if (firstOne) {
                        solution += "" + tileArray[j].fNo + tileArray[j].xPos + tileArray[j].yPos;
                        firstOne = false;
                    }
                    else {
                        solution += "." + tileArray[j].fNo + tileArray[j].xPos + tileArray[j].yPos;
                    }
                }
            }
            solution += ",";
        }

        StreamWriter writer = new StreamWriter(solutionPath, true);
        writer.WriteLine("\n" + solution);
        writer.Close();

        print(solution);
    }

    public bool validLevel(int currentNode, int currentLevel) {
        return NodeArray.Count > currentNode && NodeArray[currentNode].Count > currentLevel;
    }

    public bool checkLevelStatus(int currentNode, int currentLevel) {
        return levelTracker[currentNode][currentLevel];
    }

}