using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerftTest : MonoBehaviour
{
	enum TestType { Perft, Divide }

	[SerializeField] TestType _testType;

    [SerializeField] SinglePerftInfo _test;

    [SerializeField] Text _resultTextField;

    void Start()
    {
        _resultTextField.text = "Tested position:\n" + _test.TestedFEN + "\n\n";

		GameSettings gameSettings = new GameSettings(
			"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1",
			GameType.HumanVsHuman,
			false,
			0,
			0
		);
        ChessEngine chessEngine = new ChessEngine(gameSettings);

		switch (_testType)
		{
			case TestType.Perft:
				RunPerftTest(chessEngine);
				break;
			case TestType.Divide:
				RunDivideTest(chessEngine);
				break;
		}
    }

    public void RunPerftTest(ChessEngine chessEngine)
    {
        StartCoroutine(Perft(chessEngine));
    }

    public void RunDivideTest(ChessEngine chessEngine)
    {
        Divide(chessEngine);
    }

	IEnumerator Perft(ChessEngine chessEngine)
	{
		int depth = Mathf.Min(_test.MaxDepth, _test.CorrectResults.Length);

		for (int i = 0; i <= depth; i++)
		{
			var timer = new System.Diagnostics.Stopwatch();
			timer.Start();
			ulong nodesNumber = chessEngine.Perft.RunSinglePerft(i);
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

	void Divide(ChessEngine chessEngine)
	{
		List<string> results = chessEngine.Perft.RunDivide(_test.MaxDepth);

		foreach (string line in results)
		{
			_resultTextField.text += line + "\n";
		}
	}
}
