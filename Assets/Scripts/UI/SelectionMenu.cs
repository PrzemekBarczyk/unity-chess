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

		GameSettings _gameSettings;

		GameManager _gameManager;

		void Start()
		{
			_gameManager = GameManager.Instance;
		}

		public void HandlePlayButton()
		{
			SaveSettings();
			if (_gameSettings.GameType == GameType.BotVsHuman)
				CameraController.Instance.FlipPOV();
			ChangeMenu();
			StartGame();
		}

		void SaveSettings()
		{
			_gameSettings = new GameSettings();

			_gameSettings.StartPositionInFEN = _fenInputField.text;

			if (_whitePlayerDropdown.options[_whitePlayerDropdown.value].text == "Human" && _blackPlayerDropdown.options[_blackPlayerDropdown.value].text == "Human")
			{
				_gameSettings.GameType = GameType.HumanVsHuman;
			}
			else if (_whitePlayerDropdown.options[_whitePlayerDropdown.value].text == "Bot" && _blackPlayerDropdown.options[_blackPlayerDropdown.value].text == "Bot")
			{
				_gameSettings.GameType = GameType.BotVsBot;
			}
			else if (_whitePlayerDropdown.options[_whitePlayerDropdown.value].text == "Human" && _blackPlayerDropdown.options[_blackPlayerDropdown.value].text == "Bot")
			{
				_gameSettings.GameType = GameType.HumanVsBot;
			}
			else
			{
				_gameSettings.GameType = GameType.BotVsHuman;
			}

			_gameSettings.UseClocks = _useClockToggle.isOn;

			_gameSettings.BaseTime = uint.Parse(_baseTimeInputField.text);

			_gameSettings.AddedTime = uint.Parse(_addedTimeInputField.text);
		}

		void ChangeMenu()
		{
			_hud.SetUp(_gameSettings);
			_selectionMenu.SetActive(false);
			_hud.gameObject.SetActive(true);
		}

		void StartGame()
		{
			_gameManager.StartGame(_gameSettings);
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
