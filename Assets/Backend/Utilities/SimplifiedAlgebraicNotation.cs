using System;
using Vector2Int = UnityEngine.Vector2Int;

public static class SimplifiedAlgebraicNotation
{
    public static string MoveToLongSAN(Move move)
    {
        return PositionToShortSAN(move.OldSquare.Position) + PositionToShortSAN(move.NewSquare.Position);
    }

    public static Move LongSANToMove(ChessEngine chessEngine, string longAlgebraicNotation)
    {
        Vector2Int oldPosition = ShortSANToPosition(longAlgebraicNotation.Substring(0, 2));
        Vector2Int newPosition = ShortSANToPosition(longAlgebraicNotation.Substring(2, 2));

        foreach (Move move in chessEngine.GenerateLegalMoves())
		{
            if (move.OldSquare.Position == oldPosition && move.NewSquare.Position == newPosition)
			{
                return move;
			}
		}
        throw new Exception("Move isn't legal");
    }

    public static Vector2Int ShortSANToPosition(string algebraicNotation)
    {
        byte fileIndex = 0;
        foreach (char fileSymbol in "abcdefgh")
        {
            if (fileSymbol == algebraicNotation[0])
                return new Vector2Int(fileIndex, (byte)(char.GetNumericValue(algebraicNotation[1]) - 1));
            fileIndex++;
        }
        throw new FormatException("Forbidden file symbol");
    }

    public static string PositionToShortSAN(Vector2Int position)
    {
        int rankIndex = 0;
        foreach (char fileSymbol in "abcdefgh")
        {
            if (rankIndex == position.x)
            {
                return fileSymbol + (position.y + 1).ToString();
            }
            rankIndex++;
        }
        throw new FormatException("Forbidden file symbol");
    }
}
