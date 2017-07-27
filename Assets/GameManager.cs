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
	AudioSource audioSrc;
	public AudioClip tileClick;
	public AudioClip gateClick;

	//definitions
	public List<CubeMap> levelArray;
	public List<List<CubeMap>> NodeArray;
	char[,] LFCharray;
	public FaceMap lockedFace;
	public FaceMap blankFace;
	public CubeMap uiCube;
	public Color[] tileColorArray;

	//tiles
	Tile clickTarget;
	int paintSlug = 1;

	//ui
	public GameObject uiBoard;
	public GameObject uiHeader;
	public Transform uiPointer;
	public List<GameObject> uiObjects;
	GameObject computron;
	public List<GameObject> nodeKits;
	public List<GameObject> activeKits;

	//logic
	public List<Tile> tileArray;
	public List<GameObject> tileObjArray;
	public List<Tile> gateArray;
	public int[] wrapPointerArray; //left, up, right, down
	public int[] linkStateArray;
	public List<Tile> pingedTiles;
	public List<GameObject> tileIconArray;

	//hints
	public List<SolutionMap> solutionArray;
	public float hintTimer = 1.5f;
	public float downTime, upTime, pressTime = 0;
	public bool hintReady = false;

	//levels
	public int nodeCount = 2;
	int currentLevel = 0;
	int currentNode = 0;
	public int levelColorCount = 0;
	bool levelCompleted = false;
	bool nodeSelect = false;
	bool swiping = false;

	//loading progress
	public List<bool[]> levelTracker;

	//file path stuff
	public string levelPath;
	public string result = "";
	public string solutionPath;
	public string solutionResult = "";

	//light
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

		//load
		levelTracker = new List<bool[]>();
		SaveLoad.Load(levelTracker);

		//solutions
		solutionArray = new List<SolutionMap>();

		//light
		ceilingLight = GameObject.Find("CeilingLight").GetComponent<Light>();

		//path biz
		levelPath = System.IO.Path.Combine(Application.streamingAssetsPath, "csv1.csv");
		solutionPath = System.IO.Path.Combine(Application.streamingAssetsPath, "solutions.txt");

		char[,] LFCharray = { { '0', '0', '0', '0' }, { '0', '0', '0', '0' }, { '0', '0', '0', '0' }, { '0', '0', '0', '0' } };
		lockedFace = new FaceMap(LFCharray);
		char[,] AFCharray = { { '1', '1', '1', '1' }, { '1', '1', '1', '1' }, { '1', '1', '1', '1' }, { '1', '1', '1', '1' } };
		blankFace = new FaceMap(AFCharray);
		List<FaceMap> uiCubeFML = new List<FaceMap>();
		uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace); uiCubeFML.Add(blankFace);
		uiCube = new CubeMap(4, 6, uiCubeFML);

		levelArray = new List<CubeMap>();
		NodeArray = new List<List<CubeMap>>();

		StartCoroutine("pathEnum");
		StartCoroutine("solutionPathEnum");

	}


	// Update is called once per frame
	void Update()
	{
		mouseAudit();
		swipeAudit();
		toolAudit ();
	}

	//path ienumerator
	IEnumerator pathEnum()
	{
		if (levelPath.Contains("://"))
		{
			WWW www = new WWW(levelPath);
			yield return www;
			result = www.text;
			csvrCall.readCSV(result);
		}
		else
		{
			result = System.IO.File.ReadAllText(levelPath);
			csvrCall.readCSV(result);
		}
	}

	//solution path ienumerator
	IEnumerator solutionPathEnum()
	{
		if (solutionPath.Contains("://"))
		{
			WWW swww = new WWW(solutionPath);
			yield return swww;
			solutionResult = swww.text;
			txtCall.readTXT(solutionResult, solutionArray);
		}
		else
		{
			solutionResult = System.IO.File.ReadAllText(solutionPath);
			txtCall.readTXT(solutionResult, solutionArray);
		}
	}

	void toolAudit(){
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			paintToolLoad (0);
			paintDebug = true;
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			paintToolLoad (1);
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			paintToolLoad (2);
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			paintToolLoad (3);
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			paintToolLoad (4);
		} else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			paintToolLoad (5);
		} else if (Input.GetKeyDown (KeyCode.Alpha7)) {
			paintToolLoad (6);
		} else if (Input.GetKeyDown (KeyCode.Alpha8)) {
			paintToolLoad (7);
		} else if (Input.GetKeyDown (KeyCode.Alpha9)) {
			paintToolLoad (8);
		}
		if (paintDebug && Input.GetKeyDown(KeyCode.M)) {
			string levelCode = "";
			List<Tile> tileSortList = new List<Tile> ();

			foreach (GameObject aTile in tileObjArray) {
				if (aTile.GetComponent<Tile> ().fNo <= 3) {
					tileSortList.Add (aTile.GetComponent<Tile> ());
				}
			}

			//tileSortList = tileSortList.OrderByDescending (x => x.fNo).ToList();

			for (int i = 0; i < tileSortList.Count; i++) {
				levelCode += tileSortList [i].tileType.ToString ();

				if (i!=0 && (i+1) % 16 == 0) {
					levelCode += ",";
				}
				else if (i!=0 && i != tileSortList.Count - 1 && (i+1)%4 == 0) {
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

	void swipeAudit()
	{
		if (nodeSelect && !swiping)
		{
			if (Input.GetKeyDown("left") && currentNode < nodeCount)
			{
				StartCoroutine(translateNodeBills(true));
				swiping = true;
			}
			else if (Input.GetKeyDown("right") && currentNode > 0)
			{
				StartCoroutine(translateNodeBills(false));
				swiping = true;
			}
		}
	}

	public IEnumerator translateNodeBills(bool isLeft)
	{
		Vector3 frontPos = new Vector3(0, 10, 5);
		Vector3 leftPos = new Vector3(-10, 10, -5);
		Vector3 backPos = new Vector3(0, 10, -10);
		Vector3 rightPos = new Vector3(10, 10, -5);
		Quaternion frontRot = Quaternion.Euler(0,0,0);
		Quaternion rightRot = Quaternion.Euler(0, 90, 0);
		Quaternion backRot = Quaternion.Euler(0, 180, 0);
		Quaternion leftRot = Quaternion.Euler(0, -90, 0);
		float progress = 0f;
		float movespeed = 1f;
		float startTime = Time.time;
		int indexTarget;
		if (isLeft)
		{
			indexTarget = currentNode+3;
			GameObject newNodeBill4 = Instantiate(nodeKits[indexTarget], new Vector3(0, 10, -10), backRot) as GameObject;
			activeKits.Add(newNodeBill4);
		}
		else
		{
			indexTarget = currentNode-1;
			GameObject newNodeBill4 = Instantiate(nodeKits[indexTarget], new Vector3(0, 10, -10), backRot) as GameObject;
			activeKits.Insert(0, newNodeBill4);
		}
		while (progress < 1)
		{
			progress = Time.time - startTime;
			if (isLeft)
			{
				activeKits[0].transform.position = Vector3.Lerp(leftPos, backPos, progress);
				activeKits[0].transform.rotation = Quaternion.Lerp(leftRot, backRot, progress);
				activeKits[1].transform.position = Vector3.Lerp(frontPos, leftPos, progress);
				activeKits[1].transform.rotation = Quaternion.Lerp(frontRot, leftRot, progress);
				activeKits[2].transform.position = Vector3.Lerp(rightPos, frontPos, progress);
				activeKits[2].transform.rotation = Quaternion.Lerp(rightRot, frontRot, progress);
				activeKits[3].transform.position = Vector3.Lerp(backPos, rightPos, progress);
				activeKits[3].transform.rotation = Quaternion.Lerp(backRot, rightRot, progress);
			}
			else
			{
				Debug.Log(progress);
				activeKits[1].transform.position = Vector3.Lerp(leftPos, frontPos, progress);
				activeKits[1].transform.rotation = Quaternion.Lerp(leftRot, frontRot, progress);
				activeKits[2].transform.position = Vector3.Lerp(frontPos, rightPos, progress);
				activeKits[2].transform.rotation = Quaternion.Lerp(frontRot, rightRot, progress);
				activeKits[3].transform.position = Vector3.Lerp(rightPos, backPos, progress);
				activeKits[3].transform.rotation = Quaternion.Lerp(rightRot, backRot, progress);
				activeKits[0].transform.position = Vector3.Lerp(backPos, leftPos, progress);
				activeKits[0].transform.rotation = Quaternion.Lerp(backRot, leftRot, progress);
			}
			yield return null;
		}
		if (isLeft)
		{
			activeKits[0].transform.position = backPos;
			activeKits[0].transform.rotation = backRot;
			activeKits[1].transform.position = leftPos;
			activeKits[1].transform.rotation = leftRot;
			activeKits[2].transform.position = frontPos;
			activeKits[2].transform.rotation = frontRot;
			activeKits[3].transform.position = rightPos;
			activeKits[3].transform.rotation = rightRot;
		}
		else
		{
			activeKits[1].transform.position = frontPos;
			activeKits[1].transform.rotation = frontRot;
			activeKits[2].transform.position = rightPos;
			activeKits[2].transform.rotation = rightRot;
			activeKits[3].transform.position = backPos;
			activeKits[3].transform.rotation = backRot;
			activeKits[0].transform.position = leftPos;
			activeKits[0].transform.rotation = leftRot;
		}
		if (isLeft)
		{
			Destroy(activeKits[0]);
			activeKits.RemoveAt(0);
			currentNode += 1;
		}
		else
		{
			Destroy(activeKits[3]);
			activeKits.RemoveAt(3);
			currentNode -= 1;
		}
		activeKits[0].GetComponent<nodeBillScript> ().isActive = false;
		activeKits[1].GetComponent<nodeBillScript> ().isActive = true;
		activeKits[2].GetComponent<nodeBillScript> ().isActive = false;
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
				if (cursorCall.focusedGM.name == "Compy" && GvrController.ClickButtonDown) {
				#else
				if (cursorCall.focusedGM.name == "Compy" && Input.GetMouseButtonDown(0)) {
				#endif
					if (!activeCubeFlag) {
						packSelectDraw ();
					}
				}
				#if UNITY_ANDROID
				if (cursorCall.focusedGM.name == "Bill" && nodeSelect && cursorCall.focusedGM.GetComponentInParent<nodeBillScript>().isActive == true && GvrController.ClickButtonDown) {
				#else
				if (cursorCall.focusedGM.name == "Bill" && nodeSelect && cursorCall.focusedGM.GetComponentInParent<nodeBillScript>().isActive == true && Input.GetMouseButtonDown (0)) {
				#endif
					levelSelectDraw ();
				}
			}
			else if (clickTarget.isActive) {
				if (clickTarget.index!= 99 && clickTarget.index != 0 && levelTracker[clickTarget.fNo-1][clickTarget.index-1]==true || clickTarget.index==1) {//selected valid level
					currentLevel = clickTarget.index-2;
					levelJanitor ();
				}
				else if (clickTarget.tileType < 10 && clickTarget.tileType != 1) {//is gate
					if (clickTarget.tileType != paintSlug - 8) {
						updatePaintSlug ();
					}
					//is gate, hint code
					if (hintReady==false) {
						downTime = Time.time;
						pressTime = downTime + hintTimer;
						hintReady = true;
					}
					if (Time.time >= pressTime && hintReady==true) {
						paintHint (clickTarget.tileType, currentLevel);
						hintReady = false;
					}
				} else { //not gate
					hintReady = false;
					if (clickTarget.tileType != paintSlug) {
						updateClickedTile (clickTarget);
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
				if (tileTarget.isActive && tileTarget.tileType >9) {
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
		paintSlug = clickTarget.tileType+8;
		remoteLR.material.SetColor("_EmissionColor", tileColorArray[clickTarget.tileType + 8]);
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
		if (!levelCompleted && !paintDebug) {
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

		//save level win
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

	void levelJanitor() {

		currentLevel += 1;
		levelColorCount = 0;
		gateArray.Clear ();
		tileArray.Clear ();
		foreach (GameObject eachTile in tileObjArray) {
			Destroy (eachTile);
		}
		tileObjArray.Clear ();
		foreach (GameObject eachObj in uiObjects) {
			Destroy (eachObj);
		}
		uiObjects.Clear ();
		for (int i = 0; i < linkStateArray.Length; i++) {
			linkStateArray [i] = 0;
		}

		paintSlug = 1;
		remoteLR.material.SetColor("_EmissionColor", tileColorArray[1]);
		activeCubeFlag = false;
		levelCompleted = false;

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
					tPoint.tileType = colorPointer + 8;
					updateClickedTile (tPoint);
				}
			}
		}
	}

	void packSelectDraw() {
		Destroy (computron);
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
		foreach (GameObject aObj in activeKits) {
			Destroy (aObj);
		}
		factoryCall.drawCube (uiCube, true);
		ceilingLight.color = Color.white;
		ceilingLight.intensity = 1.3f;
		nodeSelect = false;
		activeCubeFlag = true;
	}

}