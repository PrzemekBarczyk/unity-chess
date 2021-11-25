using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
public class Clock : MonoBehaviour
{
	Text _graphicalClock;

	ColorType _playerColor;

	float _timeLeftInSeconds;
	float _timeAddedAfterMove;

	bool _useClock;

	bool _isCounting;

	int _counter;

	GameManager _gameManager;

	void Start()
	{
		_gameManager = GameManager.Instance;
	}

	public void SetUp(ColorType playerColor, bool useClock, float timeForPlayer, float timeAddedAfterMove)
	{
		_playerColor = playerColor;
		_useClock = useClock;
		_timeAddedAfterMove = timeAddedAfterMove;
		_timeLeftInSeconds = timeForPlayer;

		if (_playerColor == ColorType.White)
		{
			_graphicalClock = GameObject.Find("White Clock").GetComponent<Text>();
		}
		else
		{
			_graphicalClock = GameObject.Find("Black Clock").GetComponent<Text>();
		}

		if (useClock)
		{
			UpdateGraphicalClock();
		}
	}

	void UpdateGraphicalClock()
	{
		TimeSpan time = new TimeSpan(0, 0, 0, Mathf.CeilToInt(_timeLeftInSeconds), 0);
		_graphicalClock.text = String.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
	}

	public void Run()
	{
		_isCounting = true;
	}

	public void Stop()
	{
		_isCounting = false;
		_timeLeftInSeconds += _timeAddedAfterMove;
	}

	void Update()
	{
		if (_useClock)
		{
			if (_isCounting)
			{
				_counter = 0;

				_timeLeftInSeconds -= Time.deltaTime;

				UpdateGraphicalClock();

				if (_timeLeftInSeconds <= 0)
					_gameManager.TimeElapsed();
			}
			else if (_counter == 0)
			{
				_counter++;
				UpdateGraphicalClock();
			}
		}
	}
}
