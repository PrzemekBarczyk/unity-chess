using System.Collections.Generic;

public class PieceSet
{
	public ColorType Color { get; }

	public List<Piece> AllPieces { get; } = new List<Piece>(16);
	public List<Pawn> Pawns { get; } = new List<Pawn>(8);
	public List<Knight> Knights { get; } = new List<Knight>(2);
	public List<Bishop> Bishops { get; } = new List<Bishop>(2);
	public List<Rook> Rooks { get; } = new List<Rook>(2);
	public List<Queen> Queens { get; } = new List<Queen>(2);
	public King King { get; private set; }

	public Stack<RightsData> RightsData { get; private set; } = new Stack<RightsData>(32);

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
				switch (pieceToCreate.Type)
				{
					case PieceType.Pawn:
						Pawn newPawn = new Pawn(board, this, Color, pieceToCreate.Position);
						Pawns.Add(newPawn);
						AllPieces.Add(newPawn);
						break;
					case PieceType.Knight:
						Knight newKnight = new Knight(board, this, Color, pieceToCreate.Position);
						Knights.Add(newKnight);
						AllPieces.Add(newKnight);
						break;
					case PieceType.Bishop:
						Bishop newBishop = new Bishop(board, this, Color, pieceToCreate.Position);
						Bishops.Add(newBishop);
						AllPieces.Add(newBishop);
						break;
					case PieceType.Rook:
						Rook newRook = new Rook(board, this, Color, pieceToCreate.Position);
						Rooks.Add(newRook);
						AllPieces.Add(newRook);
						break;
					case PieceType.Queen:
						Queen newQueen = new Queen(board, this, Color, pieceToCreate.Position);
						Queens.Add(newQueen);
						AllPieces.Add(newQueen);
						break;
					case PieceType.King:
						King newKing = new King(board, this, Color, pieceToCreate.Position);
						King = newKing;
						AllPieces.Add(newKing);
						break;
				}
			}
		}
	}
}
