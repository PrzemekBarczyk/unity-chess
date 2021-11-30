public struct BoardStatistics
{
	public int Evaluation;
	public ulong ZobristKey;

	public BoardStatistics(int evaluation, ulong zobristKey)
	{
		Evaluation = evaluation;
		ZobristKey = zobristKey;
	}
}
