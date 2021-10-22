using System;

public class BotPlayer : Player
{
    public override PlayerType Type => PlayerType.Bot;

    protected override MoveData SelectMove() // runs in separate thread 
    {
        throw new NotImplementedException();
    }
}
