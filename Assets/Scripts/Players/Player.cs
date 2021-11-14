using UnityEngine;

public enum PlayerType { Undefinied, Human, Bot }

public abstract class Player : MonoBehaviour
{
	public ColorType Color { get; private set; }
	public abstract PlayerType Type { get; }

	public Move? LastMove { get; protected set; }

	public void Initialize(ColorType color)
	{
		Color = color;
	}

	public abstract Move SelectMove(ChessEngine chessEngine);
}
