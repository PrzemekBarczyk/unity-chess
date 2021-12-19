using Vector2Int = UnityEngine.Vector2Int;

namespace Backend
{
    internal sealed class Board
    {
        internal const int SIZE = 64;

        internal const int FILES = 8;
        internal const int RANKS = 8;

        internal const int LEFT_FILE_INDEX = 0;
        internal const int RIGHT_FILE_INDEX = 7;

        internal const int TOP_RANK_INDEX = 7;
        internal const int BOTTOM_RANK_INDEX = 0;

        internal ulong ZobristHash { get; set; }

        internal Square[][] Squares { get; private set; }

        internal Square EnPassantTarget { get; set; }

        internal Board(FENDataAdapter extractedFENData)
        {
            CreateBoard(extractedFENData);
        }

        void CreateBoard(FENDataAdapter extractedFENData)
        {
            Squares = new Square[FILES][];
            for (int x = 0; x < FILES; x++)
            {
                Squares[x] = new Square[RANKS];
                for (int y = 0; y < RANKS; y++)
                {
                    Squares[x][y] = new Square(this, new Vector2Int(x, y));
                }
            }

            if (extractedFENData.EnPassantTargetPiecePosition.HasValue)
            {
                EnPassantTarget = Squares[extractedFENData.EnPassantTargetPiecePosition.Value.x][extractedFENData.EnPassantTargetPiecePosition.Value.y];
            }
            else
            {
                EnPassantTarget = null;
            }
        }
    }
}
