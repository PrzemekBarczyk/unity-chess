using System;
using Vector2Int = UnityEngine.Vector2Int;

public static class AlgebraicNotation
{
    public static Vector2Int AlgebraicNotationToPosition(string algebraicNotation)
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

    public static string PositionToAlgebraicNotation(Vector2Int position)
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

    public static string MoveToAlgebraicNotation(Move move)
    {
        return PositionToAlgebraicNotation(move.OldSquare.Position) + PositionToAlgebraicNotation(move.NewSquare.Position);
    }
}
