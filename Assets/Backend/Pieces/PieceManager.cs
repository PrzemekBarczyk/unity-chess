namespace Backend
{
    internal sealed class PieceManager
    {
        internal PieceSet WhitePieces { get; private set; }
        internal PieceSet BlackPieces { get; private set; }

        internal PieceSet CurrentPieces { get; private set; }
        internal PieceSet NextPieces { get; private set; }

        internal PieceManager(Board board, FENDataAdapter extractedFENData)
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

        internal void SwitchPlayer()
        {
            PieceSet temp = CurrentPieces;
            CurrentPieces = NextPieces;
            NextPieces = temp;
        }
    }
}
