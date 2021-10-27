using UnityEngine;

public class Queen : SlidingPiece
{
    public override PieceType Type => PieceType.Queen;

	public static int VALUE => 900;
	public override int Value { get => VALUE; }

	int[,] _positionsValues = {
			{ -20,-10,-10, -5, -5,-10,-10,-20 },
			{ -10,  0,  0,  0,  0,  0,  0,-10 },
			{ -10,  0,  5,  5,  5,  5,  0,-10 },
			{  -5,  0,  5,  5,  5,  5,  0, -5 },
			{	0,  0,  5,  5,  5,  5,  0, -5 },
			{ -10,  5,  5,  5,  5,  5,  0,-10 },
			{ -10,  0,  5,  0,  0,  0,  0,-10 },
			{ -20,-10,-10, -5, -5,-10,-10,-20 }
	};
	public override int[,] PositionsValues => _positionsValues;

	new void Awake()
	{
		base.Awake();
	}

	public override void GenerateLegalMoves()
	{
		LegalMoves.Clear();

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
