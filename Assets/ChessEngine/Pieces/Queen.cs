using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;

public class Queen : SlidingPiece
{
    public override PieceType Type => PieceType.Queen;

	public static int VALUE => 900;
	public override int Value { get => VALUE; }

	public readonly static int[,] POSITION_VALUES = {
			{ -20,-10,-10, -5, -5,-10,-10,-20 },
			{ -10,  0,  0,  0,  0,  0,  0,-10 },
			{ -10,  0,  5,  5,  5,  5,  0,-10 },
			{  -5,  0,  5,  5,  5,  5,  0, -5 },
			{	0,  0,  5,  5,  5,  5,  0, -5 },
			{ -10,  5,  5,  5,  5,  5,  0,-10 },
			{ -10,  0,  5,  0,  0,  0,  0,-10 },
			{ -20,-10,-10, -5, -5,-10,-10,-20 }
	};
	public override int[,] PositionsValues => POSITION_VALUES;

	public Queen(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public override void GenerateLegalMoves()
	{
		FindSlidingMoves(new Vector2Int(0, 1));
		FindSlidingMoves(new Vector2Int(1, 1));
		FindSlidingMoves(new Vector2Int(1, 0));
		FindSlidingMoves(new Vector2Int(1, -1));
		FindSlidingMoves(new Vector2Int(0, -1));
		FindSlidingMoves(new Vector2Int(-1, -1));
		FindSlidingMoves(new Vector2Int(-1, 0));
		FindSlidingMoves(new Vector2Int(-1, 1));
	}
}
