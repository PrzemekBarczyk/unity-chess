using UnityEngine;
using UnityEngine.UI;

namespace Frontend
{
	[RequireComponent(typeof(InputField))]
	public class InputFieldValidation : MonoBehaviour
	{
		InputField _inputField;

		string _defaultValue;

		void Awake()
		{
			_inputField = GetComponent<InputField>();
			_defaultValue = _inputField.text;
		}

		public void RemoveMinus()
		{
			_inputField.text = _inputField.text.Replace("-", "");
		}

		public void RestoreDefaultIfEqualsZero()
		{
			if (uint.Parse(_inputField.text) == 0)
				_inputField.text = _defaultValue;
		}

		public void RestoreDefaultValueIfEmpty()
		{
			if (_inputField.text.Length == 0)
				_inputField.text = _defaultValue;
		}
	}
}
