public struct HistoryData
{
    public MoveData Move { get; private set; }
    public RightsData Rights { get; private set; }

    public HistoryData(MoveData madeMove, RightsData currentRights)
	{
        Move = madeMove;
        Rights = currentRights;
	}
}
