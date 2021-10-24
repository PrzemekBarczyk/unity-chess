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
        MoveData selectedMoveWithProperType = selectedMove.Piece.LegalMoves.Find(x => (x.NewSquare == selectedMove.NewSquare && !selectedMove.IsPromotion) || (x.NewSquare == selectedMove.NewSquare && selectedMove.IsPromotion && x.Type == selectedMove.Type));

        return selectedMoveWithProperType;
    }

    public void OnSquareSelected(Square selectedSquare) => StartCoroutine(_moveSelector.OnSquareSelected(selectedSquare));

    public void OnSuareDeselected() => StartCoroutine(_moveSelector.OnPieceDrop());
}
