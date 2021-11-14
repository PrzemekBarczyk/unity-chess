using System.Collections.Generic;
using UnityEngine;

public class PieceSet
{
	public ColorType Color { get; }

	public List<Piece> AllPieces { get; } = new List<Piece>(16);
	public King King { get; private set; }

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
		List<Move> legalMoves = new List<Move>();
		List<Piece> allPieces = new List<Piece>(AllPieces);
		foreach (Piece piece in allPieces)
		{
			if (piece.IsAlive)
			{
				foreach (Move legalMove in piece.GenerateLegalMoves())
					legalMoves.Add(legalMove);
			}
		}
		return legalMoves;
	}

	public List<Piece> AlivePieces()
	{
		List<Piece> alivePieces = new List<Piece>();
		foreach (Piece piece in AllPieces)
		{
			if (piece.IsAlive)
				alivePieces.Add(piece);
		}
		return alivePieces;
	}
}
