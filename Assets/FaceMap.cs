using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceMap
{
	public bool activeFace;
	public char[,] gridArray;

	public FaceMap(char[,] newGrid) {
		gridArray = newGrid;
	}
}