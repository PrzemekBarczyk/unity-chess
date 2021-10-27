using UnityEngine;

public class Knight : JumpingPiece
{
    public override PieceType Type => PieceType.Knight;

	public static int VALUE => 300;
	public override int Value { get => VALUE; }

	int[,] _positionsValues = {
			{ -50,-40,-30,-30,-30,-30,-40,-50 },
			{ -40,-20,  0,  0,  0,  0,-20,-40 },
			{ -30,  0, 10, 15, 15, 10,  0,-30 },
			{ -30,  5, 15, 20, 20, 15,  5,-30 },
			{ -30,  0, 15, 20, 20, 15,  0,-30 },
			{ -30,  5, 10, 15, 15, 10,  5,-30 },
			{ -40,-20,  0,  5,  5,  0,-20,-40 },
			{ -50,-40,-30,-30,-30,-30,-40,-50 }
	};
	public override int[,] PositionsValues => _positionsValues;

	new void Awake()
	{
		base.Awake();
	}

	public override void GenerateLegalMoves()
	{
		LegalMoves.Clear();

		FindJumpMove(new Vector2Int(-1, 2));
		FindJumpMove(new Vector2Int(1, 2));
		FindJumpMove(new Vector2Int(2, 1));
		FindJumpMove(new Vector2Int(2, -1));
		FindJumpMove(new Vector2Int(1, -2));
		FindJumpMove(new Vector2Int(-1, -2));
		FindJumpMove(new Vector2Int(-2, -1));
		FindJumpMove(new Vector2Int(-2, 1));
	}
}
