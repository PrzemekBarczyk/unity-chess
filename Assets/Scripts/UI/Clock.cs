using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Clock : MonoBehaviour
{
	public event Action OnTimeElapsed;

	Text _timeLeftText;

	bool _working;

	float _timeLeftInSeconds;
	float _timeAddedAfterMove;

	void Awake()
	{
		_timeLeftText = GetComponent<Text>();
	}

	public void SetUp(bool useClock, float timeForPlayer, float timeAddedAfterMove)
	{
		if (useClock)
		{
			_timeLeftInSeconds = timeForPlayer;
			_timeAddedAfterMove = timeAddedAfterMove;
			UpdateGraphicalClock();
		}
	}

	void UpdateGraphicalClock()
	{
		TimeSpan time = new TimeSpan(0, 0, 0, Mathf.CeilToInt(_timeLeftInSeconds), 0);
		_timeLeftText.text = String.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
	}

	public void Run()
	{
		_working = true;
	}

	public void Stop()
	{
		_working = false;
		_timeLeftInSeconds += _timeAddedAfterMove;
		UpdateGraphicalClock();
	}


	void Update()
	{
		if (_working)
		{
			_timeLeftInSeconds -= Time.deltaTime;
			UpdateGraphicalClock();

			if (_timeLeftInSeconds <= 0)
			{
				OnTimeElapsed?.Invoke();
			}
		}
	}
}
