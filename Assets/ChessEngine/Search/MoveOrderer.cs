using System.Collections.Generic;

public static class MoveOrderer
{
	const int CAPTURED_PIECE_VALUE_MULTIPLIER = 10;

	public static void EvaluateAndSort(List<Move> movesToSort)
	{
		int[] scores = new int[movesToSort.Count];

		for (int i = 0; i < movesToSort.Count; i++)
		{
			scores[i] = Evaluate(movesToSort[i]);
		}

		Sort(movesToSort, scores);
	}

	static int Evaluate(Move move)
	{
		int score = 0;

		if (move.NewSquare.IsOccupied())
		{
			score = CAPTURED_PIECE_VALUE_MULTIPLIER * (PiecesValues.GetValue(move.NewSquare.Piece) - PiecesValues.GetValue(move.OldSquare.Piece));
		}

		if (move.IsPromotion)
		{
			if (move.Type == MoveType.PromotionToQueen)
				score += PiecesValues.QUEEN;
			else if (move.Type == MoveType.PromotionToRook)
				score += PiecesValues.ROOK;
			else if (move.Type == MoveType.PromotionToBishop)
				score += PiecesValues.BISHOP;
			else if (move.Type == MoveType.PromotionToKnight)
				score += PiecesValues.KNIGHT;
		}

		return score;
	}

	static void Sort(List<Move> movesToSort, int[] scores)
	{
		for (int i = 0; i < movesToSort.Count - 1; i++)
		{
			for (int j = i + 1; j > 0; j--)
			{
				int swapIndex = j - 1;
				if (scores[swapIndex] < scores[j])
				{
					(movesToSort[j], movesToSort[swapIndex]) = (movesToSort[swapIndex], movesToSort[j]);
					(scores[j], scores[swapIndex]) = (scores[swapIndex], scores[j]);
				}
			}
		}
	}
}
