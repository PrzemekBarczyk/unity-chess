using System.Collections.Generic;

public class PieceSet
{
	public ColorType Color { get; }

	public List<Piece> AllPieces { get; } = new List<Piece>(16);
	public List<Piece> Pawns { get; } = new List<Piece>(8);
	public List<Piece> Knights { get; } = new List<Piece>(4);
	public List<Piece> Bishops { get; } = new List<Piece>(4);
	public List<Piece> Rooks { get; } = new List<Piece>(4);
	public List<Piece> Queens { get; } = new List<Piece>(4);
	public Piece King { get; private set; }

	public bool CanKingCastleKingside { get; set; }
	public bool CanKingCastleQueenside { get; set; }

	public PieceSet(Board board, ColorType color, List<PieceData> piecesToCreate)
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

	public bool IsKingChecked()
	{
		return King.Square.IsAttackedBy(Color == ColorType.White ? ColorType.Black : ColorType.White);
	}
}
