using UnityEngine;

public enum PlayerType { Undefinied, Human, Bot }

[RequireComponent(typeof(Clock))]
public abstract class Player : MonoBehaviour
{
	public ColorType Color { get; private set; }
	public abstract PlayerType Type { get; }

	public Move? LastMove { get; protected set; }

	public Clock Clock { get; private set; }

	bool _useClock;

	public void SetColor(ColorType color)
	{
		Color = color;
	}

	public void SetClock(bool useClock, uint baseTime, uint addedTime)
	{
		_useClock = useClock;

		Clock = GetComponent<Clock>();
		Clock.SetUp(Color, useClock, baseTime, addedTime);
	}

	public Move SelectMoveAndCountTime(ChessEngine chessEngine)
	{
		if (_useClock) Clock.Run();
		Move move = SelectMove(chessEngine);
		if (_useClock) Clock.Stop();
		return move;
	}

	public abstract Move SelectMove(ChessEngine chessEngine);
}
