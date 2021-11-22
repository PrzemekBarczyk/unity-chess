using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphicalBoard : MonoSingleton<GraphicalBoard>
{
    [Header("Squares")]
    [SerializeField] GraphicalSquare _whiteSquarePrefab;
    [SerializeField] GraphicalSquare _blackSquarePrefab;

    [Header("Pieces")]
    [SerializeField] PiecesSprites _piecesSprites;

    public GraphicalSquare[,] Squares { get; } = new GraphicalSquare[Board.FILES, Board.RANKS];

    public void CreateBoard(List<PieceData> pieces)
    {
        for (int y = 0; y < Board.RANKS; y++)
        {
            for (int x = 0; x < Board.FILES; x++)
            {
                GraphicalSquare newSquarePrefab = (x + y) % 2 == 0 ? _blackSquarePrefab : _whiteSquarePrefab;
                GraphicalSquare newSquare = Instantiate(newSquarePrefab, new Vector2(x, y), transform.rotation, transform);

                newSquare.name = newSquarePrefab.name;

                foreach (PieceData piece in pieces)
                {
                    if (piece.Position == new Vector2Int(x, y))
                        newSquare.PieceSprite = _piecesSprites.GetSprite(piece.Color, piece.Type);
                }

                Squares[x, y] = newSquare;
            }
        }
    }

    public void UpdateBoard(Move moveToMake, Move? lastOpponentMove)
    {
        if (lastOpponentMove.HasValue)
        {
            Squares[lastOpponentMove.Value.OldSquare.Position.x, lastOpponentMove.Value.OldSquare.Position.y].HideLastMoveIndicator();
            Squares[lastOpponentMove.Value.NewSquare.Position.x, lastOpponentMove.Value.NewSquare.Position.y].HideLastMoveIndicator();
        }

        if (moveToMake.Type == MoveType.Castle)
        {
            GraphicalSquare rookOldSquare, rookNewSquare;
            if (moveToMake.OldSquare.Position.x < moveToMake.NewSquare.Position.x)
            {
                rookOldSquare = Squares[Board.RIGHT_FILE_INDEX, moveToMake.OldSquare.Position.y];
                rookNewSquare = Squares[moveToMake.OldSquare.Position.x + 1, moveToMake.OldSquare.Position.y];
            }
            else
            {
                rookOldSquare = Squares[Board.LEFT_FILE_INDEX, moveToMake.OldSquare.Position.y];
                rookNewSquare = Squares[moveToMake.OldSquare.Position.x - 1, moveToMake.OldSquare.Position.y];
            }
            rookNewSquare.PieceSprite = rookOldSquare.PieceSprite;
            rookOldSquare.PieceSprite = null;
        }

        if (moveToMake.EncounteredPiece != null)
        {
            Squares[moveToMake.EncounteredPiece.Square.Position.x, moveToMake.EncounteredPiece.Square.Position.y].PieceSprite = null;
        }

        Squares[moveToMake.OldSquare.Position.x, moveToMake.OldSquare.Position.y].PieceSprite = null;
        if (!moveToMake.IsPromotion)
            Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.OldSquare.Piece.Color, moveToMake.OldSquare.Piece.Type);
        else if (moveToMake.Type == MoveType.PromotionToKnight)
            Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.OldSquare.Piece.Color, PieceType.Knight);
        else if (moveToMake.Type == MoveType.PromotionToBishop)
            Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.OldSquare.Piece.Color, PieceType.Bishop);
        else if (moveToMake.Type == MoveType.PromotionToRook)
            Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.OldSquare.Piece.Color, PieceType.Rook);
        else if (moveToMake.Type == MoveType.PromotionToQueen)
            Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.OldSquare.Piece.Color, PieceType.Queen);

        Squares[moveToMake.OldSquare.Position.x, moveToMake.OldSquare.Position.y].DisplayLastMoveIndicator();
        Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].DisplayLastMoveIndicator();
    }
}