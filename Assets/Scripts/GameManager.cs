using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField] string _startChessPositionInFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

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
	}
}
