using Vector2Int = UnityEngine.Vector2Int;

namespace Backend
{
	public enum PieceType { Undefinied, Pawn, Knight, Bishop, Rook, Queen, King }

	public sealed class Piece
	{
		internal bool IsAlive { get; set; } = true;

		public ColorType Color { get; }
		public PieceType Type { get; }
		public Square Square { get; set; }

		internal PieceSet Pieces { get; }

		Board _board;

		internal Piece(Board board, PieceSet pieces, ColorType color, PieceType type, Vector2Int position)
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
}
