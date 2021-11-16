using Vector2Int = UnityEngine.Vector2Int;

public abstract class SlidingPiece : Piece
{
	public SlidingPiece(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	protected void FindSlidingMoves(Vector2Int direction, int maxDistance = 8, bool canAttack = true, bool canMoveOnEmptySquare = true, bool canPromote = false)
	{
		Vector2Int checkedPosition = Square.Position;

		for (int i = 1; i <= maxDistance; i++)
		{
			checkedPosition += direction;

			if (checkedPosition.x < Board.LEFT_FILE_INDEX || checkedPosition.x > Board.RIGHT_FILE_INDEX || // square outside board
				checkedPosition.y < Board.BOTTOM_RANK_INDEX || checkedPosition.y > Board.TOP_RANK_INDEX)
				return;

			Square checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];

			if (canMoveOnEmptySquare && !checkedSquare.IsOccupied()) // empty square
			{
				if (canPromote && checkedSquare.IsPromotionSquare(Color))
				{
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToKnight));
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToBishop));
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToRook));
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToQueen));
					return;
				}
				else // normal move on empty square
				{
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece));
				}
			}
			else if (canAttack && checkedSquare.IsOccupied() && checkedSquare.Piece.Color != Color) // opponent piece
			{
				if (canPromote && checkedSquare.IsPromotionSquare(Color))
				{
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToKnight));
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToBishop));
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToRook));
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToQueen));
				}
				else // attack on square different that promoting
				{
					SaveMoveIfLegal(new Move(this, Square, checkedSquare, checkedSquare.Piece));
				}
				return;
			}
			else // if move not pseudo legal, don't search next
			{
				return;
			}
		}
	}
}
