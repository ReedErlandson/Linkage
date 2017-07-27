using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SolutionMap
{
	public List<Solution> coOrdMap;

	void Start() {
	}

	void Update(){
	}

	public SolutionMap(List<Solution> fedCoOrdMap) {
		coOrdMap = fedCoOrdMap;
	}

}