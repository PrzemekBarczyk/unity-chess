using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;

public class King : SlidingPiece
{
    public override PieceType Type => PieceType.King;

    public static int VALUE => 9000;
    public override int Value { get => VALUE; }

    public readonly static int[,] POSITION_VALUES = {
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -20, -30, -30, -40, -40, -30, -30, -20 },
            { -10, -20, -20, -20, -20, -20, -20, -10 },
            {  20,  20,   0,   0,   0,   0,  20,  20 },
            {  20,  30,  10,   0,   0,  10,  30,  20 }
    };
    public override int[,] PositionsValues => POSITION_VALUES;

    public bool CanCastleKingside { get; set; }
    public bool CanCastleQueenside { get; set; }

    public King(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

    public override void GenerateLegalMoves()
    {
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

        Square rookOldSquare = _board.Squares[rookOldPosition.x][rookOldPosition.y];
        Square rookNewSquare = _board.Squares[rookNewPosition.x][rookNewPosition.y];

        if (rookOldSquare.Piece == null || !(rookOldSquare.Piece is Rook) || rookOldSquare.Piece.Color != Color) // rook missing or wrong color
            return;

        if (!rightCastle && _board.Squares[rookOldPosition.x + 1][rookOldPosition.y].IsOccupied())
            return;

        Vector2Int checkedPosition = Square.Position + positionModifier;
        for (int i = 0; i < 2; i++) // squares king has to move over
        {
            Square checkedSuare = _board.Squares[checkedPosition.x][checkedPosition.y];

            if (checkedSuare.IsAttackedBy(Color == ColorType.White ? ColorType.Black : ColorType.White) || checkedSuare.IsOccupied()) // under attack or occupied
                return;

            checkedPosition += positionModifier;
        }

        Square newKingSquare = _board.Squares[newKingPosition.x][newKingPosition.y];
        Pieces.LegalMoves.Add(new Move(this, Square, newKingSquare, null, MoveType.Castle, rookOldSquare, rookNewSquare));
    }

    public override void Move(Move moveToMake)
    {
        base.Move(moveToMake);

        CanCastleQueenside = false;
        CanCastleKingside = false;

        if (moveToMake.Type == MoveType.Castle)
        {
            Rook rook = moveToMake.RookOldSquare.Piece as Rook;
            Move rookMove = new Move(rook, moveToMake.RookOldSquare, moveToMake.RookNewSquare, null);
            rook.Move(rookMove);
        }
    }

	public override void UndoMove(Move moveToUndo)
	{
        if (moveToUndo.Type == MoveType.Castle)
		{
            Rook rook = moveToUndo.RookNewSquare.Piece as Rook;
            Move rookMove = new Move(rook, moveToUndo.RookOldSquare, moveToUndo.RookNewSquare, null);
            rook.UndoMove(rookMove);
        }

		base.UndoMove(moveToUndo);
	}

	public bool IsChecked()
    {
        ColorType attackerColor = Color == ColorType.White ? ColorType.Black : ColorType.White;
        return Square.IsAttackedBy(attackerColor);
    }
}
