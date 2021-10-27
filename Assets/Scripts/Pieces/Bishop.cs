using UnityEngine;

public class Bishop : SlidingPiece
{
    public override PieceType Type => PieceType.Bishop;

	public static int VALUE => 300;
	public override int Value { get => VALUE; }

	int[,] _positionsValues = {
			{ -20,-10,-10,-10,-10,-10,-10, -20 },
			{ -10,  0,  0,  0,  0,  0,  0, -10 },
			{ -10,  0,  5, 10, 10,  5,  0, -10 },
			{ -10,  5,  5, 10, 10,  5,  5, -10 },
			{ -10,  0, 10, 10, 10, 10,  0, -10 },
			{ -10, 10, 10, 10, 10, 10, 10, -10 },
			{ -10,  5,  0,  0,  0,  0,  5, -10 },
			{ -20,-10,-10,-10,-10,-10,-10, -20 }
	};
	public override int[,] PositionsValues => _positionsValues;

	new void Awake()
	{
		base.Awake();
	}

	public override void GenerateLegalMoves()
	{
		LegalMoves.Clear();

		FindSlidingMoves(new Vector2Int(1, 1));
		FindSlidingMoves(new Vector2Int(1, -1));
		FindSlidingMoves(new Vector2Int(-1, -1));
		FindSlidingMoves(new Vector2Int(-1, 1));
	}
}
