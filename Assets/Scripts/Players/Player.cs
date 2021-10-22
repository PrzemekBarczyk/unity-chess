using System.Threading;
using System.Collections;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
	public ColorType Color { get; private set; }
	public abstract PlayerType Type { get; }

	public PieceSet Pieces { get; private set; }

	public MoveData? LastMove { get; private set; }
	public MoveData MoveToMake { get; private set; }

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

	public IEnumerator Move()
	{
		Thread moveSelectionThread = new Thread(() => MoveToMake = SelectMove());
		moveSelectionThread.Start();

		yield return new WaitUntil(() => !moveSelectionThread.IsAlive);

		MoveToMake.Piece.Move(MoveToMake, updateGraphic: true);

		LastMove = MoveToMake;
	}

	protected abstract MoveData SelectMove();
}
