using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class MinMax : SearchAlgorithm
	{
		readonly bool USE_QUIESCENCE_SEARCH = false;

		internal MinMax(ChessEngine chessEngine, Board board, MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(chessEngine, board, moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint depth)
		{
			_aboardSearch = false;

			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			if (_pieceManager.CurrentPieces.Color == MAXIMIZING_COLOR)
			{
				Search(_pieceManager.CurrentPieces, depth, true, depth);
			}
			else
			{
				Search(_pieceManager.CurrentPieces, depth, false, depth);
			}

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(depth, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, bool maximizingPlayer, uint maxDepth)
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
					return QuiescenceSearch(currentPlayerPieces, maximizingPlayer);
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

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			if (maximizingPlayer)
			{
				int maxEvaluation = MIN_VALUE;

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
				int minEvaluation = MAX_VALUE;

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

		int QuiescenceSearch(PieceSet currentPlayerPieces, bool maximizingPlayer)
		{
			if (_aboardSearch)
			{
				return ABOARD_VALUE;
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces, true));

			if (legalMoves.Count == 0) // no legal capture moves
			{
				return Evaluate();
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			if (maximizingPlayer)
			{
				int maxEvaluation = Evaluate();

				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					_moveExecutor.MakeMove(legalMove);

					int evaluation = QuiescenceSearch(nextDepthPlayerPieces, false);

					_moveExecutor.UndoMove(legalMove);

					_positionsEvaluated++;

					if (evaluation > maxEvaluation)
					{
						maxEvaluation = evaluation;
					}
				}

				return maxEvaluation;
			}
			else // minimizing player
			{
				int minEvaluation = Evaluate();

				for (int i = 0; i < legalMoves.Count; i++)
				{
					Move legalMove = legalMoves[i];

					_moveExecutor.MakeMove(legalMove);

					int evaluation = QuiescenceSearch(nextDepthPlayerPieces, true);

					_moveExecutor.UndoMove(legalMove);

					_positionsEvaluated++;

					if (evaluation < minEvaluation)
					{
						minEvaluation = evaluation;
					}
				}

				return minEvaluation;
			}
		}
	}
}
