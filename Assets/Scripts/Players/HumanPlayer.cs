using System;

public class HumanPlayer : Player
{
    public override PlayerType Type => PlayerType.Human;

    protected override MoveData SelectMove() // runs in separate thread 
    {
        throw new NotImplementedException();
    }
}
