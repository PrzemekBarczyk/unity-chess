using System.Collections.Generic;

public sealed class OpeningBook
{
	Dictionary<string, List<string>> _positionsWithMoves = new Dictionary<string, List<string>>();

	public OpeningBook(ChessEngine chessEngine, string openingBook)
	{
		LoadEntriesFromFile(chessEngine, openingBook);
	}

	void LoadEntriesFromFile(ChessEngine chessEngine, string openingBook)
	{
		string[] lines = openingBook.Split('\n');

		for (int i = 0; i < lines.Length; i += 2)
		{
			string fen = lines[i];
			List<string> moves = new List<string>();

			foreach (string move in lines[i+1].Split(' '))
			{
				if (move == "")
					break;
				moves.Add(move);
			}

			AddEntry(fen, moves);
		}
	}

	void AddEntry(string fen, List<string> moves)
	{
		if (_positionsWithMoves.ContainsKey(fen))
		{
			foreach (var move in moves)
				_positionsWithMoves[fen].Add(move);
		}
		else
		{
			_positionsWithMoves.Add(fen, moves);
		}
	}

	public List<string> FindEntry(string currentFEN)
	{
		if (_positionsWithMoves.ContainsKey(currentFEN))
		{
			return _positionsWithMoves[currentFEN];
		}
		else
		{
			return null;
		}
	}
}
