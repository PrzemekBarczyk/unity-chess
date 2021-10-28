using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perft : MonoBehaviour
{
    [SerializeField] string _testedFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    [SerializeField] ulong[] _correctResults;
    [SerializeField] [Range(0, 10)] ushort _maxDepth = 4;

    [SerializeField] Text _resultText;

    PieceSet _startingPieces;

    GameManager _gameManager;
    PieceManager _pieceManager;

    void OnValidate()
    {
        GameManager.Instance.StartChessPositionInFEN = _testedFEN;
    }

    void Start()
    {
        _resultText.text = "Tested position:\n" + _testedFEN + "\n\n";

        _gameManager = GameManager.Instance;
        _pieceManager = PieceManager.Instance;

        _startingPieces = _gameManager.ExtractedFENData.PlayerToMoveColor == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;

        RunPerftTest();
    }

    public void RunPerftTest()
    {
        StartCoroutine(PerftTest());
    }

    IEnumerator PerftTest()
    {
        int depth = Mathf.Min(_maxDepth, _correctResults.Length);

        for (int i = 0; i <= depth; i++)
        {
            ulong nodesNumber = Search(_startingPieces, i);

            if (nodesNumber == _correctResults[i])
            {
                _resultText.text += "Depth: " + i + "  Result: " + nodesNumber + "  <color=green>PASSED</color>\n";
            }
            else
            {
                _resultText.text += "Depth: " + i + "  Result: " + nodesNumber + "  <color=red>FAILED</color> (" + _correctResults[i] + ")\n";
            }
            yield return null;
        }
        _resultText.text += "PERFT FINISHED";
    }

    ulong Search(PieceSet currentPieces, int depth)
    {
        if (depth == 0)
        {
            return 1;
        }

        currentPieces.GenerateLegalMoves();
        List<MoveData> legalMoves = currentPieces.GetLegalMoves();

        PieceSet nextPieces = currentPieces.Color == ColorType.White ? _pieceManager.BlackPieces : _pieceManager.WhitePieces;

        ulong nodes = 0;

        foreach (MoveData legalMove in legalMoves)
        {
            legalMove.Piece.Move(legalMove);
            nodes += Search(nextPieces, depth - 1);
            legalMove.Piece.UndoMove(legalMove);
        }

        return nodes;
    }
}
