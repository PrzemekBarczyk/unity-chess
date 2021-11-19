using System.Collections.Generic;

public class MoveExecutor
{
	Stack<RightsData> _rightsHistory = new Stack<RightsData>();

	Board _board;

	public MoveExecutor(Board board)
	{
		_board = board;
	}

	public void MakeMove(Move moveToMake)
	{
		RightsData currentRights = new RightsData(_board.EnPassantTarget, moveToMake.Piece.Pieces.CanKingCastleKingside, moveToMake.Piece.Pieces.CanKingCastleQueenside);
		_rightsHistory.Push(currentRights);

		if (moveToMake.EncounteredPiece != null)
		{
			moveToMake.EncounteredPiece.IsAlive = false;
			moveToMake.EncounteredPiece.Square.Piece = null;
		}

		moveToMake.OldSquare.Piece = null;
		moveToMake.NewSquare.Piece = moveToMake.Piece;
		moveToMake.Piece.Square = moveToMake.NewSquare;

		if (moveToMake.Piece.Type == PieceType.Pawn)
		{
			bool madeDoubleMove = moveToMake.NewSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? 3 : 4) && moveToMake.OldSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? 1 : 6);
			if (madeDoubleMove)
			{
				_board.EnPassantTarget = _board.Squares[moveToMake.OldSquare.Position.x][moveToMake.OldSquare.Position.y + (moveToMake.Piece.Color == ColorType.White ? 1 : -1)];
			}
			else
			{
				_board.EnPassantTarget = null; // removes unactive old en passants

				if (moveToMake.IsPromotion)
				{
					Piece oldPiece = moveToMake.Piece;
					oldPiece.IsAlive = false;

					Piece promotion;

					switch (moveToMake.Type)
					{
						case MoveType.PromotionToKnight:
							promotion = new Piece(_board, oldPiece.Pieces, oldPiece.Color, PieceType.Knight, oldPiece.Square.Position);
							promotion.Pieces.Knights.Add(promotion);
							break;
						case MoveType.PromotionToBishop:
							promotion = new Piece(_board, oldPiece.Pieces, oldPiece.Color, PieceType.Bishop, oldPiece.Square.Position);
							promotion.Pieces.Bishops.Add(promotion);
							break;
						case MoveType.PromotionToRook:
							promotion = new Piece(_board, oldPiece.Pieces, oldPiece.Color, PieceType.Rook, oldPiece.Square.Position);
							promotion.Pieces.Rooks.Add(promotion);
							break;
						case MoveType.PromotionToQueen:
							promotion = new Piece(_board, oldPiece.Pieces, oldPiece.Color, PieceType.Queen, oldPiece.Square.Position);
							promotion.Pieces.Queens.Add(promotion);
							break;
						default:
							throw new System.Exception("Unknown piece type");
					}

					promotion.Pieces.AllPieces.Add(promotion);
				}
			}
		}
		else // piece different than pawn
		{
			_board.EnPassantTarget = null; // removes unactive old en passants

			if (moveToMake.Piece.Type == PieceType.Rook)
			{
				if (moveToMake.OldSquare.Position.x == Board.LEFT_FILE_INDEX && moveToMake.OldSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? Board.BOTTOM_RANK_INDEX : Board.TOP_RANK_INDEX))
				{
					moveToMake.Piece.Pieces.CanKingCastleQueenside = false;
				}
				else if (moveToMake.OldSquare.Position.x == Board.RIGHT_FILE_INDEX && moveToMake.OldSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? Board.BOTTOM_RANK_INDEX : Board.TOP_RANK_INDEX))
				{
					moveToMake.Piece.Pieces.CanKingCastleKingside = false;
				}
			}
			else if (moveToMake.Piece.Type == PieceType.King)
			{
				moveToMake.Piece.Pieces.CanKingCastleKingside = false;
				moveToMake.Piece.Pieces.CanKingCastleQueenside = false;

				if (moveToMake.Type == MoveType.Castle)
				{
					Piece rook = moveToMake.RookOldSquare.Piece;
					Move rookMove = new Move(rook, moveToMake.RookOldSquare, moveToMake.RookNewSquare, null);
					MakeMove(rookMove);
				}
			}
		}
	}

	public void UndoMove(Move moveToUndo)
	{
		if (moveToUndo.IsPromotion)
		{
			Piece oldPawn = moveToUndo.Piece;
			oldPawn.IsAlive = true;

			Piece promotion = moveToUndo.NewSquare.Piece;

			switch (moveToUndo.Type)
			{
				case MoveType.PromotionToKnight:
					promotion.Pieces.Knights.Remove(promotion);
					break;
				case MoveType.PromotionToBishop:
					promotion.Pieces.Bishops.Remove(promotion);
					break;
				case MoveType.PromotionToRook:
					promotion.Pieces.Rooks.Remove(promotion);
					break;
				case MoveType.PromotionToQueen:
					promotion.Pieces.Queens.Remove(promotion);
					break;
			}

			promotion.Pieces.AllPieces.Remove(promotion);
		}
		else if (moveToUndo.Type == MoveType.Castle)
		{
			Piece rook = moveToUndo.RookNewSquare.Piece;
			Move rookMove = new Move(rook, moveToUndo.RookOldSquare, moveToUndo.RookNewSquare, null);
			UndoMove(rookMove);
		}

		moveToUndo.OldSquare.Piece = moveToUndo.Piece;
		moveToUndo.NewSquare.Piece = null;
		moveToUndo.Piece.Square = moveToUndo.OldSquare;

		if (moveToUndo.EncounteredPiece != null)
		{
			moveToUndo.EncounteredPiece.IsAlive = true;
			moveToUndo.EncounteredPiece.Square.Piece = moveToUndo.EncounteredPiece;
		}

		RightsData previousRights = _rightsHistory.Pop();
		_board.EnPassantTarget = previousRights.EnPassantTarget;
		moveToUndo.Piece.Pieces.CanKingCastleKingside = previousRights.CanCastleKingside;
		moveToUndo.Piece.Pieces.CanKingCastleQueenside = previousRights.CanCastleQueenside;
	}
}
