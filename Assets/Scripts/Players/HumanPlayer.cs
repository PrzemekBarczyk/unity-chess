using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveSelector))]
public class HumanPlayer : Player
{
    public override PlayerType Type => PlayerType.Human;

    List<Move> _legalMoves;

    MoveSelector _moveSelector;

    void Awake()
	{
        _moveSelector = GetComponent<MoveSelector>();
    }

    public override Move SelectMove(ChessEngine chessEngine) // runs in separate thread 
    {
        _legalMoves = chessEngine.GenerateLegalMoves(Color);

        _moveSelector.SetLegalMoves(_legalMoves);

        while (!_moveSelector.IsMoveSelected)
            continue;

        LastMove = _moveSelector.GetSelectedMove();
        return LastMove.Value;
    }

    public void OnSquareSelected(GraphicalSquare selectedSquare) => StartCoroutine(_moveSelector.OnSquareSelected(selectedSquare));

    public void OnSuareDeselected() => StartCoroutine(_moveSelector.OnPieceDrop());
}
