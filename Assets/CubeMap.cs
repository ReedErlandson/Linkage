using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeMap
{
	public int dimension;
	public int numFaces;
	public List<FaceMap> fMapList;

	public CubeMap(int newDimension, int newFaces, List<FaceMap> nFML) {
		dimension = newDimension;
		numFaces = newFaces;
		fMapList = nFML;
	}
}