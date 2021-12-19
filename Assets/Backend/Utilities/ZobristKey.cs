using System;

public static class ZobristKey
{
	const int SEED = 1234567;

	const uint PIECE_COLORS = 2;
	const uint PIECE_TYPES = 6;

	public static readonly ulong[,,,] colorPiecePositionCombinations = new ulong[PIECE_COLORS, PIECE_TYPES, Board.FILES, Board.RANKS];
	public static readonly ulong[] castlingRights = new ulong[4];
	public static readonly ulong[] enPassantFiles = new ulong[8];
	public static readonly ulong sideToMove;

	static ZobristKey()
	{
		Random random = new Random(SEED);

		for (int y = 0; y < Board.RANKS; y++)
		{
			for (int x = 0; x < Board.FILES; x++)
			{
				for (int pieceIndex = 0; pieceIndex < PIECE_TYPES; pieceIndex++)
				{
					colorPiecePositionCombinations[0, pieceIndex, x, y] = RandomUnsigned64BitNumber(random);
					colorPiecePositionCombinations[1, pieceIndex, x, y] = RandomUnsigned64BitNumber(random);
				}
			}
		}

		for (int i = 0; i < castlingRights.Length; i++)
		{
			castlingRights[i] = RandomUnsigned64BitNumber(random);
		}

		for (int i = 0; i < enPassantFiles.Length; i++)
		{
			enPassantFiles[i] = RandomUnsigned64BitNumber(random);
		}

		sideToMove = RandomUnsigned64BitNumber(random);
	}

	static ulong RandomUnsigned64BitNumber(Random random)
	{
		byte[] buffer = new byte[8];
		random.NextBytes(buffer);
		return BitConverter.ToUInt64(buffer, 0);
	}

	public static ulong CalculateZobristKey(Board board, PieceManager pieceManager, ColorType currentPlayerColor)
	{
		ulong zobristKey = 0;

		for (int y = 0; y < Board.RANKS; y++)
		{
			for (int x = 0; x < Board.FILES; x++)
			{
				Square square = board.Squares[x][y];

				if (square.IsOccupied())
				{
					ColorType pieceColour = square.Piece.Color;
					PieceType pieceType = square.Piece.Type;

					zobristKey ^= colorPiecePositionCombinations[(uint)pieceColour - 1, (uint)pieceType - 1, x, y];
				}
			}
		}

		if (board.EnPassantTarget != null)
		{
			zobristKey ^= enPassantFiles[board.EnPassantTarget.Position.x];
		}

		if (pieceManager.WhitePieces.CanKingCastleKingside) zobristKey ^= castlingRights[0];
		if (pieceManager.WhitePieces.CanKingCastleQueenside) zobristKey ^= castlingRights[1];
		if (pieceManager.BlackPieces.CanKingCastleKingside) zobristKey ^= castlingRights[2];
		if (pieceManager.BlackPieces.CanKingCastleQueenside) zobristKey ^= castlingRights[3];

		if (currentPlayerColor == ColorType.White)
		{
			zobristKey ^= sideToMove;
		}

		return zobristKey;
	}
}
