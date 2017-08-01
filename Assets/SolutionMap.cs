using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// represents all the solutions to a given puzzle
public class SolutionMap
{
	public List<Solution> coOrdMap;

	public SolutionMap(List<Solution> fedCoOrdMap) {
		coOrdMap = fedCoOrdMap;
	}

}