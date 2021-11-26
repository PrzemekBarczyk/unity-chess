public sealed class PieceManager
{
    public PieceSet WhitePieces { get; private set; }
    public PieceSet BlackPieces { get; private set; }

    public PieceSet CurrentPieces { get; private set; }
    public PieceSet NextPieces { get; private set; }

    public PieceManager(Board board, FENDataAdapter extractedFENData)
    {
        WhitePieces = new PieceSet(board, ColorType.White, extractedFENData.Pieces);
        BlackPieces = new PieceSet(board, ColorType.Black, extractedFENData.Pieces);

        WhitePieces.CanKingCastleKingside = extractedFENData.HasWhiteCastleKingsideRights;
        WhitePieces.CanKingCastleQueenside = extractedFENData.HasWhiteCastleQueensideRights;
        BlackPieces.CanKingCastleKingside = extractedFENData.HasBlackCastleKingsideRights;
        BlackPieces.CanKingCastleQueenside = extractedFENData.HasBlackCastleQueensideRights;

        CurrentPieces = extractedFENData.PlayerToMoveColor == ColorType.White ? WhitePieces : BlackPieces;
        NextPieces = extractedFENData.PlayerToMoveColor == ColorType.White ? BlackPieces : WhitePieces;
    }

    public void SwitchPlayer()
    {
        PieceSet temp = CurrentPieces;
        CurrentPieces = NextPieces;
        NextPieces = temp;
    }
}
