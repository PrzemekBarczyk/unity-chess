using Backend;
using NUnit.Framework;
using UnityEngine;

public class Perft
{
    [Test]
    public void TestInitialPosition()
    {
		string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
		ulong[] correctResults = { 1, 20, 400, 8902, 197281, 4865609 };

		TestPosition(fen, correctResults);
	}

	[Test]
	public void TestPosition2()
	{
		string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
		ulong[] correctResults = { 1, 48, 2039, 97862, 4085603 };

		TestPosition(fen, correctResults);
	}

	[Test]
	public void TestPosition3()
	{
		string fen = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
		ulong[] correctResults = { 1, 14, 191, 2812, 43238, 674624 };

		TestPosition(fen, correctResults);
	}

	[Test]
	public void TestPosition4()
	{
		string fen = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
		ulong[] correctResults = { 1, 6, 264, 9467, 422333 };

		TestPosition(fen, correctResults);
	}

	[Test]
	public void TestPosition5()
	{
		string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
		ulong[] correctResults = { 1, 44, 1486, 62379, 2103487 };

		TestPosition(fen, correctResults);
	}

	[Test]
	public void TestPosition6()
	{
		string fen = "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10";
		ulong[] correctResults = { 1, 46, 2079, 89890, 3894594 };

		TestPosition(fen, correctResults);
	}

	void TestPosition(string fen, ulong[] correctResults)
	{
		string log = fen + "\n";

		for (int i = 0; i < correctResults.Length; i++)
		{
			ChessEngine chessEngine = new ChessEngine(fen);

			var timer = new System.Diagnostics.Stopwatch();
			timer.Start();
			ulong nodesNumber = chessEngine.Perft.RunSinglePerft(i);
			timer.Stop();

			Assert.AreEqual(nodesNumber, correctResults[i]);

			log += "Depth: " + i + "  Result: " + nodesNumber + " Time: " + timer.ElapsedMilliseconds + "\n";
		}

		Debug.Log(log);
	}
}
