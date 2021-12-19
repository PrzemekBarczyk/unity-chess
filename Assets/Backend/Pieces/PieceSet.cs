using System.Collections.Generic;

namespace Backend
{
	internal sealed class PieceSet
	{
		internal ColorType Color { get; }

		internal List<Piece> AllPieces { get; } = new List<Piece>(16);
		internal List<Piece> Pawns { get; } = new List<Piece>(8);
		internal List<Piece> Knights { get; } = new List<Piece>(4);
		internal List<Piece> Bishops { get; } = new List<Piece>(4);
		internal List<Piece> Rooks { get; } = new List<Piece>(4);
		internal List<Piece> Queens { get; } = new List<Piece>(4);
		internal Piece King { get; private set; }

		internal bool CanKingCastleKingside { get; set; }
		internal bool CanKingCastleQueenside { get; set; }

		internal PieceSet(Board board, ColorType color, List<PieceData> piecesToCreate)
		{
			Color = color;
			CreatePieces(board, piecesToCreate);
		}

		void CreatePieces(Board board, List<PieceData> piecesToCreate)
		{
			foreach (PieceData pieceToCreate in piecesToCreate)
			{
				if (pieceToCreate.Color == Color)
				{
					Piece newPiece = new Piece(board, this, Color, pieceToCreate.Type, pieceToCreate.Position);
					switch (pieceToCreate.Type)
					{
						case PieceType.Pawn:
							Pawns.Add(newPiece);
							break;
						case PieceType.Knight:
							Knights.Add(newPiece);
							break;
						case PieceType.Bishop:
							Bishops.Add(newPiece);
							break;
						case PieceType.Rook:
							Rooks.Add(newPiece);
							break;
						case PieceType.Queen:
							Queens.Add(newPiece);
							break;
						case PieceType.King:
							King = newPiece;
							break;
					}
					AllPieces.Add(newPiece);
				}
			}
		}

		internal bool IsKingChecked()
		{
			return King.Square.IsAttackedBy(Color == ColorType.White ? ColorType.Black : ColorType.White);
		}
	}
}
