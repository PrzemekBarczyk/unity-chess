using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perft : MonoBehaviour
{
    [SerializeField] SinglePerftInfo _test;

    [SerializeField] Text _resultTextField;

    PieceSet _startingPieces;

    GameManager _gameManager;
    PieceManager _pieceManager;

    void OnValidate()
    {
        GameManager.Instance.StartChessPositionInFEN = _test.TestedFEN;
    }

    void Start()
    {
        _resultTextField.text = "Tested position:\n" + _test.TestedFEN + "\n\n";

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
        int depth = Mathf.Min(_test.MaxDepth, _test.CorrectResults.Length);

        for (int i = 0; i <= depth; i++)
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            ulong nodesNumber = Search(_startingPieces, i);
            timer.Stop();

            if (nodesNumber == _test.CorrectResults[i])
            {
                _resultTextField.text += "Depth: " + i + "  Result: " + nodesNumber + " Time: " + timer.ElapsedMilliseconds + "ms  <color=green>PASSED</color>\n";
            }
            else
            {
                _resultTextField.text += "Depth: " + i + "  Result: " + nodesNumber + " Time: " + timer.ElapsedMilliseconds + "ms  <color=red>FAILED</color> (" + _test.CorrectResults[i] + ")\n";
            }
            yield return null;
        }
        _resultTextField.text += "PERFT FINISHED";
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

    ulong Divide(PieceSet currentPieces, int depth)
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
            ulong localNodes = Divide(nextPieces, depth - 1);
            nodes += localNodes;
            legalMove.Piece.UndoMove(legalMove);

            if (depth == _test.MaxDepth)
			{
                Debug.Log(legalMove.OldSquare.Position + "" + legalMove.NewSquare.Position + " " + localNodes);
			}
        }

        return nodes;
    }
}
