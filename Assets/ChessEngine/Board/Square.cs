using Mathf = UnityEngine.Mathf;
using Vector2Int = UnityEngine.Vector2Int;

public class Square
{
    public Vector2Int Position { get; }
    public Piece Piece { get; set; }

    Board _board;

    public Square(Board board, Vector2Int position)
    {
        _board = board;

        Position = position;
    }

    public bool IsOccupied()
    {
        return Piece != null;
    }

    public bool IsPromotionSquare(ColorType pieceColor)
    {
        return pieceColor == ColorType.White ? (Position.y == Board.TOP_RANK_INDEX) : (Position.y == Board.BOTTOM_RANK_INDEX);
    }

    public bool IsAttackedBy(ColorType attackerColor)
    {
        if (IsAttackedFromDirection(new Vector2Int(0, 1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, 1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, 0), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(0, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, 0), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, 1), attackerColor)) return true;

        if (IsAttackedFromRelativePosition(new Vector2Int(-1, 2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(1, 2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(2, 1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(2, -1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-1, -2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(1, -2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-2, -1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-2, 1), attackerColor)) return true;

        return false;
    }

    bool IsAttackedFromDirection(Vector2Int direction, ColorType opponentColor)
    {
        Vector2Int checkedPosition = Position;

        for (int i = 0; i < 8; i++)
        {
            checkedPosition += direction;

            if (checkedPosition.x < Board.LEFT_FILE_INDEX || checkedPosition.x > Board.RIGHT_FILE_INDEX || // square outside board
                checkedPosition.y < Board.BOTTOM_RANK_INDEX || checkedPosition.y > Board.TOP_RANK_INDEX)
                return false;

            Square checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];

            if (checkedSquare.IsOccupied())
            {
                Piece encounteredPiece = checkedSquare.Piece;

                if (encounteredPiece.Color != opponentColor)
                    return false;

                Vector2Int distance = checkedPosition - Position;

                if (encounteredPiece.Type == PieceType.Queen)
                {
                    if (distance.x == 0 || distance.y == 0 || (Mathf.Abs(distance.x) == Mathf.Abs(distance.y)))
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Rook)
                {
                    if (distance.x == 0 || distance.y == 0)
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Bishop)
                {
                    if (Mathf.Abs(distance.x) == Mathf.Abs(distance.y))
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Pawn)
                {
                    if (Mathf.Abs(distance.x) == 1 && distance.y == (encounteredPiece.Color == ColorType.White ? -1 : 1))
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.King)
                {
                    if (Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.y) <= 1)
                        return true;
                }

                return false;
            }
        }

        return false;
    }

    bool IsAttackedFromRelativePosition(Vector2Int offset, ColorType opponentColor)
    {
        Vector2Int checkedPosition = Position + offset;

        if (checkedPosition.x < Board.LEFT_FILE_INDEX || checkedPosition.x > Board.RIGHT_FILE_INDEX || // square outside board
            checkedPosition.y < Board.BOTTOM_RANK_INDEX || checkedPosition.y > Board.TOP_RANK_INDEX)
            return false;

        Square checkedSquare = _board.Squares[checkedPosition.x][checkedPosition.y];

        if (checkedSquare.IsOccupied())
        {
            Piece encounteredPiece = checkedSquare.Piece;

            if (encounteredPiece.Color == opponentColor) // square occupied by other color
            {
                if (encounteredPiece.Type == PieceType.Knight)
                    return true;
            }
        }

        return false;
    }
}
