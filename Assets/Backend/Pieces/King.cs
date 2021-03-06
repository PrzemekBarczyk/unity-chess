using Vector2Int = UnityEngine.Vector2Int;

namespace Backend
{
	internal static class King
	{
		internal static readonly Vector2Int WHITE_KING_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(6, Board.BOTTOM_RANK_INDEX);
		internal static readonly Vector2Int WHITE_KING_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(2, Board.BOTTOM_RANK_INDEX);
		internal static readonly Vector2Int BLACK_KING_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(6, Board.TOP_RANK_INDEX);
		internal static readonly Vector2Int BLACK_KING_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(2, Board.TOP_RANK_INDEX);
	}
}
