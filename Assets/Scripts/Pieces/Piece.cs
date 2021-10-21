using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
	public bool IsAlive { get; private set; } = true;

    [SerializeField] ColorType _color;

    public ColorType Color => _color;

    public abstract PieceType Type { get; }

	public Square Square { get; private set; }

	public List<MoveData> LegalMoves { get; private set; } = new List<MoveData>();

	public int DirectionModifier => Color == ColorType.White ? 1 : -1;

	public PieceSet Pieces { get; private set; }

	GameManager _gameManager;
	protected PieceManager _pieceManager;
	protected Board _board;

	protected void Awake()
	{
		Pieces = Color == ColorType.White ? PieceManager.Instance.WhitePieces : PieceManager.Instance.BlackPieces;

		_gameManager = GameManager.Instance;
		_pieceManager = PieceManager.Instance;
		_board = Board.Instance;

		ChangeSquare(_board.Squares[(int)transform.position.x, (int)transform.position.y]);
	}

	#region Finding Legal Moves
	public abstract void FindLegalMoves();

	protected void ClearLegalMoves()
	{
		LegalMoves.Clear();
	}

	protected void AddSlidingMoves(Vector2Int direction, int maxDistance = 8)
	{
		Vector2Int checkedPosition = Square.Position;

		for (int i = 1; i <= maxDistance; i++)
		{
			checkedPosition += direction * DirectionModifier;

			if (!TryAddSingleMove(checkedPosition, canAttack: true, canMoveOnEmptySquare: true, canPromote: false))
				break;
		}
	}

	protected bool TryAddSingleMove(Vector2Int checkedPosition, bool canAttack = true, bool canMoveOnEmptySquare = true, bool canPromote = false)
	{
		Square checkedSquare;
		try
		{
			checkedSquare = _board.Squares[checkedPosition.x, checkedPosition.y];
		}
		catch (IndexOutOfRangeException) // square outside board
		{
			return false;
		}

		if ((!checkedSquare.IsOccupied && canMoveOnEmptySquare) || // empty field
			(checkedSquare.IsOccupied && checkedSquare.Piece.Color != Color && canAttack)) // opponent piece
		{
			if (canPromote && checkedSquare.IsPromotionSquare(Color))
			{
				AddMoveIfLegal(new MoveData(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToKnight));
				AddMoveIfLegal(new MoveData(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToBishop));
				AddMoveIfLegal(new MoveData(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToRook));
				AddMoveIfLegal(new MoveData(this, Square, checkedSquare, checkedSquare.Piece, MoveType.PromotionToQueen));
			}
			else
			{
				AddMoveIfLegal(new MoveData(this, Square, checkedSquare, checkedSquare.Piece));
			}
		}
		else
		{
			return false;
		}
		return true;
	}

	protected void AddMoveIfLegal(MoveData pseudoLegalMove)
	{
		if (IsMoveCheckFriendly(pseudoLegalMove))
			LegalMoves.Add(pseudoLegalMove);
	}

	protected bool IsMoveCheckFriendly(MoveData pseudoLegalMove)
	{
		pseudoLegalMove.Piece.Move(pseudoLegalMove, false);

		bool isKingChecked = Pieces.King.IsChecked();

		pseudoLegalMove.Piece.UndoMove(pseudoLegalMove, false);

		return !isKingChecked;
	}
	#endregion

	#region Making and Reversing Moves
	public virtual void Move(MoveData moveToMake, bool updateGraphic = false)
	{
		RightsData currentRights = new RightsData(_pieceManager.EnPassantTarget, Pieces.King.CanCastleKingside, Pieces.King.CanCastleQueenside);
		_gameManager.History.Push(new HistoryData(moveToMake, currentRights));

		ChangeSquare(moveToMake.NewSquare);

		if (moveToMake.EncounteredPiece != null)
			CapturePiece(moveToMake.EncounteredPiece, updateGraphic);

		if (!(moveToMake.Piece is Pawn) && _pieceManager.EnPassantTarget != null) // removes unactive old en passants
			_pieceManager.EnPassantTarget = null;
	}

	public virtual void UndoMove(MoveData moveToUndo, bool updateGraphic = false)
	{
		ChangeSquare(moveToUndo.OldSquare);

		if (moveToUndo.EncounteredPiece != null)
			UndoCapturePiece(moveToUndo.EncounteredPiece, updateGraphic);

		HistoryData previousMove = _gameManager.History.Pop();
		_pieceManager.EnPassantTarget = previousMove.Rights.EnPassantTarget;
		Pieces.King.CanCastleKingside = previousMove.Rights.CanCastleKingside;
		Pieces.King.CanCastleQueenside = previousMove.Rights.CanCastleQueenside;
	}

	void ChangeSquare(Square newSquare)
	{
		Square oldSquare = Square;
		if (oldSquare != null) oldSquare.Piece = null;

		Square = newSquare;
		Square.Piece = this;
	}

	void CapturePiece(Piece enemyPiece, bool updateGraphic)
	{
		enemyPiece.Die(updateGraphic);
	}

	void UndoCapturePiece(Piece enemyPiece, bool updateGraphic)
	{
		enemyPiece.Restore(updateGraphic);
	}

	void Die(bool updateGraphic = false)
	{
		IsAlive = false;
		if (updateGraphic) gameObject.SetActive(false);
	}

	void Restore(bool updateGrpahic = false)
	{
		IsAlive = true;
		if (updateGrpahic) gameObject.SetActive(true);
	}
	#endregion
}
