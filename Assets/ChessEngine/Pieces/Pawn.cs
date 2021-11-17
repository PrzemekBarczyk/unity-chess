using Vector2Int = UnityEngine.Vector2Int;

public class Pawn : Piece
{
    public override PieceType Type => PieceType.Pawn;

	public static int VALUE => 100;
	public override int Value => VALUE;

	public readonly static int[,] POSITION_VALUES = {
			{ 0,   0,  0,  0,  0,  0,  0,  0 },
			{ 50, 50, 50, 50, 50, 50, 50, 50 },
			{ 10, 10, 20, 30, 30, 20, 10, 10 },
			{ 5,   5, 10, 25, 25, 10,  5,  5 },
			{ 0,   0,  0, 20, 20,  0,  0,  0 },
			{ 5,  -5,-10,  0,  0,-10, -5,  5 },
			{ 5,  10, 10,-20,-20, 10, 10,  5 },
			{ 0,   0,  0,  0,  0,  0,  0,  0 }
	};
	public override int[,] PositionsValues => POSITION_VALUES;

	public bool OnStartingPosition => Square.Position.y == (Color == ColorType.White ? 1 : 6);
	public bool OnPositionValidForEnPassant => Square.Position.y == (Color == ColorType.White ? 4 : 3);

	public int DirectionModifier => Color == ColorType.White ? 1 : -1;

	public Pawn(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }
}
