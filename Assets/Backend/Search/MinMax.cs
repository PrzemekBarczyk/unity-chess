using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class MinMax : SearchAlgorithm
	{
		internal MinMax(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint fixedSearchDepth)
		{
			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			if (_pieceManager.CurrentPieces.Color == MAXIMIZING_COLOR)
			{
				Search(_pieceManager.CurrentPieces, fixedSearchDepth, true, fixedSearchDepth);
			}
			else
			{
				Search(_pieceManager.CurrentPieces, fixedSearchDepth, false, fixedSearchDepth);
			}

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(fixedSearchDepth, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, bool maximizingPlayer, uint maxDepth)
		{
			if (depth == 0)
			{
				return Evaluate();
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			if (legalMoves.Count == 0) // no legal moves
			{
				if (currentPlayerPieces.IsKingChecked())
					return maximizingPlayer ? -1000000 - (int)depth : 1000000 + (int)depth;
				return 0;
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			if (maximizingPlayer)
			{
				int maxEvaluation = -10000000;

				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					_moveExecutor.MakeMove(legalMove);

					int evaluation = Search(nextDepthPlayerPieces, depth - 1, false, maxDepth);

					_moveExecutor.UndoMove(legalMove);

					_positionsEvaluated++;

					if (evaluation > maxEvaluation)
					{
						maxEvaluation = evaluation;
						if (depth == maxDepth)
						{
							_bestMove = legalMove;
							_bestEvaluation = maxEvaluation;
						}
					}
				}

				return maxEvaluation;
			}
			else // minimizing player
			{
				int minEvaluation = 10000000;

				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					_moveExecutor.MakeMove(legalMove);

					int evaluation = Search(nextDepthPlayerPieces, depth - 1, true, maxDepth);

					_moveExecutor.UndoMove(legalMove);

					_positionsEvaluated++;

					if (evaluation < minEvaluation)
					{
						minEvaluation = evaluation;
						if (depth == maxDepth)
						{
							_bestMove = legalMove;
							_bestEvaluation = minEvaluation;
						}
					}
				}

				return minEvaluation;
			}
		}
	}
}
