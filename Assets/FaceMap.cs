using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FaceMap
{
	public bool activeFace;
	public int[,] gridArray;

	public FaceMap(int[,] newGrid) {
		gridArray = newGrid;
	}
}