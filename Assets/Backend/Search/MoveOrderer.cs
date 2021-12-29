using System.Collections.Generic;

namespace Backend
{
	internal static class MoveOrderer
	{
		const int CAPTURED_PIECE_VALUE_MULTIPLIER = 10;

		internal static void EvaluateAndSort(List<Move> movesToSort, bool useTranspositionTables = false, TranspositionTable tt = null)
		{
			int[] scores = new int[movesToSort.Count];

			Move hashMove = new Move();
			if (useTranspositionTables)
			{
				hashMove = tt.GetEntry().move;
			}

			for (int i = 0; i < movesToSort.Count; i++)
			{
				scores[i] = Evaluate(movesToSort[i]);

				if (movesToSort[i].Equals(hashMove))
				{
					scores[i] += 10000;
				}
			}

			Sort(movesToSort, scores);
		}

		static int Evaluate(Move move)
		{
			int score = 0;

			if (move.EncounteredPiece != null)
			{
				score = CAPTURED_PIECE_VALUE_MULTIPLIER * PiecesValues.GetValue(move.EncounteredPiece) - PiecesValues.GetValue(move.Piece);
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
				for (int j = 0; j < movesToSort.Count - 1; j++)
				{
					int swapIndex = j + 1;
					if (scores[swapIndex] > scores[j])
					{
						(movesToSort[j], movesToSort[swapIndex]) = (movesToSort[swapIndex], movesToSort[j]);
						(scores[j], scores[swapIndex]) = (scores[swapIndex], scores[j]);
					}
				}
			}
		}
	}
}
