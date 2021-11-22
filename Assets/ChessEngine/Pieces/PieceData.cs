using Vector2Int = UnityEngine.Vector2Int;

public struct PieceData
{
	public ColorType Color { get; private set; }
	public PieceType Type { get; private set; }
	public Vector2Int Position { get; private set; }

	public PieceData(ColorType color, PieceType type, Vector2Int position)
	{
		Color = color;
		Type = type;
		Position = position;
	}
}