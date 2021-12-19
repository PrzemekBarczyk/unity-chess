public enum PlayerType { Undefinied, Human, Bot }

public class Player
{
	public ColorType Color { get; private set; }
	public PlayerType Type { get; }

	public Player(ColorType color, PlayerType type)
	{
		Color = color;
		Type = type;
	}
}
