using Vector2Int = UnityEngine.Vector2Int;

public enum PieceType { Undefinied, Pawn, Knight, Bishop, Rook, Queen, King }

public abstract class Piece
{
	public bool IsAlive { get; set; } = true;

    public ColorType Color { get; private set; }
    public abstract PieceType Type { get; }
	public Square Square { get; set; }

	public PieceSet Pieces { get; private set; }

	public abstract int Value { get; }
	public abstract int[,] PositionsValues { get; }

	protected Board _board;

	public Piece(Board board, PieceSet pieces, ColorType color, Vector2Int position)
	{
		_board = board;

		Pieces = pieces;

		IsAlive = true;
		Color = color;
		Square = _board.Squares[position.x][position.y];
		_board.Squares[position.x][position.y].Piece = this;
	}
}
