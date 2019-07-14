using UnityEngine;
using System.Collections;

//<summary>
//Pure recursive maze generation.
//Use carefully for large mazes.
//</summary>
public class RecursiveMazeGenerator : BasicMazeGenerator {

	public RecursiveMazeGenerator(int rows, int columns):base(rows,columns){

	}

	public override void GenerateMaze ()
	{
		VisitCell (0, 0, Direction.Start);
	}

	private void VisitCell(int row, int column, Direction moveMade){
		Direction[] movesAvailable = new Direction[4];
		int movesAvailableCount = 0;

		do{
			movesAvailableCount = 0;

			//check move right
			if(column+1 < ColumnCount && !GetMazeCell(row,column+1).IsVisited){
				movesAvailable[movesAvailableCount] = Direction.Right;
				movesAvailableCount++;
			}else if(!GetMazeCell(row,column).IsVisited && moveMade != Direction.Left){
				GetMazeCell(row,column).WallRight = true;
			}
			//check move forward
			if(row+1 < RowCount && !GetMazeCell(row+1,column).IsVisited){
				movesAvailable[movesAvailableCount] = Direction.Front;
				movesAvailableCount++;
			}else if(!GetMazeCell(row,column).IsVisited && moveMade != Direction.Back){
				GetMazeCell(row,column).WallFront = true;
			}
			//check move left
			if(column > 0 && column-1 >= 0 && !GetMazeCell(row,column-1).IsVisited){
				movesAvailable[movesAvailableCount] = Direction.Left;
				movesAvailableCount++;
			}else if(!GetMazeCell(row,column).IsVisited && moveMade != Direction.Right){
				GetMazeCell(row,column).WallLeft = true;
			}
			//check move backward
			if(row > 0 && row-1 >= 0 && !GetMazeCell(row-1,column).IsVisited){
				movesAvailable[movesAvailableCount] = Direction.Back;
				movesAvailableCount++;
			}else if(!GetMazeCell(row,column).IsVisited && moveMade != Direction.Front){
				GetMazeCell(row,column).WallBack = true;
			}

			if(movesAvailableCount == 0 && !GetMazeCell(row,column).IsVisited){
				GetMazeCell(row,column).IsGoal = true;
			}

			GetMazeCell(row,column).IsVisited = true;

			if(movesAvailableCount > 0){
				switch (movesAvailable[Random.Range(0,movesAvailableCount)]) {
				case Direction.Start:
					break;
				case Direction.Right:
					VisitCell(row,column+1,Direction.Right);
					break;
				case Direction.Front:
					VisitCell(row+1,column,Direction.Front);
					break;
				case Direction.Left:
					VisitCell(row,column-1,Direction.Left);
					break;
				case Direction.Back:
					VisitCell(row-1,column,Direction.Back);
					break;
				}
			}

		}while(movesAvailableCount > 0);
	}
}
