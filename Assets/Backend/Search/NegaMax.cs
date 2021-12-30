using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class NegaMax : SearchAlgorithm
	{
		readonly bool USE_QUIESCENCE_SEARCH = false;

		internal NegaMax(ChessEngine chessEngine, Board board, MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(chessEngine, board, moveGenerator, moveExecutor, pieceManager) { }

		internal override Tuple<Move, SearchStatistics> FindBestMove(uint depth)
		{
			_aboardSearch = false;

			_bestEvaluation = 0;
			_positionsEvaluated = 0;
			_cutoffs = 0;
			_transpositions = 0;

			Search(_pieceManager.CurrentPieces, depth, depth);

			return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(depth, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
		}

		internal int Search(PieceSet currentPlayerPieces, uint depth, uint maxDepth)
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
					return QuiescenceSearch(currentPlayerPieces);
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

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			int bestEvaluation = MIN_VALUE;

			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				_moveExecutor.MakeMove(legalMove);

				int evaluation = -Search(nextDepthPlayerPieces, depth - 1, maxDepth);

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
			}

			return bestEvaluation;
		}

		int QuiescenceSearch(PieceSet currentPlayerPieces)
		{
			if (_aboardSearch)
			{
				return ABOARD_VALUE;
			}

			List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces, true));

			if (legalMoves.Count == 0) // no legal capture moves
			{
				return Evaluate(currentPlayerPieces.Color);
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			int bestEvaluation = Evaluate(currentPlayerPieces.Color);

			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				_moveExecutor.MakeMove(legalMove);

				int evaluation = -QuiescenceSearch(nextDepthPlayerPieces);

				_moveExecutor.UndoMove(legalMove);

				_positionsEvaluated++;

				if (evaluation > bestEvaluation)
				{
					bestEvaluation = evaluation;
				}
			}

			return bestEvaluation;
		}
	}
}
