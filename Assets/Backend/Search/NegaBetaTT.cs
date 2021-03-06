using System;
using System.Collections.Generic;

namespace Backend
{
	internal sealed class NegaBetaTT : SearchAlgorithm
	{
		readonly bool USE_QUIESCENCE_SEARCH = true;
		readonly bool USE_MOVE_ORDERING = true;
		const int TRANSPOSITION_TABLE_SIZE = 64000;

		TranspositionTable _transpositionTable;

		internal NegaBetaTT(ChessEngine chessEngine, Board board, MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(chessEngine, board, moveGenerator, moveExecutor, pieceManager)
		{
			_transpositionTable = new TranspositionTable(board, TRANSPOSITION_TABLE_SIZE);

			_chessEngine = chessEngine;
			_board = board;
		}

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

			int alphaOrig = alpha;

			Entry ttEntry = _transpositionTable.GetEntry();

			if (!Entry.IsEntryInvalid(ttEntry) && ttEntry.depth >= depth)
			{
				if (ttEntry.nodeType == TranspositionTable.EXACT)
				{
					_transpositions++;
					if (depth == maxDepth)
					{
						_bestMove = ttEntry.move;
						_bestEvaluation = ttEntry.evaluation;
					}
					return ttEntry.evaluation;
				}

				if (ttEntry.nodeType == TranspositionTable.LOWER_BOUND && ttEntry.evaluation > alpha)
				{
					_transpositions++;
					alpha = ttEntry.evaluation;
				}
				else if (ttEntry.nodeType == TranspositionTable.UPPER_BOUND && ttEntry.evaluation < beta)
				{
					_transpositions++;
					beta = ttEntry.evaluation;
				}

				if (alpha >= beta)
				{
					_transpositions++;
					return ttEntry.evaluation;
				}
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
				MoveOrderer.EvaluateAndSort(legalMoves, true, _transpositionTable);
			}

			PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

			int bestEvaluation = MIN_VALUE;
			Move bestMoveInNode = new Move();

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
					bestMoveInNode = legalMove;

					if (depth == maxDepth)
					{
						_bestMove = bestMoveInNode;
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

			int nodeType;

			if (bestEvaluation <= alphaOrig)
			{
				nodeType = TranspositionTable.UPPER_BOUND;
			}
			else if (bestEvaluation >= beta)
			{
				nodeType = TranspositionTable.LOWER_BOUND;
			}
			else
			{
				nodeType = TranspositionTable.EXACT;
			}

			_transpositionTable.StoreEntry(depth, bestEvaluation, nodeType, bestMoveInNode);

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
			if (standPat > alpha)
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
