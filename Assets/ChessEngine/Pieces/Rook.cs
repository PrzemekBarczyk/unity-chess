using Vector2Int = UnityEngine.Vector2Int;

public class Rook : Piece
{
	public override PieceType Type => PieceType.Rook;

	public static int VALUE => 500;
	public override int Value { get => VALUE; }

	public readonly static int[,] POSITION_VALUES = {
			{  0,  0,  0,  0,  0,  0,  0,  0 },
			{  5, 10, 10, 10, 10, 10, 10,  5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{  0,  0,  0,  5,  5,  0,  0,  0 }
	};
	public override int[,] PositionsValues => POSITION_VALUES;

	public static readonly Vector2Int WHITE_LEFT_ROOK_START_POSITION = new Vector2Int(Board.LEFT_FILE_INDEX, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int WHITE_RIGHT_ROOK_START_POSITION = new Vector2Int(Board.RIGHT_FILE_INDEX, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int BLACK_LEFT_ROOK_START_POSITION = new Vector2Int(Board.LEFT_FILE_INDEX, Board.TOP_RANK_INDEX);
	public static readonly Vector2Int BLACK_RIGHT_ROOK_START_POSITION = new Vector2Int(Board.RIGHT_FILE_INDEX, Board.TOP_RANK_INDEX);
	
	public static readonly Vector2Int WHITE_ROOK_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(5, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int WHITE_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(3, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int BLACK_ROOK_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(5, Board.TOP_RANK_INDEX);
	public static readonly Vector2Int BLACK_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(3, Board.TOP_RANK_INDEX);

	public Rook(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public override void Move(Move moveToMake)
	{
		base.Move(moveToMake);

		if (moveToMake.OldSquare.Position.x == Board.LEFT_FILE_INDEX && moveToMake.OldSquare.Position.y == (Color == ColorType.White ? Board.BOTTOM_RANK_INDEX : Board.TOP_RANK_INDEX))
		{
			Pieces.King.CanCastleQueenside = false;
		}
		else if (moveToMake.OldSquare.Position.x == Board.RIGHT_FILE_INDEX && moveToMake.OldSquare.Position.y == (Color == ColorType.White ? Board.BOTTOM_RANK_INDEX : Board.TOP_RANK_INDEX))
		{
			Pieces.King.CanCastleKingside = false;
		}
	}
}
