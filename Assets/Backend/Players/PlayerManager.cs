namespace Backend
{
	internal class PlayerManager
	{
		internal Player WhitePlayer { get; private set; }
		internal Player BlackPlayer { get; private set; }

		internal Player CurrentPlayer { get; private set; }
		internal Player NextPlayer { get; private set; }

		internal PlayerManager(GameType gameType, ColorType startingPlayerColor)
		{
			if (gameType == GameType.BotVsBot)
			{
				WhitePlayer = new Player(ColorType.White, PlayerType.Bot);
				BlackPlayer = new Player(ColorType.Black, PlayerType.Bot);
			}
			else if (gameType == GameType.HumanVsHuman)
			{
				WhitePlayer = new Player(ColorType.White, PlayerType.Human);
				BlackPlayer = new Player(ColorType.Black, PlayerType.Human);
			}
			else if (gameType == GameType.HumanVsBot)
			{
				WhitePlayer = new Player(ColorType.White, PlayerType.Human);
				BlackPlayer = new Player(ColorType.Black, PlayerType.Bot);
			}
			else
			{
				WhitePlayer = new Player(ColorType.White, PlayerType.Bot);
				BlackPlayer = new Player(ColorType.Black, PlayerType.Human);
			}

			CurrentPlayer = startingPlayerColor == WhitePlayer.Color ? WhitePlayer : BlackPlayer;
			NextPlayer = startingPlayerColor == WhitePlayer.Color ? BlackPlayer : WhitePlayer;
		}

		internal void SwitchTurn()
		{
			CurrentPlayer = CurrentPlayer.Color == ColorType.White ? BlackPlayer : WhitePlayer;
			NextPlayer = NextPlayer.Color == ColorType.White ? BlackPlayer : WhitePlayer;
		}
	}
}
