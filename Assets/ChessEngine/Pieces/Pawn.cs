using System;
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
	public bool OnPositionValidForEnPassant => Square.Position.y == (Color == ColorType.White ? 4 : 3);

	public int DirectionModifier => Color == ColorType.White ? 1 : -1;

	public Pawn(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public override void GenerateLegalMoves()
	{
		if (OnStartingPosition) FindSlidingMoves(new Vector2Int(0, 1 * DirectionModifier), 2, canAttack: false, canMoveOnEmptySquare: true);
		else FindSlidingMoves(new Vector2Int(0, 1 * DirectionModifier), 1, canAttack: false, canMoveOnEmptySquare: true, canPromote: true);

		FindSlidingMoves(new Vector2Int(-1, 1 * DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);
		FindSlidingMoves(new Vector2Int(1, 1 * DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);

		if (OnPositionValidForEnPassant) FindEnPassantMoves(false);
		if (OnPositionValidForEnPassant) FindEnPassantMoves(true);
	}

	void FindEnPassantMoves(bool rightEnPassant)
	{
		Square newSquare;
		if (rightEnPassant)
		{
			if (Square.Position.x + 1 > Board.RIGHT_FILE_INDEX)
				return;
			newSquare = _board.Squares[Square.Position.x + 1][Square.Position.y + DirectionModifier];
		}
		else
		{
			if (Square.Position.x - 1 < Board.LEFT_FILE_INDEX)
				return;
			newSquare = _board.Squares[Square.Position.x - 1][Square.Position.y + DirectionModifier];
		}

		if (newSquare == _board.EnPassantTarget)
		{
			Piece encounteredPiece = _board.Squares[newSquare.Position.x][Square.Position.y].Piece;
			Move move = new Move(this, Square, newSquare, encounteredPiece);

			if (SaveMoveIfLegal(move))
				return;
		}
	}

	public override void Move(Move moveToMake)
	{
		base.Move(moveToMake);

		bool madeDoubleMove = moveToMake.NewSquare.Position.y == (Color == ColorType.White ? 3 : 4) && moveToMake.OldSquare.Position.y == (Color == ColorType.White ? 1 : 6);
		if (madeDoubleMove)
		{
			_board.EnPassantTarget = _board.Squares[Square.Position.x][Square.Position.y - DirectionModifier];
		}
		else if (moveToMake.IsPromotion)
		{
			Piece promotion;
			switch (moveToMake.Type)
			{
				case MoveType.PromotionToKnight:
					promotion = new Knight(_board, Pieces, Color, Square.Position);
					break;
				case MoveType.PromotionToBishop:
					promotion = new Bishop(_board, Pieces, Color, Square.Position);
					break;
				case MoveType.PromotionToRook:
					promotion = new Rook(_board, Pieces, Color, Square.Position);
					break;
				case MoveType.PromotionToQueen:
					promotion = new Queen(_board, Pieces, Color, Square.Position);
					break;
				default:
					throw new Exception();
			}
			Pieces.AllPieces.Add(promotion);
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
