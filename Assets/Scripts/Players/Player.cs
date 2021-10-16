using UnityEngine;

public abstract class Player : MonoBehaviour
{
	public ColorType Color { get; private set; }
	public abstract PlayerType Type { get; }

	public PieceSet Pieces { get; private set; }

	PieceManager _pieceManager;

	void Awake()
	{
		_pieceManager = PieceManager.Instance;
	}

	public void Initialize(ColorType color)
	{
		Color = color;
		Pieces = Color == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
	}
}
