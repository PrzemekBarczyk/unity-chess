using System;
using UnityEngine;

public class Pawn : Piece
{
    public override PieceType Type => PieceType.Pawn;

	public bool OnStartingPosition => Square.Position.y == (Color == ColorType.White ? 1 : 6);

	new void Awake()
	{
		base.Awake();
	}

	public override void FindLegalMoves()
	{
		ClearLegalMoves();

		if (OnStartingPosition) AddDoubleMove(new Vector2Int(0, 1));
		else AddSingleMove(new Vector2Int(0, 1), canAttack: false, canMoveOnEmptySquare: true, canPromote: true);

		AddSingleMove(new Vector2Int(-1, 1), canAttack: true, canMoveOnEmptySquare: false, canPromote: true);
		AddSingleMove(new Vector2Int(1, 1), canAttack: true, canMoveOnEmptySquare: false, canPromote: true);

		AddEnPassantMoves();
	}

	void AddDoubleMove(Vector2Int direction)
	{
		AddSlidingMoves(direction, 2);
	}

	void AddSingleMove(Vector2Int offset, bool canAttack, bool canMoveOnEmptySquare, bool canPromote)
	{
		Vector2Int checkedPosition = Square.Position + offset;

		TryAddSingleMove(checkedPosition, canAttack, canMoveOnEmptySquare, canPromote);
	}

	void AddEnPassantMoves()
	{
		int enPassantRank = Color == ColorType.White ? 4 : 3;

		if (Square.Position.y != enPassantRank)
			return;

		Square squareOnLeft = null, squareOnRight = null;
		try
		{
			squareOnLeft = _board.Squares[(Square.Position + Vector2Int.left).x, Square.Position.y];
			squareOnRight = _board.Squares[(Square.Position + Vector2Int.right).x, Square.Position.y];
		}
		catch (IndexOutOfRangeException) { } // do nothing

		foreach (Square sideSquare in new Square[] { squareOnLeft, squareOnRight })
		{
			if (sideSquare == null) // square outside the board
				continue;

			if (sideSquare.IsOccupied && sideSquare.Piece == _pieceManager.EnPassantTarget)
			{
				Square newSquare = _board.Squares[sideSquare.Position.x, (sideSquare.Position + Vector2Int.up * DirectionModifier).y];

				MoveData move = new MoveData(this, Square, newSquare, sideSquare.Piece);

				if (IsMoveCheckFriendly(move))
				{
					LegalMoves.Add(move);
					return;
				}
			}
		}
	}

	public override void Move(MoveData moveToMake, bool updateGraphic = false)
	{
		if (moveToMake.OldSquare.Position.y == (Color == ColorType.White ? 1 : 6) && moveToMake.NewSquare.Position.y == (Color == ColorType.White ? 3 : 4))
			_pieceManager.EnPassantTarget = this;
		//if (moveToMake.IsPromotion)
			// TODO

		base.Move(moveToMake, updateGraphic);
	}

	public override void UndoMove(MoveData moveToUndo, bool updateGraphic = false)
	{
		//if (moveToUndo.IsPromotion)
			// TODO

		base.UndoMove(moveToUndo, updateGraphic);
	}
}
