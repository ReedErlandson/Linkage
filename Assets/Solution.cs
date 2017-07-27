using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Solution
{
	public List<int> coordinates;
	public int tileType;

	void Start() {
	}

	void Update(){
	}

	public Solution(int fedType, List<int> fedCoords) {
		tileType = fedType;
		coordinates = fedCoords;
	}

}