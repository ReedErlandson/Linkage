using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CsvReader : MonoBehaviour
{
	//linkage
	GameManager managerCall;
	CubeFactory factoryCall;

	void Start () {

	}

	public void readCSV(string fedCSV) {
		managerCall = GetComponent<GameManager>();
		factoryCall = GetComponent<CubeFactory>();

		StringReader csvStringReader = new StringReader(fedCSV);
		string inp_ln;
		List<List<CubeMap>> tempNodeList = new List<List<CubeMap>> ();

		while((inp_ln=csvStringReader.ReadLine())!=null){

            // Accessibility
            inp_ln = inp_ln.Split('-')[0]; // comments marked by '-' are ignored by thing

            if (inp_ln == "") { // ignores empty lines
                continue;
            }

			for (int n = 0; n < managerCall.nodeCount; n++) {
				List<CubeMap> newList = new List<CubeMap> ();
				tempNodeList.Add (newList);
			}

			int dim = (int)char.GetNumericValue (inp_ln [0]);

			int nodeTarget = (int)char.GetNumericValue (inp_ln [1]);
			inp_ln = inp_ln.Substring(3);
			List<FaceMap> newFML = new List<FaceMap> ();
			string[] readFaceArray = inp_ln.Split (',');

			for (int f = 0; f < readFaceArray.Length; f++) { //for each face
				string[]readLineArray = readFaceArray[f].Split('.');
				int[,] charrayFuel = new int[dim,dim];

				for (int l = 0; l < readLineArray.Length; l++) { //for each row
					char[] readLineCharray = readLineArray[l].ToCharArray();
					for (int c = 0; c < readLineCharray.Length; c++) {
						charrayFuel [l, c] = (int)readLineCharray[c] - 97;
					}
				}
				FaceMap newFM = new FaceMap (charrayFuel);
				newFML.Add (newFM);

			}

			int fillVar = newFML.Count;

			for (int n = 0; n < 6-fillVar; n++) {
				newFML.Add(managerCall.lockedFace);
			}

			CubeMap newCube = new CubeMap (dim,readFaceArray.Length,newFML);
			tempNodeList[nodeTarget].Add(newCube);
		}
		foreach (List<CubeMap> cubeMapList in tempNodeList) {
			managerCall.NodeArray.Add (cubeMapList);
		}
	}		

}