using Vector2Int = UnityEngine.Vector2Int;

public class Pawn : Piece
{
    public override PieceType Type => PieceType.Pawn;

	public static int VALUE => 100;
	public override int Value => VALUE;

	public readonly static int[,] POSITION_VALUES = {
			{ 0,   0,  0,  0,  0,  0,  0,  0 },
			{ 50, 50, 50, 50, 50, 50, 50, 50 },
			{ 10, 10, 20, 30, 30, 20, 10, 10 },
			{ 5,   5, 10, 25, 25, 10,  5,  5 },
			{ 0,   0,  0, 20, 20,  0,  0,  0 },
			{ 5,  -5,-10,  0,  0,-10, -5,  5 },
			{ 5,  10, 10,-20,-20, 10, 10,  5 },
			{ 0,   0,  0,  0,  0,  0,  0,  0 }
	};
	public override int[,] PositionsValues => POSITION_VALUES;

	public bool OnStartingPosition => Square.Position.y == (Color == ColorType.White ? 1 : 6);
	public bool OnPositionValidForEnPassant => Square.Position.y == (Color == ColorType.White ? 4 : 3);

	public int DirectionModifier => Color == ColorType.White ? 1 : -1;

	public Pawn(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public override void Move(Move moveToMake)
	{
		base.Move(moveToMake);

		bool madeDoubleMove = moveToMake.NewSquare.Position.y == (Color == ColorType.White ? 3 : 4) && moveToMake.OldSquare.Position.y == (Color == ColorType.White ? 1 : 6);
		if (madeDoubleMove)
		{
			_board.EnPassantTarget = _board.Squares[Square.Position.x][Square.Position.y - DirectionModifier];
		}
		else if (moveToMake.IsPromotion)
		{
			switch (moveToMake.Type)
			{
				case MoveType.PromotionToKnight:
					Knight newKnight = new Knight(_board, Pieces, Color, Square.Position);
					Pieces.Knights.Add(newKnight);
					Pieces.AllPieces.Add(newKnight);
					break;
				case MoveType.PromotionToBishop:
					Bishop newBishop = new Bishop(_board, Pieces, Color, Square.Position);
					Pieces.Bishops.Add(newBishop);
					Pieces.AllPieces.Add(newBishop);
					break;
				case MoveType.PromotionToRook:
					Rook newRook = new Rook(_board, Pieces, Color, Square.Position);
					Pieces.Rooks.Add(newRook);
					Pieces.AllPieces.Add(newRook);
					break;
				case MoveType.PromotionToQueen:
					Queen newQueen = new Queen(_board, Pieces, Color, Square.Position);
					Pieces.Queens.Add(newQueen);
					Pieces.AllPieces.Add(newQueen);
					break;
			}
			Pieces.AllPieces.Remove(this);
		}
	}

	public override void UndoMove(Move moveToUndo)
	{
		if (moveToUndo.IsPromotion)
		{
			switch (moveToUndo.Type)
			{
				case MoveType.PromotionToKnight:
					Pieces.Knights.Remove(Square.Piece as Knight);
					break;
				case MoveType.PromotionToBishop:
					Pieces.Bishops.Remove(Square.Piece as Bishop);
					break;
				case MoveType.PromotionToRook:
					Pieces.Rooks.Remove(Square.Piece as Rook);
					break;
				case MoveType.PromotionToQueen:
					Pieces.Queens.Remove(Square.Piece as Queen);
					break;
			}
			Pieces.AllPieces.Remove(Square.Piece);
			Square.Piece = this;
			Pieces.AllPieces.Add(this);
		}

		base.UndoMove(moveToUndo);
	}
}
