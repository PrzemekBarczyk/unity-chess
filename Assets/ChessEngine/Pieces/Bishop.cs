using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;

public class Bishop : SlidingPiece
{
    public override PieceType Type => PieceType.Bishop;

	public static int VALUE => 300;
	public override int Value { get => VALUE; }

	public readonly static int[,] POSITION_VALUES = {
			{ -20,-10,-10,-10,-10,-10,-10, -20 },
			{ -10,  0,  0,  0,  0,  0,  0, -10 },
			{ -10,  0,  5, 10, 10,  5,  0, -10 },
			{ -10,  5,  5, 10, 10,  5,  5, -10 },
			{ -10,  0, 10, 10, 10, 10,  0, -10 },
			{ -10, 10, 10, 10, 10, 10, 10, -10 },
			{ -10,  5,  0,  0,  0,  0,  5, -10 },
			{ -20,-10,-10,-10,-10,-10,-10, -20 }
	};
	public override int[,] PositionsValues => POSITION_VALUES;

	public Bishop(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public override List<Move> GenerateLegalMoves()
	{
		_legalMoves.Clear();

		FindSlidingMoves(new Vector2Int(1, 1));
		FindSlidingMoves(new Vector2Int(1, -1));
		FindSlidingMoves(new Vector2Int(-1, -1));
		FindSlidingMoves(new Vector2Int(-1, 1));

		return _legalMoves;
	}
}
