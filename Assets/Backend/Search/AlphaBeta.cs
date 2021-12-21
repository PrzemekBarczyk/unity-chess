using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class AlphaBeta : SearchAlgorithm
	{
		readonly bool USE_QUIESCENCE_SEARCH = false;

		internal AlphaBeta(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint fixedSearchDepth)
		{
			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			if (_pieceManager.CurrentPieces.Color == MAXIMIZING_COLOR)
			{
				Search(_pieceManager.CurrentPieces, fixedSearchDepth, -10000000, 10000000, true, fixedSearchDepth);
			}
			else
			{
				Search(_pieceManager.CurrentPieces, fixedSearchDepth, -10000000, 10000000, false, fixedSearchDepth);
			}

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(fixedSearchDepth, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, int alpha, int beta, bool maximizingPlayer, uint maxDepth)
		{
			if (depth == 0)
			{
				if (USE_QUIESCENCE_SEARCH) return QuiescenceSearch(currentPlayerPieces, alpha, beta, maximizingPlayer);
				return Evaluate();
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			if (legalMoves.Count == 0) // no legal moves
			{
				if (currentPlayerPieces.IsKingChecked())
					return maximizingPlayer ? -1000000 - (int)depth : 1000000 + (int)depth;
				return 0;
			}

			MoveOrderer.EvaluateAndSort(legalMoves);

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			if (maximizingPlayer)
			{
				int maxEvaluation = -10000000;

				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					_moveExecutor.MakeMove(legalMove);

					int evaluation = Search(nextDepthPlayerPieces, depth - 1, alpha, beta, false, maxDepth);

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

					alpha = Math.Max(alpha, maxEvaluation);

					if (alpha >= beta)
					{
						_cutoffs++;
						break;
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

					int evaluation = Search(nextDepthPlayerPieces, depth - 1, alpha, beta, true, maxDepth);

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

					beta = Math.Min(beta, minEvaluation);

					if (alpha >= beta)
					{
						_cutoffs++;
						break;
					}
				}

				return minEvaluation;
			}
		}

		int QuiescenceSearch(PieceSet currentPlayerPieces, int alpha, int beta, bool maximizingPlayer)
		{
			int standPat = Evaluate();

			if (maximizingPlayer)
			{
				if (standPat >= beta)
				{
					return beta;
				}
				if (standPat > alpha)
				{
					alpha = standPat;
				}
			}
			else // minimazing player
			{
				if (standPat <= alpha)
				{
					return alpha;
				}
				if (standPat < beta)
				{
					beta = standPat;
				}
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			if (maximizingPlayer)
			{
				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					if (legalMove.EncounteredPiece != null)
					{
						_moveExecutor.MakeMove(legalMove);

						int evaluation = QuiescenceSearch(nextDepthPlayerPieces, alpha, beta, false);

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
			else // minimizing player
			{
				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					if (legalMove.EncounteredPiece != null)
					{
						_moveExecutor.MakeMove(legalMove);

						int evaluation = QuiescenceSearch(nextDepthPlayerPieces, alpha, beta, true);

						_moveExecutor.UndoMove(legalMove);

						_positionsEvaluated++;

						if (evaluation <= alpha)
						{
							return alpha;
						}
						if (evaluation < beta)
						{
							beta = evaluation;
						}
					}
				}

				return beta;
			}
		}
	}
}