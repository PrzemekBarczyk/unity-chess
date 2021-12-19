using Backend;
using System.Collections.Generic;
using UnityEngine;

namespace Frontend
{
    public class GraphicalBoard : MonoBehaviour
    {
        public const int FILES = 8;
        public const int RANKS = 8;

        public const int LEFT_FILE_INDEX = 0;
        public const int RIGHT_FILE_INDEX = 7;

        public const int TOP_RANK_INDEX = 7;
        public const int BOTTOM_RANK_INDEX = 0;

        [Header("Squares")]
        [SerializeField] GraphicalSquare _whiteSquarePrefab;
        [SerializeField] GraphicalSquare _blackSquarePrefab;

        [Header("Pieces")]
        [SerializeField] PiecesSprites _piecesSprites;

        public GraphicalSquare[,] Squares { get; } = new GraphicalSquare[FILES, RANKS];

        Move? _lastMove;

        public void CreateBoard()
        {
            for (int y = 0; y < RANKS; y++)
            {
                for (int x = 0; x < FILES; x++)
                {
                    GraphicalSquare newSquarePrefab = (x + y) % 2 == 0 ? _blackSquarePrefab : _whiteSquarePrefab;
                    GraphicalSquare newSquare = Instantiate(newSquarePrefab, new Vector2(x, y), transform.rotation, transform);

                    newSquare.name = newSquarePrefab.name;

                    Squares[x, y] = newSquare;
                }
            }
        }

        public void CreatePieces(List<PieceData> pieces)
        {
            foreach (PieceData piece in pieces)
            {
                Squares[piece.Position.x, piece.Position.y].PieceSprite = _piecesSprites.GetSprite(piece.Color, piece.Type);
            }
        }

        public void UpdateBoard(Move moveToMake)
        {
            if (_lastMove.HasValue)
            {
                Squares[_lastMove.Value.OldSquare.Position.x, _lastMove.Value.OldSquare.Position.y].HideLastMoveIndicator();
                Squares[_lastMove.Value.NewSquare.Position.x, _lastMove.Value.NewSquare.Position.y].HideLastMoveIndicator();
            }

            if (moveToMake.Type == MoveType.Castle)
            {
                GraphicalSquare rookOldSquare, rookNewSquare;
                if (moveToMake.OldSquare.Position.x < moveToMake.NewSquare.Position.x)
                {
                    rookOldSquare = Squares[RIGHT_FILE_INDEX, moveToMake.OldSquare.Position.y];
                    rookNewSquare = Squares[moveToMake.OldSquare.Position.x + 1, moveToMake.OldSquare.Position.y];
                }
                else
                {
                    rookOldSquare = Squares[LEFT_FILE_INDEX, moveToMake.OldSquare.Position.y];
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
                Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.Piece.Color, moveToMake.Piece.Type);
            else if (moveToMake.Type == MoveType.PromotionToKnight)
                Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Knight);
            else if (moveToMake.Type == MoveType.PromotionToBishop)
                Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Bishop);
            else if (moveToMake.Type == MoveType.PromotionToRook)
                Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Rook);
            else if (moveToMake.Type == MoveType.PromotionToQueen)
                Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].PieceSprite = _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Queen);

            Squares[moveToMake.OldSquare.Position.x, moveToMake.OldSquare.Position.y].DisplayLastMoveIndicator();
            Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].DisplayLastMoveIndicator();

            _lastMove = moveToMake;
        }
    }
}
