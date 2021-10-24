using UnityEngine;

public class Bishop : SlidingPiece
{
    public override PieceType Type => PieceType.Bishop;

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
