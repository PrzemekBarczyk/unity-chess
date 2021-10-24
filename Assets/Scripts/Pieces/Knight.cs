using UnityEngine;

public class Knight : JumpingPiece
{
    public override PieceType Type => PieceType.Knight;

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
