public class PieceManager
{
    public PieceSet WhitePieces { get; private set; }
    public PieceSet BlackPieces { get; private set; }

    public PieceSet CurrentPieces { get; private set; }
    public PieceSet NextPieces { get; private set; }

    public PieceManager(Board board, ExtractedFENData extractedFENData)
    {
        WhitePieces = new PieceSet(board, ColorType.White, extractedFENData.PiecesToCreate);
        BlackPieces = new PieceSet(board, ColorType.Black, extractedFENData.PiecesToCreate);

        WhitePieces.King.CanCastleKingside = extractedFENData.HasWhiteCastleKingsideRights;
        WhitePieces.King.CanCastleQueenside = extractedFENData.HasWhiteCastleQueensideRights;
        BlackPieces.King.CanCastleKingside = extractedFENData.HasBlackCastleKingsideRights;
        BlackPieces.King.CanCastleQueenside = extractedFENData.HasBlackCastleQueensideRights;

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
