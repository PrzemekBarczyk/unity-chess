namespace Backend
{
	public struct Move
	{
		public MoveType Type { get; private set; }
		public Piece Piece { get; private set; }

		public Square OldSquare { get; private set; }
		public Square NewSquare { get; private set; }

		public Piece EncounteredPiece { get; private set; }

		internal Square RookOldSquare { get; private set; }
		internal Square RookNewSquare { get; private set; }

		public bool IsPromotion { get => Type == MoveType.PromotionToQueen || Type == MoveType.PromotionToRook || Type == MoveType.PromotionToKnight || Type == MoveType.PromotionToBishop; }

		internal Move(Piece piece, Square oldSquare, Square newSquare, Piece encounteredPiece, MoveType type = MoveType.Normal, Square rookOldSquare = null, Square rookNewSquare = null)
		{
			Type = type;
			Piece = piece;

			OldSquare = oldSquare;
			NewSquare = newSquare;

			EncounteredPiece = encounteredPiece;

			RookOldSquare = rookOldSquare;
			RookNewSquare = rookNewSquare;
		}

		public override string ToString()
		{
			return Piece.Color + " " + Piece.Type + " " + SimplifiedAlgebraicNotation.MoveToLongSAN(this);
		}
	}
}
