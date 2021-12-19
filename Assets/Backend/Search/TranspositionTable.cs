namespace Backend
{
	internal sealed class TranspositionTable
	{
		internal const int EVAL_MISSING = -1000001;

		internal const int EXACT = 0;
		internal const int UPPER_BOUND = 1;
		internal const int LOWER_BOUND = 2;

		internal Entry[] _entries;

		internal ulong Size { get; private set; }

		Board _board;

		internal TranspositionTable(Board board, int size)
		{
			_board = board;
			Size = (ulong)size;
			_entries = new Entry[Size];
		}

		internal void Clear()
		{
			_entries = new Entry[Size];
		}

		internal ulong Index()
		{
			return _board.ZobristHash % Size;
		}

		internal Entry GetEntry()
		{
			Entry entry = _entries[Index()];

			if (entry.key == _board.ZobristHash) // entry exists
			{
				return entry;
			}
			return Entry.InvalidEntry();
		}

		internal void StoreEntry(uint depth, int evaluation, int nodeType, Move move)
		{
			_entries[Index()] = new Entry(_board.ZobristHash, depth, evaluation, nodeType, move);
		}
	}

	internal struct Entry
	{
		internal readonly ulong key;
		internal readonly uint depth;
		internal readonly int evaluation;
		internal readonly int nodeType;
		internal readonly Move move;

		internal Entry(ulong key, uint depth, int evaluation, int nodeType, Move move)
		{
			this.key = key;
			this.evaluation = evaluation;
			this.depth = depth;
			this.nodeType = nodeType;
			this.move = move;
		}

		internal static Entry InvalidEntry() => new Entry();
		internal static bool IsEntryInvalid(Entry entry)
		{
			if (entry.depth == 0 && entry.key == 0)
				return true;
			return false;
		}
	}
}
