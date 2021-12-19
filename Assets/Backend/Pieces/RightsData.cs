public struct RightsData
{
	public Square EnPassantTarget { get; private set; }
	public bool CanCastleKingside { get; private set; }
	public bool CanCastleQueenside { get; private set; }

	public RightsData(Square enPassantTarget, bool canCastleKingside, bool canCastleQueenside)
	{
		EnPassantTarget = enPassantTarget;
		CanCastleKingside = canCastleKingside;
		CanCastleQueenside = canCastleQueenside;
	}
}
