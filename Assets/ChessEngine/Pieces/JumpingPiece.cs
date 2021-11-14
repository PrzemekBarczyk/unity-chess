using System;
using Vector2Int = UnityEngine.Vector2Int;

public abstract class JumpingPiece : Piece
{
	public JumpingPiece(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { } 

    public void FindJumpMove(Vector2Int offset)
	{
		Vector2Int checkedPosition = Square.Position + offset;

		Square checkedSquare;
		try
		{
			checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];
		}
		catch (IndexOutOfRangeException) // square outside board
		{
			return;
		}

		if (!checkedSquare.IsOccupied() || // empty square
			(checkedSquare.IsOccupied() && checkedSquare.Piece.Color != Color)) // opponent piece
		{
			SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece));
		}
	}
}
