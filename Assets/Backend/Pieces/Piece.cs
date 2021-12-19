using Vector2Int = UnityEngine.Vector2Int;

public enum PieceType { Undefinied, Pawn, Knight, Bishop, Rook, Queen, King }

public sealed class Piece
{
	public bool IsAlive { get; set; } = true;

    public ColorType Color { get; }
    public PieceType Type { get; }
	public Square Square { get; set; }

	public PieceSet Pieces { get; }

	protected Board _board;

	public Piece(Board board, PieceSet pieces, ColorType color, PieceType type, Vector2Int position)
	{
		_board = board;

		Pieces = pieces;

		IsAlive = true;
		Color = color;
		Type = type;

		Square = _board.Squares[position.x][position.y];
		_board.Squares[position.x][position.y].Piece = this;
	}
}
