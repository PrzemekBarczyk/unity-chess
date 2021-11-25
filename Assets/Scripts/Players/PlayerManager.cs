using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [SerializeField] Player _humanPlayerPrefab;
    [SerializeField] Player _botPlayerPrefab;

	public Player WhitePlayer { get; private set; }
	public Player BlackPlayer { get; private set; }

	public Player CurrentPlayer { get; private set; }
	public Player NextPlayer { get; private set; }

	public void CreatePlayers(GameType gameType, bool useClocks, uint baseTime, uint addedTime)
	{
		if (gameType == GameType.BotVsBot)
		{
			WhitePlayer = Instantiate(_botPlayerPrefab, transform.position, transform.rotation, transform);
			BlackPlayer = Instantiate(_botPlayerPrefab, transform.position, transform.rotation, transform);
		}
		else if (gameType == GameType.HumanVsHuman)
		{
			WhitePlayer = Instantiate(_humanPlayerPrefab, transform.position, transform.rotation, transform);
			BlackPlayer = Instantiate(_humanPlayerPrefab, transform.position, transform.rotation, transform);
		}
		else if (gameType == GameType.HumanVsBot)
		{
			WhitePlayer = Instantiate(_humanPlayerPrefab, transform.position, transform.rotation, transform);
			BlackPlayer = Instantiate(_botPlayerPrefab, transform.position, transform.rotation, transform);
		}
		else
		{
			WhitePlayer = Instantiate(_botPlayerPrefab, transform.position, transform.rotation, transform);
			BlackPlayer = Instantiate(_humanPlayerPrefab, transform.position, transform.rotation, transform);
		}

		WhitePlayer.SetColor(ColorType.White);
		BlackPlayer.SetColor(ColorType.Black);

		WhitePlayer.SetClock(useClocks, baseTime, addedTime);
		BlackPlayer.SetClock(useClocks, baseTime, addedTime);

		WhitePlayer.name = WhitePlayer.name.Replace("(Clone)", "");
		BlackPlayer.name = BlackPlayer.name.Replace("(Clone)", "");
	}

	public void SetStartingPlayer(ColorType startingPlayerColor)
	{
		CurrentPlayer = startingPlayerColor == WhitePlayer.Color ? WhitePlayer : BlackPlayer;
		NextPlayer = startingPlayerColor == WhitePlayer.Color ? BlackPlayer : WhitePlayer;
	}

	public void SwitchTurn()
	{
		CurrentPlayer = CurrentPlayer.Color == ColorType.White ? BlackPlayer : WhitePlayer;
		NextPlayer = NextPlayer.Color == ColorType.White ? BlackPlayer : WhitePlayer;
	}
}
