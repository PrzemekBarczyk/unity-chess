public class BotPlayer : Player
{
    public override PlayerType Type => PlayerType.Bot;

	public override Move SelectMove(ChessEngine chessEngine) // runs in separate thread 
	{
		LastMove = chessEngine.FindBestMove();
		return chessEngine.FindBestMove();
	}
}
