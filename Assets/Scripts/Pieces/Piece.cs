using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [SerializeField] ColorType _color;

    public ColorType Color => _color;

    public abstract PieceType Type { get; }

	public Square Square { get; private set; }

	Board _board;

	protected void Awake()
	{
		_board = FindObjectOfType<Board>();
		ChangeSquare(_board.Squares[(int)transform.position.x, (int)transform.position.y]);
	}

	void ChangeSquare(Square newSquare)
	{
		Square oldSquare = Square;
		if (oldSquare != null) oldSquare.Piece = null;

		Square = newSquare;
		Square.Piece = this;
	}
}
