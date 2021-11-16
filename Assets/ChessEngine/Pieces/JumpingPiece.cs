using Vector2Int = UnityEngine.Vector2Int;

public abstract class JumpingPiece : Piece
{
	public JumpingPiece(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { } 

    public void FindJumpMove(Vector2Int offset)
	{
		Vector2Int checkedPosition = Square.Position + offset;

		if (checkedPosition.x < Board.LEFT_FILE_INDEX || checkedPosition.x > Board.RIGHT_FILE_INDEX || // square outside board
				checkedPosition.y < Board.BOTTOM_RANK_INDEX || checkedPosition.y > Board.TOP_RANK_INDEX)
			return;

		Square checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];

		if (!checkedSquare.IsOccupied() || // empty square
			(checkedSquare.IsOccupied() && checkedSquare.Piece.Color != Color)) // opponent piece
		{
			SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece));
		}
	}
}
