namespace Backend
{
	internal struct RightsData
	{
		internal Square EnPassantTarget { get; private set; }
		internal bool CanCastleKingside { get; private set; }
		internal bool CanCastleQueenside { get; private set; }

		internal RightsData(Square enPassantTarget, bool canCastleKingside, bool canCastleQueenside)
		{
			EnPassantTarget = enPassantTarget;
			CanCastleKingside = canCastleKingside;
			CanCastleQueenside = canCastleQueenside;
		}
	}
}
