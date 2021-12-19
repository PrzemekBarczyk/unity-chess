namespace Backend
{
	internal enum PlayerType { Undefinied, Human, Bot }

	internal class Player
	{
		internal ColorType Color { get; private set; }
		internal PlayerType Type { get; }

		internal Player(ColorType color, PlayerType type)
		{
			Color = color;
			Type = type;
		}
	}
}
