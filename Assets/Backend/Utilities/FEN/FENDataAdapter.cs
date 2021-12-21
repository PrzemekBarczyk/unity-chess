using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
	public struct FENDataAdapter
	{
		public List<PieceData> Pieces { get; private set; }
		public ColorType PlayerToMoveColor { get; private set; }
		internal bool HasWhiteCastleKingsideRights { get; private set; }
		internal bool HasWhiteCastleQueensideRights { get; private set; }
		internal bool HasBlackCastleKingsideRights { get; private set; }
		internal bool HasBlackCastleQueensideRights { get; private set; }
		internal Vector2Int? EnPassantTargetPiecePosition { get; private set; }
		internal uint HalfMovesClock { get; private set; }
		internal uint FullMovesNumber { get; private set; }

		internal FENDataAdapter(List<PieceData> piecesToCreate, ColorType playerToMoveColor,
								bool hasWhiteCastleKingsideRights, bool hasWhiteCastleQueensideRights,
								bool hasBlackCastleKingsideRights, bool hasBlackCastleQueensideRights,
								Vector2Int? enPassantTargetPiecePosition, uint halfMovesClock, uint fullMovesNumber)
		{
			Pieces = piecesToCreate;
			PlayerToMoveColor = playerToMoveColor;
			HasWhiteCastleKingsideRights = hasWhiteCastleKingsideRights;
			HasWhiteCastleQueensideRights = hasWhiteCastleQueensideRights;
			HasBlackCastleKingsideRights = hasBlackCastleKingsideRights;
			HasBlackCastleQueensideRights = hasBlackCastleQueensideRights;
			EnPassantTargetPiecePosition = enPassantTargetPiecePosition;
			HalfMovesClock = halfMovesClock;
			FullMovesNumber = fullMovesNumber;
		}
	}
}
