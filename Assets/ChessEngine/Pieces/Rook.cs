using Vector2Int = UnityEngine.Vector2Int;

public static class Rook
{
	public static readonly Vector2Int WHITE_LEFT_ROOK_START_POSITION = new Vector2Int(Board.LEFT_FILE_INDEX, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int WHITE_RIGHT_ROOK_START_POSITION = new Vector2Int(Board.RIGHT_FILE_INDEX, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int BLACK_LEFT_ROOK_START_POSITION = new Vector2Int(Board.LEFT_FILE_INDEX, Board.TOP_RANK_INDEX);
	public static readonly Vector2Int BLACK_RIGHT_ROOK_START_POSITION = new Vector2Int(Board.RIGHT_FILE_INDEX, Board.TOP_RANK_INDEX);

	public static readonly Vector2Int WHITE_ROOK_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(5, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int WHITE_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(3, Board.BOTTOM_RANK_INDEX);
	public static readonly Vector2Int BLACK_ROOK_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(5, Board.TOP_RANK_INDEX);
	public static readonly Vector2Int BLACK_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(3, Board.TOP_RANK_INDEX);
}
