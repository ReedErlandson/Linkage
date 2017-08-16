﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TxtReader : MonoBehaviour
{
	//linkage
	GameManager managerCall;

    //fedTXT reads the solution txt and stores the solution
	public void readTXT(string fedTXT, List<SolutionMap> fedList) {
		managerCall = GetComponent<GameManager>();

		StringReader txtStringReader = new StringReader(fedTXT);
		string inp_ln;
		while((inp_ln=txtStringReader.ReadLine())!=null){  // for every solution in the fedTXT file...

			List<Solution> newSolList = new List<Solution>();

			List<int> readTypeArray = new List<int>();
			string[] readSolArray = inp_ln.Split (','); // splits it into its individual tile types
			for (int l = 0; l < readSolArray.Length; l++) {
				readTypeArray.Add((int)char.GetNumericValue(readSolArray[l][0])); // this is the tile type
				readSolArray [l] = readSolArray [l].Substring (2); // this is the pattern
			}

			for (int m = 0; m < readSolArray.Length; m++) {
				List<int> newSolIntList = new List<int> ();
				string[] solBitAr = readSolArray [m].Split ('.'); // split it by tile coordinate

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