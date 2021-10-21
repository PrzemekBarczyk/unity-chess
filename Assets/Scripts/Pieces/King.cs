using UnityEngine;

public class King : Piece
{
    public override PieceType Type => PieceType.King;

    public bool CanCastleKingside { get; set; }
    public bool CanCastleQueenside { get; set; }

    new void Awake()
	{
		base.Awake();
	}

    public override void FindLegalMoves()
    {
        ClearLegalMoves();

        AddSlidingMoves(new Vector2Int(-1, 1), 1);
        AddSlidingMoves(new Vector2Int(0, 1), 1);
        AddSlidingMoves(new Vector2Int(1, 1), 1);
        AddSlidingMoves(new Vector2Int(1, 0), 1);
        AddSlidingMoves(new Vector2Int(1, -1), 1);
        AddSlidingMoves(new Vector2Int(0, -1), 1);
        AddSlidingMoves(new Vector2Int(-1, -1), 1);
        AddSlidingMoves(new Vector2Int(-1, 0), 1);

        AddCastlingMove(false);
        AddCastlingMove(true);
    }

    public void AddCastlingMove(bool rightCastle)
    {
        if (rightCastle ? !CanCastleKingside : !CanCastleQueenside) // canCastle tells if rook or king moved since start
            return;

        if (IsChecked())
            return;

        Vector2Int positionModifier;
        Vector2Int rookOldPosition;
        Vector2Int rookNewPosition;
        Vector2Int newKingPosition;

        if (rightCastle)
        {
            positionModifier = Vector2Int.right;
            rookOldPosition = Square.Position + new Vector2Int(3, 0);
            rookNewPosition = Square.Position + new Vector2Int(1, 0);
            newKingPosition = Square.Position + new Vector2Int(2, 0);
        }
        else
        {
            positionModifier = Vector2Int.left;
            rookOldPosition = Square.Position + new Vector2Int(-4, 0);
            rookNewPosition = Square.Position + new Vector2Int(-1, 0);
            newKingPosition = Square.Position + new Vector2Int(-2, 0);
        }

        Square rookOldSquare = _board.Squares[rookOldPosition.x, rookOldPosition.y];

        Square rookNewSquare = _board.Squares[rookNewPosition.x, rookNewPosition.y];

        Vector2Int checkedPosition = Square.Position + positionModifier;

        for (int i = 0; i < 2; i++) // squares king has to move over
        {
            Square checkedSuare = _board.Squares[checkedPosition.x, checkedPosition.y];

            if (checkedSuare.IsAttackedBy(Color == ColorType.White ? ColorType.Black : ColorType.White) || checkedSuare.IsOccupied) // under attack or occupied
                return;

            checkedPosition += positionModifier;
        }

        Square validSquare = _board.Squares[newKingPosition.x, newKingPosition.y];
        LegalMoves.Add(new MoveData(this, Square, validSquare, null, MoveType.Castle, rookOldSquare, rookNewSquare));
    }

    public override void Move(MoveData moveToMake, bool updateGraphic = false)
    {
        CanCastleQueenside = false;
        CanCastleKingside = false;

        if (moveToMake.Type == MoveType.Castle)
		{
            Rook rook = moveToMake.RookOldSquare.Piece as Rook;
            MoveData rookMove = new MoveData(rook, moveToMake.RookOldSquare, moveToMake.RookNewSquare, null);
            rook.Move(rookMove, updateGraphic);
		}

        base.Move(moveToMake, updateGraphic);
    }

	public override void UndoMove(MoveData moveToUndo, bool updateGraphic = false)
	{
        if (moveToUndo.Type == MoveType.Castle)
		{
            Rook rook = moveToUndo.RookNewSquare.Piece as Rook;
            MoveData rookMove = new MoveData(rook, moveToUndo.RookOldSquare, moveToUndo.RookNewSquare, null);
            rook.UndoMove(rookMove);
        }

		base.UndoMove(moveToUndo, updateGraphic);
	}

	public bool IsChecked()
    {
        ColorType attackerColor = Color == ColorType.White ? ColorType.Black : ColorType.White;
        return Square.IsAttackedBy(attackerColor);
    }
}
