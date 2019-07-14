using UnityEngine;
using System.Collections;

//<summary>
//Subclass for selecting last cell from container.
//Result equal to Recursive algorithm, so TreeMazeGenerator becomes non-recursive realisation of RecursiveGenerator = )
//</summary>
public class RecursiveTreeMazeGenerator : TreeMazeGenerator {
	
	public RecursiveTreeMazeGenerator(int row, int column):base(row,column){
		
	}
	
	protected override int GetCellInRange(int max)
	{
		return max;
	}
}
