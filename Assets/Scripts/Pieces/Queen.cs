using UnityEngine;

public class Queen : Piece
{
    public override PieceType Type => PieceType.Queen;

	new void Awake()
	{
		base.Awake();
	}

	public override void FindLegalMoves()
	{
		ClearLegalMoves();

		AddSlidingMoves(new Vector2Int(0, 1));
		AddSlidingMoves(new Vector2Int(1, 1));
		AddSlidingMoves(new Vector2Int(1, 0));
		AddSlidingMoves(new Vector2Int(1, -1));
		AddSlidingMoves(new Vector2Int(0, -1));
		AddSlidingMoves(new Vector2Int(-1, -1));
		AddSlidingMoves(new Vector2Int(-1, 0));
		AddSlidingMoves(new Vector2Int(-1, 1));
	}
}
