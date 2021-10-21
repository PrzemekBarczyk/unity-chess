using UnityEngine;

public class Knight : Piece
{
    public override PieceType Type => PieceType.Knight;

	new void Awake()
	{
		base.Awake();
	}

	public override void FindLegalMoves()
	{
		ClearLegalMoves();

		AddValidJumpMove(new Vector2Int(-1, 2));
		AddValidJumpMove(new Vector2Int(1, 2));
		AddValidJumpMove(new Vector2Int(2, 1));
		AddValidJumpMove(new Vector2Int(2, -1));
		AddValidJumpMove(new Vector2Int(1, -2));
		AddValidJumpMove(new Vector2Int(-1, -2));
		AddValidJumpMove(new Vector2Int(-2, -1));
		AddValidJumpMove(new Vector2Int(-2, 1));
	}

	void AddValidJumpMove(Vector2Int offset)
	{
		Vector2Int checkedPosition = Square.Position + offset;

		TryAddSingleMove(checkedPosition);
	}
}
