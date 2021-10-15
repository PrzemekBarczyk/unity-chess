using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] string _startChessPositionInFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

	PieceSet _whitePieces;
	PieceSet _blackPieces;

	void Awake()
	{
		_whitePieces = GameObject.Find("White Pieces").GetComponent<PieceSet>();
		_blackPieces = GameObject.Find("Black Pieces").GetComponent<PieceSet>();

		ExtractDataFromFEN();
    }

    void ExtractDataFromFEN()
	{
		ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(_startChessPositionInFEN);

		_whitePieces.CreatePieces(extractedFENData.PiecesToCreate);
		_blackPieces.CreatePieces(extractedFENData.PiecesToCreate);
	}
}
