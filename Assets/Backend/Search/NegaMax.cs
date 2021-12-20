using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class NegaMax : SearchAlgorithm
	{
		internal NegaMax(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint fixedDepthSearch)
		{
			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			Search(_pieceManager.CurrentPieces, fixedDepthSearch, fixedDepthSearch);

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(fixedDepthSearch, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, uint maxDepth)
		{
			if (depth == 0)
			{
				return Evaluate(currentPlayerPieces.Color);
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			if (legalMoves.Count == 0) // no legal moves
			{
				if (currentPlayerPieces.IsKingChecked())
					return -1000000 - (int)depth;
				return 0;
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			int maxEvaluation = -10000000;

			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				_moveExecutor.MakeMove(legalMove);

				int evaluation = -Search(nextDepthPlayerPieces, depth - 1, maxDepth);

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
	}
}
