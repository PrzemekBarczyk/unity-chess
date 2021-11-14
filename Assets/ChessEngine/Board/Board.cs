using Vector2Int = UnityEngine.Vector2Int;

public class Board
{
    public const int SIZE = 64;

    public const int FILES = 8;
    public const int RANKS = 8;

    public const int LEFT_FILE_INDEX = 0;
    public const int RIGHT_FILE_INDEX = 7;

    public const int TOP_RANK_INDEX = 7;
    public const int BOTTOM_RANK_INDEX = 0;

    public Square[][] Squares { get; private set; }

    public Square EnPassantTarget { get; set; }

    public Board(ExtractedFENData extractedFENData)
    {
        CreateBoard(extractedFENData);
    }

    void CreateBoard(ExtractedFENData extractedFENData)
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
