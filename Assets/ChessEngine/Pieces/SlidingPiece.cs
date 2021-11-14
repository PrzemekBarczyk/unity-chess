using System;
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

			Square checkedSquare;
			try
			{
				checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];
			}
			catch (IndexOutOfRangeException) // square outside board
			{
				return;
			}

			if (!checkedSquare.IsOccupied() && canMoveOnEmptySquare) // empty square
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
			else if (checkedSquare.IsOccupied() && checkedSquare.Piece.Color != Color && canAttack) // opponent piece
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
