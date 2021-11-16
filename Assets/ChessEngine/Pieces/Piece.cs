using Vector2Int = UnityEngine.Vector2Int;

public enum PieceType { Undefinied, Pawn, Knight, Bishop, Rook, Queen, King }

public abstract class Piece
{
	public bool IsAlive { get; protected set; } = true;

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

	public abstract void GenerateLegalMoves();

	protected bool SaveMoveIfLegal(Move pseudoLegalMove)
	{
		pseudoLegalMove.Piece.Move(pseudoLegalMove);

		bool isKingChecked = pseudoLegalMove.Piece.Pieces.King.IsChecked();

		pseudoLegalMove.Piece.UndoMove(pseudoLegalMove);

		if (!isKingChecked)
		{
			Pieces.LegalMoves.Add(pseudoLegalMove);
			return true;
		}

		return false;
	}

	public virtual void Move(Move moveToMake)
	{
		RightsData currentRights = new RightsData(_board.EnPassantTarget, Pieces.King.CanCastleKingside, Pieces.King.CanCastleQueenside);
		Pieces.RightsData.Push(currentRights);

		if (moveToMake.EncounteredPiece != null)
		{
			moveToMake.EncounteredPiece.IsAlive = false;
			moveToMake.EncounteredPiece.Square.Piece = null;
		}

		moveToMake.OldSquare.Piece = null;
		moveToMake.NewSquare.Piece = this;
		Square = moveToMake.NewSquare;

		_board.EnPassantTarget = null; // removes unactive old en passants
	}

	public virtual void UndoMove(Move moveToUndo)
	{
		moveToUndo.OldSquare.Piece = this;
		moveToUndo.NewSquare.Piece = null;
		Square = moveToUndo.OldSquare;

		if (moveToUndo.EncounteredPiece != null)
		{
			moveToUndo.EncounteredPiece.IsAlive = true;
			moveToUndo.EncounteredPiece.Square.Piece = moveToUndo.EncounteredPiece;
		}

		RightsData previousRights = Pieces.RightsData.Pop();
		_board.EnPassantTarget = previousRights.EnPassantTarget;
		Pieces.King.CanCastleKingside = previousRights.CanCastleKingside;
		Pieces.King.CanCastleQueenside = previousRights.CanCastleQueenside;
	}
}
