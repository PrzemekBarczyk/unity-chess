using System;
using UnityEngine;

[RequireComponent(typeof(PawnPromotions))]
public class Pawn : SlidingPiece
{
    public override PieceType Type => PieceType.Pawn;

	public static int VALUE => 100;
	public override int Value => VALUE;

	int[,] _positionsValues = {
			{ 0,   0,  0,  0,  0,  0,  0,  0 },
			{ 50, 50, 50, 50, 50, 50, 50, 50 },
			{ 10, 10, 20, 30, 30, 20, 10, 10 },
			{ 5,   5, 10, 25, 25, 10,  5,  5 },
			{ 0,   0,  0, 20, 20,  0,  0,  0 },
			{ 5,  -5,-10,  0,  0,-10, -5,  5 },
			{ 5,  10, 10,-20,-20, 10, 10,  5 },
			{ 0,   0,  0,  0,  0,  0,  0,  0 }
	};
	public override int[,] PositionsValues => _positionsValues;

	public bool OnStartingPosition => Square.Position.y == (Color == ColorType.White ? 1 : 6);

	public int DirectionModifier => Color == ColorType.White ? 1 : -1;

	bool IsPromoted { get => _pawnPromotions.CurrentPromotion != null; }

	PawnPromotions _pawnPromotions;

	new void Awake()
	{
		base.Awake();
		_pawnPromotions = GetComponent<PawnPromotions>();
	}

	public override void GenerateLegalMoves()
	{
		LegalMoves.Clear();

		if (IsPromoted)
		{
			_pawnPromotions.CurrentPromotion.GenerateLegalMoves();
			return;
		}

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
		base.Move(moveToMake, updateGraphic);

		if (moveToMake.OldSquare.Position.y == (Color == ColorType.White ? 1 : 6) && moveToMake.NewSquare.Position.y == (Color == ColorType.White ? 3 : 4))
			_pieceManager.EnPassantTarget = this;
		else if (moveToMake.IsPromotion)
			_pawnPromotions.Promote(moveToMake.Type, updateGraphic);
	}

	public override void UndoMove(MoveData moveToUndo, bool updateGraphic = false)
	{
		base.UndoMove(moveToUndo, updateGraphic);

		if (moveToUndo.IsPromotion)
			_pawnPromotions.UndoPromotion(updateGraphic);
	}
}
