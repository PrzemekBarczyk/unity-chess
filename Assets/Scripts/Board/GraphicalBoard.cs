using Backend;
using System.Collections;
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

        [Header("Squares Prefabs")]
        [SerializeField] GraphicalSquare _whiteSquarePrefab;
        [SerializeField] GraphicalSquare _blackSquarePrefab;

        [Header("Piece Movement Animation")]
        [SerializeField] bool _animate = true;
        [SerializeField] SpriteRenderer _movingPiecePrefab;
        [SerializeField] [Range(0.05f, 0.3f)] float _moveDuration = 0.2f;

        [Header("Move Selector")]
        [SerializeField] MoveSelector _moveSelector;

        [Header("Pieces Sprites")]
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

            Squares[moveToMake.OldSquare.Position.x, moveToMake.OldSquare.Position.y].DisplayLastMoveIndicator();
            Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y].DisplayLastMoveIndicator();

            _lastMove = moveToMake;

            GraphicalSquare encounteredPieceSquare = null;
            if (moveToMake.EncounteredPiece != null)
            {
                encounteredPieceSquare = Squares[moveToMake.EncounteredPiece.Square.Position.x, moveToMake.EncounteredPiece.Square.Position.y];
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

                StartCoroutine(MovePiece(rookOldSquare, rookNewSquare, encounteredPieceSquare, rookOldSquare.PieceSprite));
            }

            GraphicalSquare oldSquare = Squares[moveToMake.OldSquare.Position.x, moveToMake.OldSquare.Position.y];
            GraphicalSquare newSquare = Squares[moveToMake.NewSquare.Position.x, moveToMake.NewSquare.Position.y];

            if (!moveToMake.IsPromotion)
            {
                StartCoroutine(MovePiece(oldSquare, newSquare, encounteredPieceSquare, _piecesSprites.GetSprite(moveToMake.Piece.Color, moveToMake.Piece.Type)));
            }
            else if (moveToMake.Type == MoveType.PromotionToKnight)
            {
                StartCoroutine(MovePiece(oldSquare, newSquare, encounteredPieceSquare, _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Knight)));
            }
            else if (moveToMake.Type == MoveType.PromotionToBishop)
            {
                StartCoroutine(MovePiece(oldSquare, newSquare, encounteredPieceSquare, _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Bishop)));
            }
            else if (moveToMake.Type == MoveType.PromotionToRook)
            {
                StartCoroutine(MovePiece(oldSquare, newSquare, encounteredPieceSquare, _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Rook)));
            }
            else if (moveToMake.Type == MoveType.PromotionToQueen)
            {
                StartCoroutine(MovePiece(oldSquare, newSquare, encounteredPieceSquare, _piecesSprites.GetSprite(moveToMake.Piece.Color, PieceType.Queen)));
            }
        }

        IEnumerator MovePiece(GraphicalSquare startSquare, GraphicalSquare endSquare, GraphicalSquare encounteredPieceSquare, Sprite finalSprite)
		{
            if (!_animate || _moveSelector.PieceWasDropped)
			{
                startSquare.PieceSprite = null;
            }

            if (_animate && !_moveSelector.PieceWasDropped)
            {
                SpriteRenderer piece = Instantiate(_movingPiecePrefab, startSquare.transform.position, transform.rotation, transform);
                piece.sprite = startSquare.PieceSprite;
                if (Camera.main.transform.rotation != Quaternion.Euler(Vector3.zero))
                {
                    piece.transform.rotation = Camera.main.transform.rotation;
                }

                startSquare.PieceSprite = null;

                float t = 0f;
                while (t < _moveDuration)
                {
                    piece.transform.position = Vector2.Lerp(startSquare.transform.position, endSquare.transform.position, t / _moveDuration);
                    t += Time.deltaTime;
                    yield return null;
                }

                Destroy(piece.gameObject);
            }

            if (encounteredPieceSquare != null)
            {
                encounteredPieceSquare.PieceSprite = null;
            }
            endSquare.PieceSprite = finalSprite;

            _moveSelector.PieceWasDropped = false;
        }
    }
}
