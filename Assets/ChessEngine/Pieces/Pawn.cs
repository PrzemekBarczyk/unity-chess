using System;
using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;

public class Pawn : SlidingPiece
{
    public override PieceType Type => PieceType.Pawn;

	public static int VALUE => 100;
	public override int Value => VALUE;

	public readonly static int[,] POSITION_VALUES = {
			{ 0,   0,  0,  0,  0,  0,  0,  0 },
			{ 50, 50, 50, 50, 50, 50, 50, 50 },
			{ 10, 10, 20, 30, 30, 20, 10, 10 },
			{ 5,   5, 10, 25, 25, 10,  5,  5 },
			{ 0,   0,  0, 20, 20,  0,  0,  0 },
			{ 5,  -5,-10,  0,  0,-10, -5,  5 },
			{ 5,  10, 10,-20,-20, 10, 10,  5 },
			{ 0,   0,  0,  0,  0,  0,  0,  0 }
	};
	public override int[,] PositionsValues => POSITION_VALUES;

	public bool OnStartingPosition => Square.Position.y == (Color == ColorType.White ? 1 : 6);

	public int DirectionModifier => Color == ColorType.White ? 1 : -1;

	public Pawn(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public override List<Move> GenerateLegalMoves()
	{
		_legalMoves.Clear();

		if (OnStartingPosition) FindSlidingMoves(new Vector2Int(0, 1 * DirectionModifier), 2, canAttack: false, canMoveOnEmptySquare: true);
		else FindSlidingMoves(new Vector2Int(0, 1 * DirectionModifier), 1, canAttack: false, canMoveOnEmptySquare: true, canPromote: true);

		FindSlidingMoves(new Vector2Int(-1, 1 * DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);
		FindSlidingMoves(new Vector2Int(1, 1 * DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);

		FindEnPassantMoves();

		return _legalMoves;
	}

	void FindEnPassantMoves()
	{
		int enPassantRank = Color == ColorType.White ? 4 : 3;

		if (Square.Position.y != enPassantRank)
			return;

		Square squareOnLeft = null, squareOnRight = null;
		try
		{
			squareOnLeft = _board.Squares[Square.Position.x - 1][Square.Position.y];
		}
		catch (IndexOutOfRangeException) { } // do nothing
		try
		{
			squareOnRight = _board.Squares[Square.Position.x + 1][Square.Position.y];
		}
		catch (IndexOutOfRangeException) { } // do nothing

		foreach (Square sideSquare in new Square[] { squareOnLeft, squareOnRight })
		{
			if (sideSquare == null) // square outside the board
				continue;

			Square newSquare = _board.Squares[sideSquare.Position.x][sideSquare.Position.y + DirectionModifier];
			if (newSquare == _board.EnPassantTarget)
			{
				Move move = new Move(this, Square, newSquare, sideSquare.Piece);

				if (SaveMoveIfLegal(move))
				{
					return;
				}
			}
		}
	}

	public override void Move(Move moveToMake)
	{
		base.Move(moveToMake);

		if (moveToMake.OldSquare.Position.y == (Color == ColorType.White ? 1 : 6) && moveToMake.NewSquare.Position.y == (Color == ColorType.White ? 3 : 4))
		{
			_board.EnPassantTarget = _board.Squares[Square.Position.x][Square.Position.y - DirectionModifier];
		}
		else if (moveToMake.IsPromotion)
		{
			Piece newPiece;
			switch (moveToMake.Type)
			{
				case MoveType.PromotionToKnight:
					newPiece = new Knight(_board, Pieces, Color, Square.Position);
					break;
				case MoveType.PromotionToBishop:
					newPiece = new Bishop(_board, Pieces, Color, Square.Position);
					break;
				case MoveType.PromotionToRook:
					newPiece = new Rook(_board, Pieces, Color, Square.Position);
					break;
				case MoveType.PromotionToQueen:
					newPiece = new Queen(_board, Pieces, Color, Square.Position);
					break;
				default:
					throw new Exception();
			}
			Pieces.AllPieces.Add(newPiece);
			Pieces.AllPieces.Remove(this);
		}
	}

	public override void UndoMove(Move moveToUndo)
	{
		if (moveToUndo.IsPromotion)
		{
			Pieces.AllPieces.Remove(Square.Piece);
			Square.Piece = this;
			Pieces.AllPieces.Add(this);
		}

		base.UndoMove(moveToUndo);
	}
}
