using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;

public class MoveGenerator
{
	List<Move> _legalMoves = new List<Move>(128);

	Board _board;
	MoveExecutor _moveExecutor;

	public MoveGenerator(Board board, MoveExecutor moveExecutor)
	{
		_board = board;
		_moveExecutor = moveExecutor;
	}

	public List<Move> GenerateLegalMoves(PieceSet pieces)
	{
		_legalMoves.Clear();

		GeneratePawnMoves(pieces.Pawns);
		GenerateKnightsMoves(pieces.Knights);
		GenerateBishopMoves(pieces.Bishops);
		GenerateRookMoves(pieces.Rooks);
		GenerateQueensMoves(pieces.Queens);
		GenerateKingMoves(pieces.King);

		return _legalMoves;
	}

	void GeneratePawnMoves(List<Pawn> pawns)
	{
		for (int i = 0; i < pawns.Count; i++)
		{
			Pawn pawn = pawns[i];

			if (pawn.IsAlive)
			{
				if (pawn.OnStartingPosition) FindSlidingMoves(pawn, new Vector2Int(0, pawn.DirectionModifier), 2, canAttack: false, canMoveOnEmptySquare: true);
				else FindSlidingMoves(pawn, new Vector2Int(0, pawn.DirectionModifier), 1, canAttack: false, canMoveOnEmptySquare: true, canPromote: true);

				FindSlidingMoves(pawn, new Vector2Int(-1, pawn.DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);
				FindSlidingMoves(pawn, new Vector2Int(1, pawn.DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);

				if (pawn.OnPositionValidForEnPassant)
				{
					FindEnPassantMoves(pawn, false);
					FindEnPassantMoves(pawn, true);
				}
			}
		}
	}

	void GenerateKnightsMoves(List<Knight> knights)
	{
		for (int i = 0; i < knights.Count; i++)
		{
			Piece knight = knights[i];

			if (knight.IsAlive)
			{
				FindJumpMove(knight, new Vector2Int(-1, 2));
				FindJumpMove(knight, new Vector2Int(1, 2));
				FindJumpMove(knight, new Vector2Int(2, 1));
				FindJumpMove(knight, new Vector2Int(2, -1));
				FindJumpMove(knight, new Vector2Int(1, -2));
				FindJumpMove(knight, new Vector2Int(-1, -2));
				FindJumpMove(knight, new Vector2Int(-2, -1));
				FindJumpMove(knight, new Vector2Int(-2, 1));
			}
		}
	}

	void GenerateBishopMoves(List<Bishop> bishops)
	{
		for (int i = 0; i < bishops.Count; i++)
		{
			Piece bishop = bishops[i];

			if (bishop.IsAlive)
			{
				FindSlidingMoves(bishop, new Vector2Int(1, 1));
				FindSlidingMoves(bishop, new Vector2Int(1, -1));
				FindSlidingMoves(bishop, new Vector2Int(-1, -1));
				FindSlidingMoves(bishop, new Vector2Int(-1, 1));
			}
		}
	}

	void GenerateRookMoves(List<Rook> rooks)
	{
		for (int i = 0; i < rooks.Count; i++)
		{
			Piece rook = rooks[i];

			if (rook.IsAlive)
			{
				FindSlidingMoves(rook, new Vector2Int(0, 1));
				FindSlidingMoves(rook, new Vector2Int(1, 0));
				FindSlidingMoves(rook, new Vector2Int(0, -1));
				FindSlidingMoves(rook, new Vector2Int(-1, 0));
			}
		}
	}

	void GenerateQueensMoves(List<Queen> queens)
	{
		for (int i = 0; i < queens.Count; i++)
		{
			Piece queen = queens[i];

			if (queen.IsAlive)
			{
				FindSlidingMoves(queen, new Vector2Int(0, 1));
				FindSlidingMoves(queen, new Vector2Int(1, 1));
				FindSlidingMoves(queen, new Vector2Int(1, 0));
				FindSlidingMoves(queen, new Vector2Int(1, -1));
				FindSlidingMoves(queen, new Vector2Int(0, -1));
				FindSlidingMoves(queen, new Vector2Int(-1, -1));
				FindSlidingMoves(queen, new Vector2Int(-1, 0));
				FindSlidingMoves(queen, new Vector2Int(-1, 1));
			}
		}
	}

	void GenerateKingMoves(King king)
	{
		if (king.IsAlive)
		{
			FindSlidingMoves(king, new Vector2Int(-1, 1), 1);
			FindSlidingMoves(king, new Vector2Int(0, 1), 1);
			FindSlidingMoves(king, new Vector2Int(1, 1), 1);
			FindSlidingMoves(king, new Vector2Int(1, 0), 1);
			FindSlidingMoves(king, new Vector2Int(1, -1), 1);
			FindSlidingMoves(king, new Vector2Int(0, -1), 1);
			FindSlidingMoves(king, new Vector2Int(-1, -1), 1);
			FindSlidingMoves(king, new Vector2Int(-1, 0), 1);

			if (king.CanCastleQueenside) FindCastlingMove(king, false);
			if (king.CanCastleKingside) FindCastlingMove(king, true);
		}
	}

	void FindSlidingMoves(Piece piece, Vector2Int direction, int maxDistance = 8, bool canAttack = true, bool canMoveOnEmptySquare = true, bool canPromote = false)
	{
		Vector2Int checkedPosition = piece.Square.Position;

		for (int i = 1; i <= maxDistance; i++)
		{
			checkedPosition += direction;

			if (checkedPosition.x < Board.LEFT_FILE_INDEX || checkedPosition.x > Board.RIGHT_FILE_INDEX || // square outside board
				checkedPosition.y < Board.BOTTOM_RANK_INDEX || checkedPosition.y > Board.TOP_RANK_INDEX)
				return;

			Square checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];

			if (canMoveOnEmptySquare && !checkedSquare.IsOccupied()) // empty square
			{
				if (canPromote && checkedSquare.IsPromotionSquare(piece.Color))
				{
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToKnight));
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToBishop));
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToRook));
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToQueen));
					return;
				}
				else // normal move on empty square
				{
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece));
				}
			}
			else if (canAttack && checkedSquare.IsOccupied() && checkedSquare.Piece.Color != piece.Color) // opponent piece
			{
				if (canPromote && checkedSquare.IsPromotionSquare(piece.Color))
				{
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToKnight));
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToBishop));
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToRook));
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece, MoveType.PromotionToQueen));
				}
				else // attack on square different that promoting
				{
					SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece));
				}
				return;
			}
			else // if move not pseudo legal, don't search next
			{
				return;
			}
		}
	}

	void FindJumpMove(Piece piece, Vector2Int offset)
	{
		Vector2Int checkedPosition = piece.Square.Position + offset;

		if (checkedPosition.x < Board.LEFT_FILE_INDEX || checkedPosition.x > Board.RIGHT_FILE_INDEX || // square outside board
			checkedPosition.y < Board.BOTTOM_RANK_INDEX || checkedPosition.y > Board.TOP_RANK_INDEX)
			return;

		Square checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];

		if (!checkedSquare.IsOccupied() || // empty square
			(checkedSquare.IsOccupied() && checkedSquare.Piece.Color != piece.Color)) // opponent piece
		{
			SaveMoveIfLegal(new Move(piece, _board.Squares[piece.Square.Position.x][piece.Square.Position.y], checkedSquare, checkedSquare.Piece));
		}
	}

	void FindEnPassantMoves(Pawn pawn, bool rightEnPassant)
	{
		Square newSquare;
		if (rightEnPassant)
		{
			if (pawn.Square.Position.x + 1 > Board.RIGHT_FILE_INDEX)
				return;
			newSquare = _board.Squares[pawn.Square.Position.x + 1][pawn.Square.Position.y + pawn.DirectionModifier];
		}
		else
		{
			if (pawn.Square.Position.x - 1 < Board.LEFT_FILE_INDEX)
				return;
			newSquare = _board.Squares[pawn.Square.Position.x - 1][pawn.Square.Position.y + pawn.DirectionModifier];
		}

		if (newSquare == _board.EnPassantTarget)
		{
			Piece encounteredPiece = _board.Squares[newSquare.Position.x][pawn.Square.Position.y].Piece;
			Move move = new Move(pawn, pawn.Square, newSquare, encounteredPiece);

			if (SaveMoveIfLegal(move))
				return;
		}
	}

	void FindCastlingMove(Piece king, bool kingsideCastle)
	{
		Vector2Int positionModifier;
		Square rookOldSquare;
		Square rookNewSquare;
		Vector2Int newKingPosition;

		if (kingsideCastle)
		{
			positionModifier = Vector2Int.right;
			if (king.Color == ColorType.White)
			{
				rookOldSquare = _board.Squares[Rook.WHITE_RIGHT_ROOK_START_POSITION.x][Rook.WHITE_RIGHT_ROOK_START_POSITION.y];
				rookNewSquare = _board.Squares[Rook.WHITE_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.x][Rook.WHITE_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.y];
				newKingPosition = King.WHITE_KING_AFTER_KINGSIDE_CASTLE_POSITION;
			}
			else // black
			{
				rookOldSquare = _board.Squares[Rook.BLACK_RIGHT_ROOK_START_POSITION.x][Rook.BLACK_RIGHT_ROOK_START_POSITION.y];
				rookNewSquare = _board.Squares[Rook.BLACK_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.x][Rook.BLACK_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.y];
				newKingPosition = King.BLACK_KING_AFTER_KINGSIDE_CASTLE_POSITION;
			}
		}
		else // queenside castle
		{
			positionModifier = Vector2Int.left;
			if (king.Color == ColorType.White)
			{
				rookOldSquare = _board.Squares[Rook.WHITE_LEFT_ROOK_START_POSITION.x][Rook.WHITE_LEFT_ROOK_START_POSITION.y];
				rookNewSquare = _board.Squares[Rook.WHITE_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.x][Rook.WHITE_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.y];
				newKingPosition = King.WHITE_KING_AFTER_QUEENSIDE_CASTLE_POSITION;
			}
			else // black
			{
				rookOldSquare = _board.Squares[Rook.BLACK_LEFT_ROOK_START_POSITION.x][Rook.BLACK_LEFT_ROOK_START_POSITION.y];
				rookNewSquare = _board.Squares[Rook.BLACK_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.x][Rook.BLACK_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.y];
				newKingPosition = King.BLACK_KING_AFTER_QUEENSIDE_CASTLE_POSITION;
			}
		}

		if (rookOldSquare.Piece == null || !(rookOldSquare.Piece.Type == PieceType.Rook) || rookOldSquare.Piece.Color != king.Color) // rook missing or wrong color
			return;

		if (!kingsideCastle && _board.Squares[1][king.Square.Position.y].IsOccupied())
			return;

		Vector2Int checkedPosition = king.Square.Position + positionModifier;
		for (int i = 0; i < 2; i++) // squares king has to move over
		{
			Square checkedSuare = _board.Squares[checkedPosition.x][checkedPosition.y];

			if (checkedSuare.IsAttackedBy(king.Color == ColorType.White ? ColorType.Black : ColorType.White) || checkedSuare.IsOccupied()) // under attack or occupied
				return;

			checkedPosition += positionModifier;
		}

		if (king.Square.IsAttackedBy(king.Color == ColorType.White ? ColorType.Black : ColorType.White))
			return;

		Square newKingSquare = _board.Squares[newKingPosition.x][newKingPosition.y];
		_legalMoves.Add(new Move(king, king.Square, newKingSquare, null, MoveType.Castle, rookOldSquare, rookNewSquare));
	}

	bool SaveMoveIfLegal(Move pseudoLegalMove)
	{
		_moveExecutor.MakeMove(pseudoLegalMove);

		bool isKingChecked = pseudoLegalMove.Piece.Pieces.King.IsChecked();

		_moveExecutor.UndoMove(pseudoLegalMove);

		if (!isKingChecked)
		{
			_legalMoves.Add(pseudoLegalMove);
			return true;
		}

		return false;
	}
}
