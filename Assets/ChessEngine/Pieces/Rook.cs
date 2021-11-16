using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;

public class Rook : SlidingPiece
{
	public override PieceType Type => PieceType.Rook;

	public static int VALUE => 500;
	public override int Value { get => VALUE; }

	public readonly static int[,] POSITION_VALUES = {
			{  0,  0,  0,  0,  0,  0,  0,  0 },
			{  5, 10, 10, 10, 10, 10, 10,  5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{  0,  0,  0,  5,  5,  0,  0,  0 }
	};
	public override int[,] PositionsValues => POSITION_VALUES;

	public Rook(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public override void GenerateLegalMoves()
	{
		FindSlidingMoves(new Vector2Int(0, 1));
		FindSlidingMoves(new Vector2Int(1, 0));
		FindSlidingMoves(new Vector2Int(0, -1));
		FindSlidingMoves(new Vector2Int(-1, 0));
	}

	public override void Move(Move moveToMake)
	{
		base.Move(moveToMake);

		if (Square.Position.x == Board.LEFT_FILE_INDEX && Square.Position.y == Board.BOTTOM_RANK_INDEX)
			Pieces.King.CanCastleQueenside = false;
		else if (Square.Position.x == Board.RIGHT_FILE_INDEX && Square.Position.y == Board.BOTTOM_RANK_INDEX)
			Pieces.King.CanCastleKingside = false;
	}
}
