using System;
using UnityEngine;

public abstract class JumpingPiece : Piece
{
    public void FindJumpMove(Vector2Int offset)
	{
		Vector2Int checkedPosition = Square.Position + offset;

		Square checkedSquare;
		try
		{
			checkedSquare = _board.Squares[checkedPosition.x, checkedPosition.y];
		}
		catch (IndexOutOfRangeException) // square outside board
		{
			return;
		}

		if (!checkedSquare.IsOccupied || // empty square
			(checkedSquare.IsOccupied && checkedSquare.Piece.Color != Color)) // opponent piece
		{
			SaveMoveIfLegal(new MoveData(this, Square, checkedSquare, checkedSquare.Piece));
		}
	}
}
