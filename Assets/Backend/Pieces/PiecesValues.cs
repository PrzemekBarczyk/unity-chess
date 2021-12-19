using System;

public static class PiecesValues
{
    public const int PAWN = 100;
    public const int KNIGHT = 300;
    public const int BISHOP = 300;
    public const int ROOK = 500;
    public const int QUEEN = 900;
    public const int KING = 9000;

    public static int GetValue(Piece piece)
    {
        switch (piece.Type)
        {
            case PieceType.Pawn:
                return PAWN;
            case PieceType.Knight:
                return KNIGHT;
            case PieceType.Bishop:
                return BISHOP;
            case PieceType.Rook:
                return ROOK;
            case PieceType.Queen:
                return QUEEN;
            case PieceType.King:
                return KING;
            default:
                throw new Exception();
        }
    }
}
