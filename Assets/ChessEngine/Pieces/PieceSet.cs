using System.Collections.Generic;

public class PieceSet
{
	public ColorType Color { get; }

	public List<Piece> AllPieces { get; } = new List<Piece>(16);
	public King King { get; private set; }

	public Stack<RightsData> RightsData { get; private set; } = new Stack<RightsData>(32);
	public List<Move> LegalMoves { get; private set; } = new List<Move>(128);

	public PieceSet(Board board, ColorType color, List<PieceData> piecesToCreate)
	{
		Color = color;
		CreatePieces(board, piecesToCreate);
	}

	public void CreatePieces(Board board, List<PieceData> piecesToCreate)
	{
		foreach (PieceData pieceToCreate in piecesToCreate)
		{
			if (pieceToCreate.Color == Color)
			{
				switch (pieceToCreate.Type)
				{
					case PieceType.Pawn:
						Pawn newPawn = new Pawn(board, this, Color, pieceToCreate.Position);
						AllPieces.Add(newPawn);
						break;
					case PieceType.Knight:
						Knight newKnight = new Knight(board, this, Color, pieceToCreate.Position);
						AllPieces.Add(newKnight);
						break;
					case PieceType.Bishop:
						Bishop newBishop = new Bishop(board, this, Color, pieceToCreate.Position);
						AllPieces.Add(newBishop);
						break;
					case PieceType.Rook:
						Rook newRook = new Rook(board, this, Color, pieceToCreate.Position);
						AllPieces.Add(newRook);
						break;
					case PieceType.Queen:
						Queen newQueen = new Queen(board, this, Color, pieceToCreate.Position);
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

	public List<Move> GenerateLegalMoves()
	{
		LegalMoves.Clear();
		List<Piece> allPieces = new List<Piece>(AllPieces);
		for (int i = 0; i < allPieces.Count; i++)
		{
			if (allPieces[i].IsAlive)
			{
				allPieces[i].GenerateLegalMoves();
			}
		}
		return LegalMoves;
	}
}
