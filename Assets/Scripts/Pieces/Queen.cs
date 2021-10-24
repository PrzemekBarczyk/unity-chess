using UnityEngine;

public class Queen : SlidingPiece
{
    public override PieceType Type => PieceType.Queen;

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
