public struct PlayerData
{
    public ColorType Color { get; private set; }
    public PlayerType Type { get; private set; }

    public PlayerData(ColorType color, PlayerType type)
	{
        Color = color;
        Type = type;
	}
}
