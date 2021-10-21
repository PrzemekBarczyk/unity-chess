using UnityEngine;

public class PieceManager : MonoSingleton<PieceManager>
{
    [SerializeField] PieceSet _whitePieces;
    [SerializeField] PieceSet _blackPieces;

    public PieceSet WhitePieces => _whitePieces;
    public PieceSet BlackPieces => _blackPieces;

    public Pawn EnPassantTarget { get; set; }

    Board _board;

    new void Awake()
	{
        base.Awake();
        _board = Board.Instance;
	}

    public void SetEnPassantTarget(Vector2Int? enPassantTargetPosition)
	{
        if (enPassantTargetPosition.HasValue)
            EnPassantTarget = (Pawn)_board.Squares[enPassantTargetPosition.Value.x, enPassantTargetPosition.Value.y].Piece;
        else
            EnPassantTarget = null;
	}
}
