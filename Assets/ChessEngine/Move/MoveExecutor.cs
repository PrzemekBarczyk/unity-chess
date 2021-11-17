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
		RightsData currentRights = new RightsData(_board.EnPassantTarget, moveToMake.Piece.Pieces.King.CanCastleKingside, moveToMake.Piece.Pieces.King.CanCastleQueenside);
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
					switch (moveToMake.Type)
					{
						case MoveType.PromotionToKnight:
							Knight newKnight = new Knight(_board, moveToMake.Piece.Pieces, moveToMake.NewSquare.Piece.Color, moveToMake.NewSquare.Position);
							newKnight.Pieces.AllPieces.Add(newKnight);
							newKnight.Pieces.Knights.Add(newKnight);
							break;
						case MoveType.PromotionToBishop:
							Bishop newBishop = new Bishop(_board, moveToMake.Piece.Pieces, moveToMake.NewSquare.Piece.Color, moveToMake.NewSquare.Position);
							newBishop.Pieces.AllPieces.Add(newBishop);
							newBishop.Pieces.Bishops.Add(newBishop);
							break;
						case MoveType.PromotionToRook:
							Rook newRook = new Rook(_board, moveToMake.Piece.Pieces, moveToMake.NewSquare.Piece.Color, moveToMake.NewSquare.Position);
							newRook.Pieces.AllPieces.Add(newRook);
							newRook.Pieces.Rooks.Add(newRook);
							break;
						case MoveType.PromotionToQueen:
							Queen newQueen = new Queen(_board, moveToMake.Piece.Pieces, moveToMake.NewSquare.Piece.Color, moveToMake.NewSquare.Position);
							newQueen.Pieces.AllPieces.Add(newQueen);
							newQueen.Pieces.Queens.Add(newQueen);
							break;
					}
					moveToMake.Piece.Pieces.AllPieces.Remove(moveToMake.Piece);
					return;
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
					moveToMake.Piece.Pieces.King.CanCastleQueenside = false;
				}
				else if (moveToMake.OldSquare.Position.x == Board.RIGHT_FILE_INDEX && moveToMake.OldSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? Board.BOTTOM_RANK_INDEX : Board.TOP_RANK_INDEX))
				{
					moveToMake.Piece.Pieces.King.CanCastleKingside = false;
				}
			}
			else if (moveToMake.Piece.Type == PieceType.King)
			{
				moveToMake.Piece.Pieces.King.CanCastleKingside = false;
				moveToMake.Piece.Pieces.King.CanCastleQueenside = false;

				if (moveToMake.Type == MoveType.Castle)
				{
					Rook rook = moveToMake.RookOldSquare.Piece as Rook;
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
			switch (moveToUndo.Type)
			{
				case MoveType.PromotionToKnight:
					moveToUndo.NewSquare.Piece.Pieces.AllPieces.Remove(moveToUndo.NewSquare.Piece);
					moveToUndo.NewSquare.Piece.Pieces.Knights.Remove(moveToUndo.NewSquare.Piece as Knight);
					break;
				case MoveType.PromotionToBishop:
					moveToUndo.NewSquare.Piece.Pieces.AllPieces.Remove(moveToUndo.NewSquare.Piece);
					moveToUndo.NewSquare.Piece.Pieces.Bishops.Remove(moveToUndo.NewSquare.Piece as Bishop);
					break;
				case MoveType.PromotionToRook:
					moveToUndo.NewSquare.Piece.Pieces.AllPieces.Remove(moveToUndo.NewSquare.Piece);
					moveToUndo.NewSquare.Piece.Pieces.Rooks.Remove(moveToUndo.NewSquare.Piece as Rook);
					break;
				case MoveType.PromotionToQueen:
					moveToUndo.NewSquare.Piece.Pieces.AllPieces.Remove(moveToUndo.NewSquare.Piece);
					moveToUndo.NewSquare.Piece.Pieces.Queens.Remove(moveToUndo.NewSquare.Piece as Queen);
					break;
			}

			Piece oldPawn = moveToUndo.Piece;
			oldPawn.Pieces.AllPieces.Add(oldPawn);
		}
		else if (moveToUndo.Type == MoveType.Castle)
		{
			Rook rook = moveToUndo.RookNewSquare.Piece as Rook;
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
		moveToUndo.Piece.Pieces.King.CanCastleKingside = previousRights.CanCastleKingside;
		moveToUndo.Piece.Pieces.King.CanCastleQueenside = previousRights.CanCastleQueenside;
	}
}
