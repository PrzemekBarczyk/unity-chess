using System;
using UnityEngine;
using UnityEngine.UI;

namespace Frontend
{
	public class Clocks : MonoBehaviour
	{
		[Header("Clocks on UI")]
		[SerializeField] Text _timeLeftForWhiteText;
		[SerializeField] Text _timeLeftForBlackText;

		public event Action OnTimeElapsed;

		bool _useClocks;

		bool _isWhitePlayerClockWorking;
		bool _isBlackPlayerClockWorking;

		float _timeLeftForWhitePlayer;
		float _timeLeftForBlackPlayer;

		float _timeAddedAfterMove;

		public void SetUp(bool useClock, float timeForPlayer, float timeAddedAfterMove)
		{
			if (useClock)
			{
				_useClocks = true;

				_timeLeftForWhitePlayer = timeForPlayer;
				_timeLeftForBlackPlayer = timeForPlayer;

				_timeAddedAfterMove = timeAddedAfterMove;

				UpdateGraphicalClock(_timeLeftForWhitePlayer, _timeLeftForWhiteText);
				UpdateGraphicalClock(_timeLeftForBlackPlayer, _timeLeftForBlackText);
			}
			else
			{
				_useClocks = false;

				_timeLeftForWhiteText.text = "";
				_timeLeftForBlackText.text = "";
			}
		}

		void UpdateGraphicalClock(float timeLeftForPlayer, Text timeLeftForPlayerText)
		{
			TimeSpan time = new TimeSpan(0, 0, 0, Mathf.CeilToInt(timeLeftForPlayer), 0);
			timeLeftForPlayerText.text = String.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
		}

		public void Run(ColorType color)
		{
			if (_useClocks)
			{
				if (color == ColorType.White)
				{
					_isWhitePlayerClockWorking = true;
				}
				else
				{
					_isBlackPlayerClockWorking = true;
				}
			}
		}

		public void Stop(ColorType color)
		{
			if (_useClocks)
			{
				if (color == ColorType.White)
				{
					_isWhitePlayerClockWorking = false;

					_timeLeftForWhitePlayer += _timeAddedAfterMove;

					UpdateGraphicalClock(_timeLeftForWhitePlayer, _timeLeftForWhiteText);
				}
				else
				{
					_isBlackPlayerClockWorking = false;

					_timeLeftForBlackPlayer += _timeAddedAfterMove;

					UpdateGraphicalClock(_timeLeftForBlackPlayer, _timeLeftForBlackText);
				}
			}
		}


		void Update()
		{
			if (_isWhitePlayerClockWorking)
			{
				_timeLeftForWhitePlayer -= Time.deltaTime;

				UpdateGraphicalClock(_timeLeftForWhitePlayer, _timeLeftForWhiteText);

				if (_timeLeftForWhitePlayer <= 0)
				{
					_isWhitePlayerClockWorking = false;
					_useClocks = false;
					OnTimeElapsed?.Invoke();
				}
			}

			if (_isBlackPlayerClockWorking)
			{
				_timeLeftForBlackPlayer -= Time.deltaTime;

				UpdateGraphicalClock(_timeLeftForBlackPlayer, _timeLeftForBlackText);

				if (_timeLeftForBlackPlayer <= 0)
				{
					_isBlackPlayerClockWorking = false;
					_useClocks = false;
					OnTimeElapsed?.Invoke();
				}
			}
		}
	}
}
