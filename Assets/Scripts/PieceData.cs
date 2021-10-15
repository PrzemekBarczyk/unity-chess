using UnityEngine;

public struct PieceData
{
	public ColorType Color { get; private set; }
	public PieceType Type { get; private set; }
	public Vector2 Position { get; private set; }

	public PieceData(ColorType color, PieceType type, Vector2 position)
	{
		Color = color;
		Type = type;
		Position = position;
	}
}
