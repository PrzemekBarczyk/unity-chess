using UnityEngine;

public class Rook : SlidingPiece
{
	public override PieceType Type => PieceType.Rook;

	public static int VALUE => 500;
	public override int Value { get => VALUE; }

	int[,] _positionsValues = {
			{  0,  0,  0,  0,  0,  0,  0,  0 },
			{  5, 10, 10, 10, 10, 10, 10,  5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{  0,  0,  0,  5,  5,  0,  0,  0 }
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
		FindSlidingMoves(new Vector2Int(1, 0));
		FindSlidingMoves(new Vector2Int(0, -1));
		FindSlidingMoves(new Vector2Int(-1, 0));
	}

	public override void Move(MoveData moveToMake, bool updateGraphic = false)
	{
		base.Move(moveToMake, updateGraphic);

		if (Square.Position.x == Board.LEFT_FILE && Square.Position.y == Board.BOTTOM_RANK)
			Pieces.King.CanCastleQueenside = false;
		else if (Square.Position.x == Board.RIGHT_FILE && Square.Position.y == Board.BOTTOM_RANK)
			Pieces.King.CanCastleKingside = false;
	}
}
