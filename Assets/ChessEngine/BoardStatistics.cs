public struct BoardStatistics
{
	public int Evaluation;
	public string FEN;
	public ulong ZobristKey;

	public BoardStatistics(int evaluation, string fen, ulong zobristKey)
	{
		Evaluation = evaluation;
		FEN = fen;
		ZobristKey = zobristKey;
	}
}
