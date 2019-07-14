using UnityEngine;
using System.Collections;

//<summary>
//Subclass for selecting oldest cell from container
//</summary>
public class OldestTreeMazeGenerator : TreeMazeGenerator {

	public OldestTreeMazeGenerator(int row, int column):base(row,column){
		
	}
	
	protected override int GetCellInRange(int max)
	{
		return 0;
	}
}
