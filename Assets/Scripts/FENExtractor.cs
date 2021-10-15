using System;
using System.Collections.Generic;
using UnityEngine;

public static class FENExtractor
{
	public static ExtractedFENData FENToBoardPositionData(string fen)
	{
		string[] splitedFENString = fen.Split(' ');

		bool[] castlingRights = ExtractCastlingRights(splitedFENString[2]);

		return new ExtractedFENData(ExtractPiecePlacement(splitedFENString[0]), ExtractPlayerToMoveColor(splitedFENString[1]),
			castlingRights[0], castlingRights[1], castlingRights[2], castlingRights[3], ExtractEnPassantTargetPiecePosition(splitedFENString[3]),
			ExtractHalfMoveClock(splitedFENString[4]), ExtractFullMovesClock(splitedFENString[5]));
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
				throw new FormatException("Forbidden char type");
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
				throw new FormatException("Unknown piece type");
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

		Vector2Int enPassantTargetSquarePosition = Board.AlgebraicNotationToPosition(enPassantSquare);
		Vector2Int enPassantPiecePiecePosition = enPassantTargetSquarePosition + Vector2Int.up * (enPassantTargetSquarePosition.y == 2 ? 1 : -1);

		return enPassantPiecePiecePosition;
	}

	static int ExtractHalfMoveClock(string halfMovesClock)
	{
		return (int)char.GetNumericValue(halfMovesClock[0]);
	}

	static int ExtractFullMovesClock(string fullMovesNumber)
	{
		return (int)char.GetNumericValue(fullMovesNumber[0]);
	}
}
