using System;

namespace Backend
{
	internal static class PiecesPositionValues
	{
		internal static readonly int[,] PAWN = {
			{ 0,   0,   0,   0,   0,   0,   0,  0 },
			{ 50, 50,  50,  50,  50,  50,  50, 50 },
			{ 10, 10,  20,  30,  30,  20,  10, 10 },
			{ 5,   5,  10,  25,  25,  10,   5,  5 },
			{ 0,   0,   0,  20,  20,   0,   0,  0 },
			{ 5,  -5, -10,   0,   0, -10,  -5,  5 },
			{ 5,  10,  10, -20, -20,  10,  10,  5 },
			{ 0,   0,   0,   0,   0,   0,   0,  0 }
		};

		internal static readonly int[,] KNIGHT = {
			{ -50, -40, -30, -30, -30, -30, -40, -50 },
			{ -40, -20,   0,   0,   0,   0, -20, -40 },
			{ -30,   0,  10,  15,  15,  10,   0, -30 },
			{ -30,   5,  15,  20,  20,  15,   5, -30 },
			{ -30,   0,  15,  20,  20,  15,   0, -30 },
			{ -30,   5,  10,  15,  15,  10,   5, -30 },
			{ -40, -20,   0,   5,   5,   0, -20, -40 },
			{ -50, -40, -30, -30, -30, -30, -40, -50 }
		};

		internal static readonly int[,] BISHOP = {
			{ -20, -10, -10, -10, -10, -10, -10, -20 },
			{ -10,   0,   0,   0,   0,   0,   0, -10 },
			{ -10,   0,   5,  10,  10,   5,   0, -10 },
			{ -10,   5,   5,  10,  10,   5,   5, -10 },
			{ -10,   0,  10,  10,  10,  10,   0, -10 },
			{ -10,  10,  10,  10,  10,  10,  10, -10 },
			{ -10,   5,   0,   0,   0,   0,   5, -10 },
			{ -20, -10, -10, -10, -10, -10, -10, -20 }
		};

		internal static readonly int[,] ROOK = {
			{  0,  0,  0,  0,  0,  0,  0,  0 },
			{  5, 10, 10, 10, 10, 10, 10,  5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{ -5,  0,  0,  0,  0,  0,  0, -5 },
			{  0,  0,  0,  5,  5,  0,  0,  0 }
		};

		internal static readonly int[,] QUEEN = {
			{ -20, -10, -10, -5, -5, -10, -10, -20 },
			{ -10,   0,   0,  0,  0,   0,   0, -10 },
			{ -10,   0,   5,  5,  5,   5,   0, -10 },
			{  -5,   0,   5,  5,  5,   5,   0,  -5 },
			{   0,   0,   5,  5,  5,   5,   0,  -5 },
			{ -10,   5,   5,  5,  5,   5,   0, -10 },
			{ -10,   0,   5,  0,  0,   0,   0, -10 },
			{ -20, -10, -10, -5, -5, -10, -10, -20 }
		};

		internal static readonly int[,] KING = {
			{ -30, -40, -40, -50, -50, -40, -40, -30 },
			{ -30, -40, -40, -50, -50, -40, -40, -30 },
			{ -30, -40, -40, -50, -50, -40, -40, -30 },
			{ -30, -40, -40, -50, -50, -40, -40, -30 },
			{ -20, -30, -30, -40, -40, -30, -30, -20 },
			{ -10, -20, -20, -20, -20, -20, -20, -10 },
			{  20,  20,   0,   0,   0,   0,  20,  20 },
			{  20,  30,  10,   0,   0,  10,  30,  20 }
		};

		internal static int[,] GetValue(Piece piece)
		{
			switch (piece.Type)
			{
				case PieceType.Pawn:
					return PAWN;
				case PieceType.Knight:
					return KNIGHT;
				case PieceType.Bishop:
					return BISHOP;
				case PieceType.Rook:
					return ROOK;
				case PieceType.Queen:
					return QUEEN;
				case PieceType.King:
					return KING;
				default:
					throw new Exception();
			}
		}
	}
}