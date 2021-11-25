public sealed class TranspositionTable
{
	public const int EVAL_MISSING = -1000001;

	public const int EXACT = 0;
	public const int UPPER_BOUND = 1;
	public const int LOWER_BOUND = 2;

	public Entry[] _entries;

	public ulong Size { get; private set; }

	Board _board;

	public TranspositionTable(Board board, int size)
	{
		_board = board;
		Size = (ulong)size;
		_entries = new Entry[Size];
	}

	public void Clear()
	{
		_entries = new Entry[Size];
	}

	public ulong Index()
	{
		return _board.ZobristHash % Size;
	}

	public Entry GetEntry()
	{
		Entry entry = _entries[Index()];

		if (entry.key == _board.ZobristHash) // entry exists
		{
			return entry;
		}
		return Entry.InvalidEntry();
	}

	public void StoreEntry(int depth, int evaluation, int nodeType, Move move)
	{
		_entries[Index()] = new Entry(_board.ZobristHash, depth, evaluation, nodeType, move);
	}
}

public struct Entry
{
	public readonly ulong key;
	public readonly int depth;
	public readonly int evaluation;
	public readonly int nodeType;
	public readonly Move move;

	public Entry(ulong key, int depth, int evaluation, int nodeType, Move move)
	{
		this.key = key;
		this.evaluation = evaluation;
		this.depth = depth;
		this.nodeType = nodeType;
		this.move = move;
	}

	public static Entry InvalidEntry() => new Entry();
	public static bool IsEntryInvalid(Entry entry)
	{
		if (entry.depth == 0 && entry.key == 0)
			return true;
		return false;
	}
}
