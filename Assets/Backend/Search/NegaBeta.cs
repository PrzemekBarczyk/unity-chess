using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class NegaBeta : SearchAlgorithm
	{
		readonly bool USE_QUIESCENCE_SEARCH = true;
		readonly bool USE_MOVE_ORDERING = true;

		internal NegaBeta(ChessEngine chessEngine, Board board, MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(chessEngine, board, moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint depth)
		{
			_aboardSearch = false;

			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			Search(_pieceManager.CurrentPieces, depth, MIN_VALUE, MAX_VALUE, depth);

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(depth, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, int alpha, int beta, uint maxDepth)
		{
			if (_aboardSearch)
			{
				return ABOARD_VALUE;
			}

			if (depth < maxDepth && _chessEngine.RepetitionHistory.Contains(_board.ZobristHash)) // simplified draw detection
			{
				return DRAW_SCORE;
			}

			if (depth == 0)
			{
				if (USE_QUIESCENCE_SEARCH)
				{
					return QuiescenceSearch(currentPlayerPieces, alpha, beta);
				}
				return Evaluate(currentPlayerPieces.Color);
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			if (legalMoves.Count == 0) // no legal moves
			{
				if (currentPlayerPieces.IsKingChecked())
				{
					return MATED_SCORE + (int)(maxDepth - depth);
				}
				return DRAW_SCORE;
			}

			if (USE_MOVE_ORDERING)
			{
				MoveOrderer.EvaluateAndSort(legalMoves);
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			int bestEvaluation = MIN_VALUE;

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
			if (_aboardSearch)
			{
				return ABOARD_VALUE;
			}

			int standPat = Evaluate(currentPlayerPieces.Color);

			if (standPat >= beta)
			{
				return beta;
			}
			if (alpha < standPat)
			{
				alpha = standPat;
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces, true));

			if (USE_MOVE_ORDERING)
			{
				MoveOrderer.EvaluateAndSort(legalMoves);
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				_moveExecutor.MakeMove(legalMove);

				int evaluation = -QuiescenceSearch(nextDepthPlayerPieces, -beta, -alpha);

				_moveExecutor.UndoMove(legalMove);

				_positionsEvaluated++;

				if (evaluation >= beta)
				{
					_cutoffs++;
					return beta;
				}
				if (evaluation > alpha)
				{
					alpha = evaluation;
				}
			}

			return alpha;
		}
	}
}
