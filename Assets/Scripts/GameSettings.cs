public enum GameType { HumanVsBot, BotVsHuman, HumanVsHuman, BotVsBot }

public struct GameSettings
{
    public string StartPositionInFEN { get; set; }
    public GameType GameType { get; set; }
    public bool UseClocks { get; set; }
    public uint BaseTime { get; set; }
    public uint AddedTime { get; set; }
}
