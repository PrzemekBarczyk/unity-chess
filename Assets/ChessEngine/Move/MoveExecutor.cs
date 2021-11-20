using System.Collections.Generic;

public class MoveExecutor
{
	Stack<RightsData> _rightsHistory = new Stack<RightsData>();

	public ulong _zobristKey;

	Board _board;
	PieceManager _pieceManager;

	public MoveExecutor(Board board, PieceManager pieceManager, ColorType currentPlayerColor)
	{
		_board = board;
		_pieceManager = pieceManager;

		_zobristKey = ZobristKey.CalculateZobristKey(_board, _pieceManager, currentPlayerColor);
	}

	public void MakeMove(Move moveToMake)
	{
		RightsData currentRights = new RightsData(_board.EnPassantTarget, moveToMake.Piece.Pieces.CanKingCastleKingside, moveToMake.Piece.Pieces.CanKingCastleQueenside);
		_rightsHistory.Push(currentRights);

		if (moveToMake.EncounteredPiece != null)
		{
			Piece encounteredPiece = moveToMake.EncounteredPiece;
			_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)encounteredPiece.Color - 1, (uint)encounteredPiece.Type - 1, encounteredPiece.Square.Position.x, encounteredPiece.Square.Position.y];

			encounteredPiece.IsAlive = false;
			encounteredPiece.Square.Piece = null;
		}

		_zobristKey ^= ZobristKey.sideToMove;
		_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)moveToMake.Piece.Color - 1, (uint)moveToMake.Piece.Type - 1, moveToMake.OldSquare.Position.x, moveToMake.OldSquare.Position.y];
		_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)moveToMake.Piece.Color - 1, (uint)moveToMake.Piece.Type - 1, moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y];

		moveToMake.OldSquare.Piece = null;
		moveToMake.NewSquare.Piece = moveToMake.Piece;
		moveToMake.Piece.Square = moveToMake.NewSquare;

		if (moveToMake.Piece.Type == PieceType.Pawn)
		{
			bool madeDoubleMove = moveToMake.NewSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? 3 : 4) && moveToMake.OldSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? 1 : 6);
			if (madeDoubleMove)
			{
				if (_board.EnPassantTarget != null) _zobristKey ^= ZobristKey.enPassantFiles[_board.EnPassantTarget.Position.x];
				_board.EnPassantTarget = _board.Squares[moveToMake.OldSquare.Position.x][moveToMake.OldSquare.Position.y + (moveToMake.Piece.Color == ColorType.White ? 1 : -1)];
				_zobristKey ^= ZobristKey.enPassantFiles[_board.EnPassantTarget.Position.x];
			}
			else
			{
				if (_board.EnPassantTarget != null) // removes unactive old en passants
				{
					_zobristKey ^= ZobristKey.enPassantFiles[_board.EnPassantTarget.Position.x];
					_board.EnPassantTarget = null;
				}

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

					_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)moveToMake.Piece.Color - 1, (uint)moveToMake.Piece.Type - 1, moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y];
					_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)promotion.Color - 1, (uint)promotion.Type - 1, moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y];
				}
			}
		}
		else // piece different than pawn
		{
			if (_board.EnPassantTarget != null) // removes unactive old en passants
			{
				_zobristKey ^= ZobristKey.enPassantFiles[_board.EnPassantTarget.Position.x];
				_board.EnPassantTarget = null;
			}

			if (moveToMake.Piece.Type == PieceType.Rook)
			{
				if (moveToMake.OldSquare.Position.x == Board.LEFT_FILE_INDEX && moveToMake.OldSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? Board.BOTTOM_RANK_INDEX : Board.TOP_RANK_INDEX))
				{
					if (moveToMake.Piece.Pieces.CanKingCastleQueenside)
					{
						_zobristKey ^= ZobristKey.castlingRights[moveToMake.Piece.Color == ColorType.White ? 1 : 3];
						moveToMake.Piece.Pieces.CanKingCastleQueenside = false;
					}
				}
				else if (moveToMake.OldSquare.Position.x == Board.RIGHT_FILE_INDEX && moveToMake.OldSquare.Position.y == (moveToMake.Piece.Color == ColorType.White ? Board.BOTTOM_RANK_INDEX : Board.TOP_RANK_INDEX))
				{
					if (moveToMake.Piece.Pieces.CanKingCastleKingside)
					{
						_zobristKey ^= ZobristKey.castlingRights[moveToMake.Piece.Color == ColorType.White ? 0 : 2];
						moveToMake.Piece.Pieces.CanKingCastleKingside = false;
					}
				}
			}
			else if (moveToMake.Piece.Type == PieceType.King)
			{
				if (moveToMake.Piece.Pieces.CanKingCastleKingside)
				{
					_zobristKey ^= ZobristKey.castlingRights[moveToMake.Piece.Color == ColorType.White ? 0 : 2];
					moveToMake.Piece.Pieces.CanKingCastleKingside = false;
				}
				if (moveToMake.Piece.Pieces.CanKingCastleQueenside)
				{
					_zobristKey ^= ZobristKey.castlingRights[moveToMake.Piece.Color == ColorType.White ? 1 : 3];
					moveToMake.Piece.Pieces.CanKingCastleQueenside = false;
				}

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

			_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)promotion.Color - 1, (uint)promotion.Type - 1, moveToUndo.NewSquare.Position.x, moveToUndo.NewSquare.Position.y];
			_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)moveToUndo.Piece.Color - 1, (uint)moveToUndo.Piece.Type - 1, moveToUndo.NewSquare.Position.x, moveToUndo.NewSquare.Position.y];
		}
		else if (moveToUndo.Type == MoveType.Castle)
		{
			Piece rook = moveToUndo.RookNewSquare.Piece;
			Move rookMove = new Move(rook, moveToUndo.RookOldSquare, moveToUndo.RookNewSquare, null);
			UndoMove(rookMove);
		}

		_zobristKey ^= ZobristKey.sideToMove;
		_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)moveToUndo.Piece.Color - 1, (uint)moveToUndo.Piece.Type - 1, moveToUndo.OldSquare.Position.x, moveToUndo.OldSquare.Position.y];
		_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)moveToUndo.Piece.Color - 1, (uint)moveToUndo.Piece.Type - 1, moveToUndo.NewSquare.Position.x, moveToUndo.NewSquare.Position.y];

		moveToUndo.OldSquare.Piece = moveToUndo.Piece;
		moveToUndo.NewSquare.Piece = null;
		moveToUndo.Piece.Square = moveToUndo.OldSquare;

		if (moveToUndo.EncounteredPiece != null)
		{
			Piece encounteredPiece = moveToUndo.EncounteredPiece;
			_zobristKey ^= ZobristKey.colorPiecePositionCombinations[(uint)encounteredPiece.Color - 1, (uint)encounteredPiece.Type - 1, encounteredPiece.Square.Position.x, encounteredPiece.Square.Position.y];

			encounteredPiece.IsAlive = true;
			encounteredPiece.Square.Piece = encounteredPiece;
		}

		RightsData previousRights = _rightsHistory.Pop();

		if (previousRights.EnPassantTarget != _board.EnPassantTarget)
		{
			if (_board.EnPassantTarget != null) _zobristKey ^= ZobristKey.enPassantFiles[_board.EnPassantTarget.Position.x];
			_board.EnPassantTarget = previousRights.EnPassantTarget;
			if (_board.EnPassantTarget != null) _zobristKey ^= ZobristKey.enPassantFiles[_board.EnPassantTarget.Position.x];
		}
		if (previousRights.CanCastleKingside != moveToUndo.Piece.Pieces.CanKingCastleKingside)
		{
			_zobristKey ^= ZobristKey.castlingRights[moveToUndo.Piece.Color == ColorType.White ? 0 : 2];
			moveToUndo.Piece.Pieces.CanKingCastleKingside = previousRights.CanCastleKingside;
		}
		if (previousRights.CanCastleQueenside != moveToUndo.Piece.Pieces.CanKingCastleQueenside)
		{
			_zobristKey ^= ZobristKey.castlingRights[moveToUndo.Piece.Color == ColorType.White ? 1 : 3];
			moveToUndo.Piece.Pieces.CanKingCastleQueenside = previousRights.CanCastleQueenside;
		}
	}
}
