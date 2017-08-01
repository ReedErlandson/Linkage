using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// represents the correct sequence or coordinates on the tilemap for a specific tile type
public class Solution
{
	public List<int> coordinates;
	public int tileType;

	public Solution(int fedType, List<int> fedCoords) {
		tileType = fedType;
		coordinates = fedCoords;
	}

}