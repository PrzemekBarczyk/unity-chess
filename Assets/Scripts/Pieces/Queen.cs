public class Queen : Piece
{
    public override PieceType Type => PieceType.Queen;

	new void Awake()
	{
		base.Awake();
	}
}
