public class Pawn : Piece
{
    public override PieceType Type => PieceType.Pawn;

	new void Awake()
	{
		base.Awake();
	}
}
