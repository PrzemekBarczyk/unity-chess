using UnityEngine;

public class King : SlidingPiece
{
    public override PieceType Type => PieceType.King;

    public static int VALUE => 9000;
    public override int Value { get => VALUE; }

    int[,] _positionsValues = {
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -20, -30, -30, -40, -40, -30, -30, -20 },
            { -10, -20, -20, -20, -20, -20, -20, -10 },
            {  20,  20,   0,   0,   0,   0,  20,  20 },
            {  20,  30,  10,   0,   0,  10,  30,  20 }
    };
    public override int[,] PositionsValues => _positionsValues;

    public bool CanCastleKingside { get; set; }
    public bool CanCastleQueenside { get; set; }

    new void Awake()
	{
		base.Awake();
	}

    public override void GenerateLegalMoves()
    {
        LegalMoves.Clear();

        FindSlidingMoves(new Vector2Int(-1, 1), 1);
        FindSlidingMoves(new Vector2Int(0, 1), 1);
        FindSlidingMoves(new Vector2Int(1, 1), 1);
        FindSlidingMoves(new Vector2Int(1, 0), 1);
        FindSlidingMoves(new Vector2Int(1, -1), 1);
        FindSlidingMoves(new Vector2Int(0, -1), 1);
        FindSlidingMoves(new Vector2Int(-1, -1), 1);
        FindSlidingMoves(new Vector2Int(-1, 0), 1);

        FindCastlingMove(false);
        FindCastlingMove(true);
    }

    public void FindCastlingMove(bool rightCastle)
    {
        if (rightCastle ? !CanCastleKingside : !CanCastleQueenside) // canCastle tells if king or proper rook moved since start
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

        if (rookOldSquare.Piece == null || !(rookOldSquare.Piece is Rook) || rookOldSquare.Piece.Color != Color) // rook missing or wrong color
            return;

        if (!rightCastle && _board.Squares[rookOldPosition.x + 1, rookOldPosition.y].IsOccupied)
            return;

        Vector2Int checkedPosition = Square.Position + positionModifier;
        for (int i = 0; i < 2; i++) // squares king has to move over
        {
            Square checkedSuare = _board.Squares[checkedPosition.x, checkedPosition.y];

            if (checkedSuare.IsAttackedBy(Color == ColorType.White ? ColorType.Black : ColorType.White) || checkedSuare.IsOccupied) // under attack or occupied
                return;

            checkedPosition += positionModifier;
        }

        Square newKingSquare = _board.Squares[newKingPosition.x, newKingPosition.y];
        LegalMoves.Add(new MoveData(this, Square, newKingSquare, null, MoveType.Castle, rookOldSquare, rookNewSquare));
    }

    public override void Move(MoveData moveToMake, bool updateGraphic = false)
    {
        base.Move(moveToMake, updateGraphic);

        CanCastleQueenside = false;
        CanCastleKingside = false;

        if (moveToMake.Type == MoveType.Castle)
        {
            Rook rook = moveToMake.RookOldSquare.Piece as Rook;
            MoveData rookMove = new MoveData(rook, moveToMake.RookOldSquare, moveToMake.RookNewSquare, null);
            rook.Move(rookMove, updateGraphic);
        }
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
