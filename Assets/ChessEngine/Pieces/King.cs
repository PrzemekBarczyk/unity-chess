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

    public static readonly Vector2Int WHITE_KING_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(6, Board.BOTTOM_RANK_INDEX);
    public static readonly Vector2Int WHITE_KING_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(2, Board.BOTTOM_RANK_INDEX);
    public static readonly Vector2Int BLACK_KING_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(6, Board.TOP_RANK_INDEX);
    public static readonly Vector2Int BLACK_KING_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(2, Board.TOP_RANK_INDEX);

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

        if (CanCastleQueenside) FindCastlingMove(false);
        if (CanCastleKingside) FindCastlingMove(true);
    }

    public void FindCastlingMove(bool kingsideCastle)
    {
        Vector2Int positionModifier;
        Square rookOldSquare;
        Square rookNewSquare;
        Vector2Int newKingPosition;

        if (kingsideCastle)
        {
            positionModifier = Vector2Int.right;
            if (Color == ColorType.White)
			{
                rookOldSquare = _board.Squares[Rook.WHITE_RIGHT_ROOK_START_POSITION.x][Rook.WHITE_RIGHT_ROOK_START_POSITION.y];
                rookNewSquare = _board.Squares[Rook.WHITE_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.x][Rook.WHITE_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.y];
                newKingPosition = WHITE_KING_AFTER_KINGSIDE_CASTLE_POSITION;
			}
			else // black
			{
                rookOldSquare = _board.Squares[Rook.BLACK_RIGHT_ROOK_START_POSITION.x][Rook.BLACK_RIGHT_ROOK_START_POSITION.y];
                rookNewSquare = _board.Squares[Rook.BLACK_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.x][Rook.BLACK_ROOK_AFTER_KINGSIDE_CASTLE_POSITION.y];
                newKingPosition = BLACK_KING_AFTER_KINGSIDE_CASTLE_POSITION;
            }
        }
        else // queenside castle
        {
            positionModifier = Vector2Int.left;
            if (Color == ColorType.White)
            {
                rookOldSquare = _board.Squares[Rook.WHITE_LEFT_ROOK_START_POSITION.x][Rook.WHITE_LEFT_ROOK_START_POSITION.y];
                rookNewSquare = _board.Squares[Rook.WHITE_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.x][Rook.WHITE_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.y];
                newKingPosition = WHITE_KING_AFTER_QUEENSIDE_CASTLE_POSITION;
            }
            else // black
            {
                rookOldSquare = _board.Squares[Rook.BLACK_LEFT_ROOK_START_POSITION.x][Rook.BLACK_LEFT_ROOK_START_POSITION.y];
                rookNewSquare = _board.Squares[Rook.BLACK_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.x][Rook.BLACK_ROOK_AFTER_QUEENSIDE_CASTLE_POSITION.y];
                newKingPosition = BLACK_KING_AFTER_QUEENSIDE_CASTLE_POSITION;
            }
        }

        if (rookOldSquare.Piece == null || !(rookOldSquare.Piece.Type == PieceType.Rook) || rookOldSquare.Piece.Color != Color) // rook missing or wrong color
            return;

        if (!kingsideCastle && _board.Squares[1][Square.Position.y].IsOccupied())
            return;

        Vector2Int checkedPosition = Square.Position + positionModifier;
        for (int i = 0; i < 2; i++) // squares king has to move over
        {
            Square checkedSuare = _board.Squares[checkedPosition.x][checkedPosition.y];

            if (checkedSuare.IsAttackedBy(Color == ColorType.White ? ColorType.Black : ColorType.White) || checkedSuare.IsOccupied()) // under attack or occupied
                return;

            checkedPosition += positionModifier;
        }

        if (IsChecked())
            return;

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
