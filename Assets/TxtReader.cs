using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TxtReader : MonoBehaviour
{
	//linkage
	GameManager managerCall;

	void Start () {

	}

	public void readTXT(string fedTXT, List<SolutionMap> fedList) {

		managerCall = GetComponent<GameManager>();

		StringReader txtStringReader = new StringReader(fedTXT);
		string inp_ln;
		while((inp_ln=txtStringReader.ReadLine())!=null){

			List<Solution> newSolList = new List<Solution>();

			List<int> readTypeArray = new List<int>();
			string[] readSolArray = inp_ln.Split (',');
			for (int l = 0; l < readSolArray.Length; l++) {
				readTypeArray.Add((int)char.GetNumericValue(readSolArray[l][0]));
				readSolArray [l] = readSolArray [l].Substring (2);
			}
			for (int m = 0; m < readSolArray.Length; m++) {
				List<int> newSolIntList = new List<int> ();
				string[] solBitAr = readSolArray [m].Split ('.');

				foreach (string aStr in solBitAr) {
					newSolIntList.Add (int.Parse(aStr));
				}
				Solution newSol = new Solution (readTypeArray [m], newSolIntList);
				newSolList.Add (newSol);
			}

			SolutionMap newSolMap = new SolutionMap (newSolList);
			fedList.Add (newSolMap);
		}
	}		

}