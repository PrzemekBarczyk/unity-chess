public class King : Piece
{
    public override PieceType Type => PieceType.King;

	new void Awake()
	{
		base.Awake();
	}
}
