using System;
using System.Collections.Generic;
using Vector2Int = UnityEngine.Vector2Int;

public static class FENConverter
{
	public static FENDataAdapter FENToBoardPositionData(string fen)
	{
		string[] splitedFENString = fen.Split(' ');

		if (splitedFENString.Length != 6)
			throw new FormatException("FEN has to many fields");

		bool[] castlingRights = ExtractCastlingRights(splitedFENString[2]);

		return new FENDataAdapter(ExtractPiecePlacement(splitedFENString[0]), ExtractPlayerToMoveColor(splitedFENString[1]),
			castlingRights[0], castlingRights[1], castlingRights[2], castlingRights[3], ExtractEnPassantTargetPiecePosition(splitedFENString[3]),
			ExtractHalfMoveClock(splitedFENString[4]), ExtractFullMovesNumber(splitedFENString[5]));
	}

	static List<PieceData> ExtractPiecePlacement(string piecesPositions)
	{
		List<PieceData> piecesToCreateData = new List<PieceData>();

		Vector2Int piecePosition = new Vector2Int(0, Board.RANKS - 1);

		foreach (char singleChar in piecesPositions)
		{
			if (singleChar.Equals('/'))
			{
				piecePosition.x = 0;
				piecePosition.y -= 1;
				continue;
			}

			if (char.IsNumber(singleChar))
			{
				piecePosition.x += (int)char.GetNumericValue(singleChar);
				continue;
			}

			if (char.IsLetter(singleChar))
			{
				ColorType pieceColor = char.IsUpper(singleChar) ? ColorType.White : ColorType.Black;
				PieceType pieceType = SelectPieceType(singleChar);
				piecesToCreateData.Add(new PieceData(pieceColor, pieceType, piecePosition));
				piecePosition.x += 1;
			}
			else
			{
				throw new FormatException("Forbidden char in piece placement field");
			}
		}

		return piecesToCreateData;
	}

	static PieceType SelectPieceType(char pieceChar)
	{
		switch (char.ToLower(pieceChar))
		{
			case 'p':
				return PieceType.Pawn;
			case 'n':
				return PieceType.Knight;
			case 'b':
				return PieceType.Bishop;
			case 'r':
				return PieceType.Rook;
			case 'q':
				return PieceType.Queen;
			case 'k':
				return PieceType.King;
			default:
				throw new FormatException("Unknown piece type in piece placement");
		}
	}

	static ColorType ExtractPlayerToMoveColor(string activeColor)
	{
		if (activeColor.Equals("w"))
		{
			return ColorType.White;
		}
		else if (activeColor.Equals("b"))
		{
			return ColorType.Black;
		}
		else
		{
			throw new FormatException("Unknown active color symbol");
		}
	}

	static bool[] ExtractCastlingRights(string castlingRights)
	{
		bool[] result = new bool[4];

		result[0] = castlingRights.Contains("K");
		result[1] = castlingRights.Contains("Q");
		result[2] = castlingRights.Contains("k");
		result[3] = castlingRights.Contains("q");

		return result;
	}

	static Vector2Int? ExtractEnPassantTargetPiecePosition(string enPassantSquare)
	{
		if (enPassantSquare[0].Equals('-'))
		{
			return null;
		}

		Vector2Int enPassantPosition;
		try
		{
			enPassantPosition = AlgebraicNotation.AlgebraicNotationToPosition(enPassantSquare);
		}
		catch (FormatException)
		{
			throw new FormatException("Uncorrect en passant position");
		}

		if (enPassantPosition.y != 2 && enPassantPosition.y != 5)
		{
			throw new FormatException("Uncorrect en passant position");
		}

		return enPassantPosition;
	}

	static uint ExtractHalfMoveClock(string halfMovesClock)
	{
		uint halfMovesClockValue;
		try
		{
			halfMovesClockValue = (uint)char.GetNumericValue(halfMovesClock[0]);
		}
		catch (Exception)
		{
			throw new FormatException("Uncorrect half moves clock");
		}

		return halfMovesClockValue;
	}

