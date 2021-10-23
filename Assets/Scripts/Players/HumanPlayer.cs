using UnityEngine;

[RequireComponent(typeof(MoveSelector))]
public class HumanPlayer : Player
{
    public override PlayerType Type => PlayerType.Human;

    MoveSelector _moveSelector;

    new void Awake()
	{
        base.Awake();
        _moveSelector = GetComponent<MoveSelector>();
    }

    protected override MoveData SelectMove() // runs in separate thread 
    {
        Pieces.FindLegalMoves();

        while (!_moveSelector.IsMoveSelected)
            continue;

        MoveData selectedMove = _moveSelector.GetSelectedMove();
        MoveData selectedMoveWithProperType = selectedMove.Piece.FindLegalMove(selectedMove);

        return selectedMoveWithProperType;
    }

    public void OnSquareSelected(Square selectedSquare) => StartCoroutine(_moveSelector.OnSquareSelected(selectedSquare));

    public void OnSuareDeselected() => StartCoroutine(_moveSelector.OnPieceDrop());
}
