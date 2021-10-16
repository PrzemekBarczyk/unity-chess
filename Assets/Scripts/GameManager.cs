using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] string _startChessPositionInFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

	PieceSet _whitePieces;
	PieceSet _blackPieces;

	PlayerManager _players;

	void Awake()
	{
		_whitePieces = GameObject.Find("White Pieces").GetComponent<PieceSet>();
		_blackPieces = GameObject.Find("Black Pieces").GetComponent<PieceSet>();

		_players = FindObjectOfType<PlayerManager>();

		ExtractDataFromFEN();
    }

    void ExtractDataFromFEN()
	{
		ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(_startChessPositionInFEN);

		_whitePieces.CreatePieces(extractedFENData.PiecesToCreate);
		_blackPieces.CreatePieces(extractedFENData.PiecesToCreate);
		_players.SetStartingPlayerColor(extractedFENData.PlayerToMoveColor);
	}
}
