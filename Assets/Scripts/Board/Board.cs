using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoSingleton<Board>
{
    [SerializeField] Square _whiteSquarePrefab;
    [SerializeField] Square _blackSquarePrefab;

    public const int BOARD_SIZE = 64;

    public const int FILES = 8;
    public const int RANKS = 8;

    public const int LEFT_FILE = 0;
    public const int RIGHT_FILE = 7;

    public const int TOP_RANK = 7;
    public const int BOTTOM_RANK = 0;

    public Square[,] Squares { get; } = new Square[FILES, RANKS];

    new void Awake()
    {
        base.Awake();

        CreateBoard();
    }

    void CreateBoard()
    {
        for (int y = 0; y < RANKS; y++)
        {
            for (int x = 0; x < FILES; x++)
            {
                Square newSquarePrefab = (x + y) % 2 == 0 ? _blackSquarePrefab : _whiteSquarePrefab;
                Squares[x, y] = Instantiate(newSquarePrefab, new Vector2(x, y), transform.rotation, transform);
                Squares[x, y].name = newSquarePrefab.name;
            }
        }
    }

    public void DisplayLegalMovesIndicators(List<MoveData> legalMoves)
    {
        foreach (MoveData move in legalMoves)
        {
            if (move.EncounteredPiece != null) move.NewSquare.DisplayValidForAttackIndicator();
            else move.NewSquare.DisplayValidForMoveIndicator();
        }
    }

    public void HideLegalMovesIndicators(List<MoveData> legalMoves)
    {
        foreach (MoveData move in legalMoves)
            move.NewSquare.HideValidMovementIndicators();
    }

    public void DisplayLastMoveIndicators(MoveData lastMove)
    {
        lastMove.OldSquare.DisplayLastMoveIndicator();
        lastMove.NewSquare.DisplayLastMoveIndicator();
    }

    public void HideLastMoveIndicators(MoveData lastMove)
    {
        lastMove.OldSquare.HideLastMoveIndicator();
        lastMove.NewSquare.HideLastMoveIndicator();
    }

    public static Vector2Int AlgebraicNotationToPosition(string an)
	{
        int fileIndex = 0;
        foreach (char fileSymbols in "abcdefgh")
		{
            if (fileSymbols == an[0])
                return new Vector2Int(fileIndex, (int)char.GetNumericValue(an[1]) - 1);
            fileIndex++;
        }
        throw new FormatException("Forbidden file symbol");
    }
}
