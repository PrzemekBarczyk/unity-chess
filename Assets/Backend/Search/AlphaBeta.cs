using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class AlphaBeta : SearchAlgorithm
	{
		readonly bool USE_QUIESCENCE_SEARCH = true;
		readonly bool USE_MOVE_ORDERING = true;

		internal AlphaBeta(ChessEngine chessEngine, Board board, MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(chessEngine, board, moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint depth)
		{
			_aboardSearch = false;

			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			if (_pieceManager.CurrentPieces.Color == MAXIMIZING_COLOR)
			{
				Search(_pieceManager.CurrentPieces, depth, MIN_VALUE, MAX_VALUE, true, depth);
			}
			else
			{
				Search(_pieceManager.CurrentPieces, depth, MIN_VALUE, MAX_VALUE, false, depth);
			}

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(depth, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, int alpha, int beta, bool maximizingPlayer, uint maxDepth)
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
					return QuiescenceSearch(currentPlayerPieces, alpha, beta, maximizingPlayer);
				}
				return Evaluate();
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

			if (legalMoves.Count == 0) // no legal moves
			{
				if (currentPlayerPieces.IsKingChecked())
				{
					return maximizingPlayer ? MATED_SCORE + (int)(maxDepth - depth) : -MATED_SCORE - (int)(maxDepth - depth);
				}
				return DRAW_SCORE;
			}

			if (USE_MOVE_ORDERING)
			{
				MoveOrderer.EvaluateAndSort(legalMoves);
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			if (maximizingPlayer)
			{
				int maxEvaluation = MIN_VALUE;

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
				int minEvaluation = MAX_VALUE;

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
			if (_aboardSearch)
			{
				return ABOARD_VALUE;
			}

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

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces, true));

			if (USE_MOVE_ORDERING)
			{
				MoveOrderer.EvaluateAndSort(legalMoves);
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			if (maximizingPlayer)
			{
				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					_moveExecutor.MakeMove(legalMove);

					int evaluation = QuiescenceSearch(nextDepthPlayerPieces, alpha, beta, false);

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
			else // minimizing player
			{
				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

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

				return beta;
			}
		}
	}
}
