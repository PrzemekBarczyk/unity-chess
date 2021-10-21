using UnityEngine;

public class Rook : Piece
{
    public override PieceType Type => PieceType.Rook;

	new void Awake()
	{
		base.Awake();

	}

	public override void FindLegalMoves()
	{
		ClearLegalMoves();

		AddSlidingMoves(new Vector2Int(0, 1));
		AddSlidingMoves(new Vector2Int(1, 0));
		AddSlidingMoves(new Vector2Int(0, -1));
		AddSlidingMoves(new Vector2Int(-1, 0));
	}

	public override void Move(MoveData moveToMake, bool updateGraphic = false)
	{
		if (Square.Position.x == Board.LEFT_FILE && Square.Position.y == Board.BOTTOM_RANK)
			Pieces.King.CanCastleQueenside = false;
		else if (Square.Position.x == Board.RIGHT_FILE && Square.Position.y == Board.BOTTOM_RANK)
			Pieces.King.CanCastleKingside = false;

		base.Move(moveToMake, updateGraphic);
	}
}
