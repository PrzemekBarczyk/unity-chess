using System;
using UnityEngine;

public class Pawn : SlidingPiece
{
    public override PieceType Type => PieceType.Pawn;

	public bool OnStartingPosition => Square.Position.y == (Color == ColorType.White ? 1 : 6);

	public int DirectionModifier => Color == ColorType.White ? 1 : -1;

	new void Awake()
	{
		base.Awake();
	}

	public override void GenerateLegalMoves()
	{
		LegalMoves.Clear();

		if (OnStartingPosition) FindSlidingMoves(new Vector2Int(0, 1 * DirectionModifier), 2, canAttack: false, canMoveOnEmptySquare: true);
		else FindSlidingMoves(new Vector2Int(0, 1 * DirectionModifier), 1, canAttack: false, canMoveOnEmptySquare: true, canPromote: true);

		FindSlidingMoves(new Vector2Int(-1, 1 * DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);
		FindSlidingMoves(new Vector2Int(1, 1 * DirectionModifier), 1, canAttack: true, canMoveOnEmptySquare: false, canPromote: true);

		FindEnPassantMoves();
	}

	void FindEnPassantMoves()
	{
		int enPassantRank = Color == ColorType.White ? 4 : 3;

		if (Square.Position.y != enPassantRank)
			return;

		Square squareOnLeft = null, squareOnRight = null;
		try
		{
			squareOnLeft = _board.Squares[(Square.Position.x - 1), Square.Position.y];
			squareOnRight = _board.Squares[(Square.Position.x + 1), Square.Position.y];
		}
		catch (IndexOutOfRangeException) { } // do nothing

		foreach (Square sideSquare in new Square[] { squareOnLeft, squareOnRight })
		{
			if (sideSquare == null) // square outside the board
				continue;

			if (sideSquare.IsOccupied && sideSquare.Piece == _pieceManager.EnPassantTarget)
			{
				Square newSquare = _board.Squares[sideSquare.Position.x, sideSquare.Position.y + DirectionModifier];

				MoveData move = new MoveData(this, Square, newSquare, sideSquare.Piece);

				if (move.IsCheckFriendly())
				{
					LegalMoves.Add(move);
					return;
				}
			}
		}
	}

	public override void Move(MoveData moveToMake, bool updateGraphic = false)
	{
		//if (moveToMake.IsPromotion)
			// TODO

		base.Move(moveToMake, updateGraphic);

		if (moveToMake.OldSquare.Position.y == (Color == ColorType.White ? 1 : 6) && moveToMake.NewSquare.Position.y == (Color == ColorType.White ? 3 : 4))
			_pieceManager.EnPassantTarget = this;
	}

	public override void UndoMove(MoveData moveToUndo, bool updateGraphic = false)
	{
		//if (moveToUndo.IsPromotion)
			// TODO

		base.UndoMove(moveToUndo, updateGraphic);
	}
}