	static uint ExtractFullMovesNumber(string fullMovesNumber)
	{
		uint halfMovesNumberValue;
		try
		{
			halfMovesNumberValue = (uint)char.GetNumericValue(fullMovesNumber[0]);
		}
		catch (Exception)
		{
			throw new FormatException("Uncorrect full moves number");
		}

		return halfMovesNumberValue;
	}

	public static string BoardPositionToFEN(FENDataAdapter fenDataAdapter)
	{
		string fen = "";

		fen += ParsePiecePlacement(fenDataAdapter.Pieces);
		fen += " ";
		fen += ParseSideToMove(fenDataAdapter.PlayerToMoveColor);
		fen += " ";
		fen += ParseCastlingRights(fenDataAdapter.HasWhiteCastleKingsideRights,
								   fenDataAdapter.HasWhiteCastleQueensideRights,
								   fenDataAdapter.HasBlackCastleKingsideRights,
								   fenDataAdapter.HasBlackCastleQueensideRights);
		fen += " ";
		fen += ParseEnPassant(fenDataAdapter.EnPassantTargetPiecePosition);
		fen += " ";
		fen += ParseHalfMoveClock(fenDataAdapter.HalfMovesClock);
		fen += " ";
		fen += ParseFullMoveNumber(fenDataAdapter.FullMovesNumber);

		return fen;
	}

	static string ParsePiecePlacement(List<PieceData> pieces)
	{
		string piecePlacement = "";

		for (int y = Board.TOP_RANK_INDEX; y >= 0; y--)
		{
			uint emptySquaresInRow = 0;
			for (int x = 0; x <= Board.RIGHT_FILE_INDEX; x++)
			{
				bool squareOccupied = false;
				foreach (PieceData piece in pieces)
				{
					if (piece.Position.x == x && piece.Position.y == y)
					{
						squareOccupied = true;

						if (emptySquaresInRow > 0)
						{
							piecePlacement += emptySquaresInRow;
							emptySquaresInRow = 0;
						}
						piecePlacement += SelectPieceChar(piece);

						break;
					}
				}

				if (!squareOccupied)
				{
					emptySquaresInRow++;
				}
			}

			if (emptySquaresInRow > 0)
			{
				piecePlacement += emptySquaresInRow;
			}

			if (y != 0)
			{
				piecePlacement += "/";
			}
		}

		return piecePlacement;
	}

	static char SelectPieceChar(PieceData piece)
	{
		char pieceChar;

		switch (piece.Type)
		{
			case PieceType.Pawn:
				pieceChar = 'p';
				break;
			case PieceType.Knight:
				pieceChar = 'n';
				break;
			case PieceType.Bishop:
				pieceChar = 'b';
				break;
			case PieceType.Rook:
				pieceChar = 'r';
				break;
			case PieceType.Queen:
				pieceChar = 'q';
				break;
			case PieceType.King:
				pieceChar = 'k';
				break;
			default:
				throw new FormatException("Unknown piece type");
		}

		return piece.Color == ColorType.White ? char.ToUpper(pieceChar) : pieceChar;
	}

	static char ParseSideToMove(ColorType sideToMove)
	{
		if (sideToMove == ColorType.White) return 'w';
		return 'b';
	}

	static string ParseCastlingRights(bool whiteKingsideCastle, bool whiteQueensideCastle, bool blackKingsideCastle, bool blackQueensideCastle)
	{
		string castlingRights = "";

		if (whiteKingsideCastle) castlingRights += "K";
		if (whiteQueensideCastle) castlingRights += "Q";
		if (blackKingsideCastle) castlingRights += "k";
		if (blackQueensideCastle) castlingRights += "q";

		return castlingRights;
	}

	static string ParseEnPassant(Vector2Int? enPassantPosition)
	{
		if (enPassantPosition.HasValue)
		{
			return AlgebraicNotation.PositionToAlgebraicNotation(enPassantPosition.Value);
		}
		else
		{
			return "-";
		}
	}

	static string ParseHalfMoveClock(uint halfMoveClock)
	{
		return halfMoveClock.ToString();
	}

	static string ParseFullMoveNumber(uint fullMoveNumber)
	{
		return fullMoveNumber.ToString();
	}
}
