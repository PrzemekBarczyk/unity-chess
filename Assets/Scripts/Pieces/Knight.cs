public class Knight : Piece
{
    public override PieceType Type => PieceType.Knight;

	new void Awake()
	{
		base.Awake();
	}
}
