using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
	public bool IsAlive { get; private set; } = true;

    [SerializeField] ColorType _color;

    public ColorType Color => _color;

    public abstract PieceType Type { get; }

	public Square Square { get; set; }

	public List<MoveData> LegalMoves { get; private set; } = new List<MoveData>();

	public PieceSet Pieces { get; private set; }

	GameManager _gameManager;
	protected PieceManager _pieceManager;
	protected Board _board;

	protected void Awake()
	{
		_gameManager = GameManager.Instance;
		_pieceManager = PieceManager.Instance;
		_board = Board.Instance;

		Pieces = Color == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;

		Square = _board.Squares[(int)transform.position.x, (int)transform.position.y];
		Square.Piece = this;
	}

	public abstract void GenerateLegalMoves();

	protected void SaveMoveIfLegal(MoveData pseudoLegalMove)
	{
		if (pseudoLegalMove.IsCheckFriendly())
			LegalMoves.Add(pseudoLegalMove);
	}

	public virtual void Move(MoveData moveToMake, bool updateGraphic = false)
	{
		RightsData currentRights = new RightsData(_pieceManager.EnPassantTarget, Pieces.King.CanCastleKingside, Pieces.King.CanCastleQueenside);
		_gameManager.History.Push(new HistoryData(moveToMake, currentRights));

		if (moveToMake.EncounteredPiece != null)
		{
			moveToMake.EncounteredPiece.IsAlive = false;
			moveToMake.EncounteredPiece.Square.Piece = null;
			if (updateGraphic) moveToMake.EncounteredPiece.gameObject.SetActive(false);
		}

		moveToMake.OldSquare.Piece = null;
		moveToMake.NewSquare.Piece = this;
		Square = moveToMake.NewSquare;

		if (updateGraphic) transform.position = new Vector3(Square.Position.x, Square.Position.y);

		if (!(this is Pawn)) // removes unactive old en passants
			_pieceManager.EnPassantTarget = null;
	}

	public virtual void UndoMove(MoveData moveToUndo, bool updateGraphic = false)
	{
		moveToUndo.OldSquare.Piece = this;
		moveToUndo.NewSquare.Piece = null;
		Square = moveToUndo.OldSquare;

		if (moveToUndo.EncounteredPiece != null)
		{
			moveToUndo.EncounteredPiece.IsAlive = true;
			moveToUndo.EncounteredPiece.Square.Piece = moveToUndo.EncounteredPiece;
			if (updateGraphic) moveToUndo.EncounteredPiece.gameObject.SetActive(true);
		}

		if (updateGraphic) transform.position = new Vector3(Square.Position.x, Square.Position.y);

		HistoryData previousMove = _gameManager.History.Pop();
		_pieceManager.EnPassantTarget = previousMove.Rights.EnPassantTarget;
		Pieces.King.CanCastleKingside = previousMove.Rights.CanCastleKingside;
		Pieces.King.CanCastleQueenside = previousMove.Rights.CanCastleQueenside;
	}
}
