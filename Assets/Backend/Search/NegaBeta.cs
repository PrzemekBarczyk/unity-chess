using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class NegaBeta : SearchAlgorithm
	{
		bool useQuiescenceSearch = false;

		internal NegaBeta(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint fixedDepthSearch)
		{
			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			Search(_pieceManager.CurrentPieces, fixedDepthSearch, -10000000, 10000000, fixedDepthSearch);

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(fixedDepthSearch, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, int alpha, int beta, uint maxDepth)
		{
			if (depth == 0)
			{
				if (useQuiescenceSearch) return QuiescenceSearch(currentPlayerPieces, alpha, beta);
				return Evaluate(currentPlayerPieces.Color);
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			if (legalMoves.Count == 0) // no legal moves
			{
				if (currentPlayerPieces.IsKingChecked())
					return -1000000 - (int)depth;
				return 0;
			}

			MoveOrderer.EvaluateAndSort(legalMoves);

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			int bestEvaluation = -10000000;
			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				_moveExecutor.MakeMove(legalMove);

				int evaluation = -Search(nextDepthPlayerPieces, depth - 1, -beta, -alpha, maxDepth);

				_moveExecutor.UndoMove(legalMove);

				_positionsEvaluated++;

				if (evaluation > bestEvaluation)
				{
					bestEvaluation = evaluation;
					if (depth == maxDepth)
					{
						_bestMove = legalMove;
						_bestEvaluation = bestEvaluation;
					}
				}

				alpha = Math.Max(alpha, bestEvaluation);

				if (alpha >= beta)
				{
					_cutoffs++;
					break;
				}
			}

			return bestEvaluation;
		}

		int QuiescenceSearch(PieceSet currentPlayerPieces, int alpha, int beta)
		{
			int standPat = Evaluate(currentPlayerPieces.Color);

			if (standPat >= beta)
			{
				return beta;
			}
			if (alpha < standPat)
			{
				alpha = standPat;
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				if (legalMove.EncounteredPiece != null)
				{
					_moveExecutor.MakeMove(legalMove);

					int evaluation = -QuiescenceSearch(nextDepthPlayerPieces, -beta, -alpha);

					_moveExecutor.UndoMove(legalMove);

					_positionsEvaluated++;

					if (evaluation >= beta)
					{
						return beta;
					}
					if (evaluation > alpha)
					{
						alpha = evaluation;
					}
				}
			}

			return alpha;
		}
	}
}
