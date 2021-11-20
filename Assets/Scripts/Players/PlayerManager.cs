using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [SerializeField] Player _humanPlayerPrefab;
    [SerializeField] Player _botPlayerPrefab;

	ColorType _startingPlayerColor;
	float _timeForPlayer;
	float _timeAddedAfterMove;

	public Player WhitePlayer { get; private set; }
	public Player BlackPlayer { get; private set; }

	public Player CurrentPlayer { get; private set; }
	public Player NextPlayer { get; private set; }

	new void Awake()
	{
		base.Awake();
	}

	public void SaveStartingPlayerColor(ColorType startingPlayerColor)
	{
		_startingPlayerColor = startingPlayerColor;
	}

	public void SaveClockData(float timeForPlayer, float timeAddedAfterMove)
	{
		_timeForPlayer = timeForPlayer;
		_timeAddedAfterMove = timeAddedAfterMove;
	}

	public void CreatePlayers(PlayerData mainPlayer, PlayerData secondPlayer)
	{
		if (mainPlayer.Color == ColorType.White)
		{
			WhitePlayer = Instantiate(mainPlayer.Type == PlayerType.Human ? _humanPlayerPrefab : _botPlayerPrefab, transform.position, transform.rotation, transform);
			BlackPlayer = Instantiate(secondPlayer.Type == PlayerType.Human ? _humanPlayerPrefab : _botPlayerPrefab, transform.position, transform.rotation, transform);
		}
		else if (mainPlayer.Color == ColorType.Black)
		{
			WhitePlayer = Instantiate(secondPlayer.Type == PlayerType.Human ? _humanPlayerPrefab : _botPlayerPrefab, transform.position, transform.rotation, transform);
			BlackPlayer = Instantiate(mainPlayer.Type == PlayerType.Human ? _humanPlayerPrefab : _botPlayerPrefab, transform.position, transform.rotation, transform);
		}

		WhitePlayer.name = WhitePlayer.name.Replace("(Clone)", "");
		BlackPlayer.name = BlackPlayer.name.Replace("(Clone)", "");

		WhitePlayer.Initialize(ColorType.White, _timeForPlayer, _timeAddedAfterMove);
		BlackPlayer.Initialize(ColorType.Black, _timeForPlayer, _timeAddedAfterMove);

		CurrentPlayer = _startingPlayerColor == WhitePlayer.Color ? WhitePlayer : BlackPlayer;
		NextPlayer = _startingPlayerColor == WhitePlayer.Color ? BlackPlayer : WhitePlayer;
	}

	public void SwitchTurn()
	{
		CurrentPlayer = CurrentPlayer.Color == ColorType.White ? BlackPlayer : WhitePlayer;
		NextPlayer = NextPlayer.Color == ColorType.White ? BlackPlayer : WhitePlayer;
	}
}
