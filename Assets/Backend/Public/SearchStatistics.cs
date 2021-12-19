public struct SearchStatistics
{
	public uint Depth;
	public int BestEvaluation;
	public uint PositionsEvaluated;
	public uint Cutoffs;
	public uint Transpositions;

	public SearchStatistics(uint depth, int bestEvaluation, uint positionsEvaluated, uint cutoffs, uint transpositions)
	{
		Depth = depth;
		BestEvaluation = bestEvaluation;
		PositionsEvaluated = positionsEvaluated;
		Cutoffs = cutoffs;
		Transpositions = transpositions;
	}
}
