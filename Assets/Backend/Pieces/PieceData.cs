using Vector2Int = UnityEngine.Vector2Int;

namespace Backend
{
	public struct PieceData
	{
		public ColorType Color { get; private set; }
		public PieceType Type { get; private set; }
		public Vector2Int Position { get; private set; }

		internal PieceData(ColorType color, PieceType type, Vector2Int position)
		{
			Color = color;
			Type = type;
			Position = position;
		}

		internal PieceData(Piece piece)
		{
			Color = piece.Color;
			Type = piece.Type;
			Position = piece.Square.Position;
		}
	}
}
