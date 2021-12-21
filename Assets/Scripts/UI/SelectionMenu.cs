using Backend;
using UnityEngine;
using UnityEngine.UI;

namespace Frontend
{
	public class SelectionMenu : MonoBehaviour
	{
		[Header("Selection Menu")]
		[SerializeField] GameObject _selectionMenu;
		[SerializeField] InputField _fenInputField;
		[SerializeField] Dropdown _whitePlayerDropdown;
		[SerializeField] Dropdown _blackPlayerDropdown;
		[SerializeField] Toggle _useClockToggle;
		[SerializeField] InputField _baseTimeInputField;
		[SerializeField] InputField _addedTimeInputField;

		[Header("HUD")]
		[SerializeField] HUD _hud;

		string _startPositionInFEN;
		GameType _gameType;
		bool _useClocks;
		uint _baseTime;
		uint _addedTime;

		GameManager _gameManager;

		void Start()
		{
			_gameManager = GameManager.Instance;
		}

		public void HandlePlayButton()
		{
			SaveSettings();
			AdjustCameraPOV();
			ChangeMenu();
			StartGame();
		}

		void SaveSettings()
		{
			_startPositionInFEN = _fenInputField.text;

			if (_whitePlayerDropdown.options[_whitePlayerDropdown.value].text == "Human" && _blackPlayerDropdown.options[_blackPlayerDropdown.value].text == "Human")
			{
				_gameType = GameType.HumanVsHuman;
			}
			else if (_whitePlayerDropdown.options[_whitePlayerDropdown.value].text == "Bot" && _blackPlayerDropdown.options[_blackPlayerDropdown.value].text == "Bot")
			{
				_gameType = GameType.BotVsBot;
			}
			else if (_whitePlayerDropdown.options[_whitePlayerDropdown.value].text == "Human" && _blackPlayerDropdown.options[_blackPlayerDropdown.value].text == "Bot")
			{
				_gameType = GameType.HumanVsBot;
			}
			else
			{
				_gameType = GameType.BotVsHuman;
			}

			_useClocks = _useClockToggle.isOn;

			_baseTime = uint.Parse(_baseTimeInputField.text);

			_addedTime = uint.Parse(_addedTimeInputField.text);
		}

		void AdjustCameraPOV()
		{
			if (_gameType == GameType.BotVsHuman)
			{
				CameraController.Instance.FlipPOV();
			}
		}

		void ChangeMenu()
		{
			_hud.SetUp(_gameType);

			_selectionMenu.SetActive(false);
			_hud.gameObject.SetActive(true);
		}

		void StartGame()
		{
			_gameManager.StartGame(_startPositionInFEN, _gameType, _useClocks, _baseTime, _addedTime);
		}

		public void HandleExitButton()
		{
			ExitGame();
		}

		void ExitGame()
		{
			Application.Quit();
		}
	}
}
