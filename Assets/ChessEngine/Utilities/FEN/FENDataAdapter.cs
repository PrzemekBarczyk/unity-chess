using System.Collections.Generic;
using UnityEngine;

public struct FENDataAdapter
{
	public List<PieceData> Pieces { get; private set; }
	public ColorType PlayerToMoveColor { get; private set; }
	public bool HasWhiteCastleKingsideRights { get; private set; }
	public bool HasWhiteCastleQueensideRights { get; private set; }
	public bool HasBlackCastleKingsideRights { get; private set; }
	public bool HasBlackCastleQueensideRights { get; private set; }
	public Vector2Int? EnPassantTargetPiecePosition { get; private set; }
	public uint HalfMovesClock { get; private set; }
	public uint FullMovesNumber { get; private set; }

	public FENDataAdapter(List<PieceData> piecesToCreate, ColorType playerToMoveColor,
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
