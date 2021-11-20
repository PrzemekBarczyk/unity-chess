using UnityEngine;

public enum PlayerType { Undefinied, Human, Bot }

[RequireComponent(typeof(Clock))]
public abstract class Player : MonoBehaviour
{
	public ColorType Color { get; private set; }
	public abstract PlayerType Type { get; }

	public Move? LastMove { get; protected set; }

	public Clock Clock { get; private set; }

	public void Initialize(ColorType color, float timeForMove, float timeAddedAfterMove)
	{
		Color = color;
		Clock = GetComponent<Clock>();
		Clock.SetUp(Color, timeForMove, timeAddedAfterMove);
	}

	public Move SelectMoveAndCountTime(ChessEngine chessEngine)
	{
		Clock.Run();
		Move move = SelectMove(chessEngine);
		Clock.Stop();
		return move;
	}

	public abstract Move SelectMove(ChessEngine chessEngine);
}
