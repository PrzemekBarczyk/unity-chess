public struct RightsData
{
	public Pawn EnPassantTarget { get; private set; }
	public bool CanCastleKingside { get; private set; }
	public bool CanCastleQueenside { get; private set; }

	public RightsData(Pawn enPassantTarget, bool canCastleKingside, bool canCastleQueenside)
	{
		EnPassantTarget = enPassantTarget;
		CanCastleKingside = canCastleKingside;
		CanCastleQueenside = canCastleQueenside;
	}
}
