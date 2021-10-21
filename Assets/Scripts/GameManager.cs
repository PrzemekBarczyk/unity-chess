using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField] string _startChessPositionInFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

	public Stack<HistoryData> History { get; private set; } = new Stack<HistoryData>();

	PieceManager _pieces;
	PlayerManager _players;

	new void Awake()
	{
		base.Awake();

		_pieces = PieceManager.Instance;
		_players = PlayerManager.Instance;

		ExtractDataFromFEN();
    }

    void ExtractDataFromFEN()
	{
		ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(_startChessPositionInFEN);

		_pieces.WhitePieces.CreatePieces(extractedFENData.PiecesToCreate);
		_pieces.BlackPieces.CreatePieces(extractedFENData.PiecesToCreate);
		_players.SetStartingPlayerColor(extractedFENData.PlayerToMoveColor);
		_pieces.WhitePieces.King.CanCastleKingside = extractedFENData.HasWhiteCastleKingsideRights;
		_pieces.WhitePieces.King.CanCastleQueenside = extractedFENData.HasWhiteCastleQueensideRights;
		_pieces.BlackPieces.King.CanCastleKingside = extractedFENData.HasBlackCastleKingsideRights;
		_pieces.BlackPieces.King.CanCastleQueenside = extractedFENData.HasBlackCastleQueensideRights;
		_pieces.SetEnPassantTarget(extractedFENData.EnPassantTargetPiecePosition);
	}
}
