using UnityEngine;

public class PlayersSelector : MonoBehaviour
{
    [SerializeField] GameObject _selectOpponentMenu;
    [SerializeField] GameObject _selectColorMenu;

	PlayerType _mainPlayerType;
	PlayerType _secondPlayerType;

	ColorType _mainPlayerColor;
	ColorType _secondPlayerColor;

	PlayerManager _playerManager;

	void Awake()
	{
		_playerManager = FindObjectOfType<PlayerManager>();
	}

	public void BotVsBot()
	{
		_mainPlayerType = PlayerType.Bot;
		_secondPlayerType = PlayerType.Bot;
		_selectOpponentMenu.SetActive(false);
		_selectColorMenu.SetActive(true);
	}

    public void PlayBot()
	{
		_mainPlayerType = PlayerType.Human;
		_secondPlayerType = PlayerType.Bot;
		_selectOpponentMenu.SetActive(false);
		_selectColorMenu.SetActive(true);
	}

	public void PlayHuman()
	{
		_mainPlayerType = PlayerType.Human;
		_secondPlayerType = PlayerType.Human;
		_selectOpponentMenu.SetActive(false);
		_selectColorMenu.SetActive(true);
	}

	public void SelectBlack()
	{
		_mainPlayerColor = ColorType.Black;
		_secondPlayerColor = ColorType.White;
		_selectColorMenu.SetActive(false);
		_playerManager.CreatePlayers(new PlayerData(_mainPlayerColor, _mainPlayerType), new PlayerData(_secondPlayerColor, _secondPlayerType));
	}

	public void SelectWhite()
	{
		_mainPlayerColor = ColorType.White;
		_secondPlayerColor = ColorType.Black;
		_selectColorMenu.SetActive(false);
		_playerManager.CreatePlayers(new PlayerData(_mainPlayerColor, _mainPlayerType), new PlayerData(_secondPlayerColor, _secondPlayerType));
	}
}
