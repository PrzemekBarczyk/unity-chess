using System;
using System.Collections.Generic;

public sealed class NegaBetaTT : SearchAlgorithm
{
	bool useQuiescenceSearch = false;

	const int TRANSPOSITION_TABLE_SIZE = 64000;

	TranspositionTable _transpositionTable;

	public NegaBetaTT(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager, Board board) : base(moveGenerator, moveExecutor, pieceManager)
	{
		_transpositionTable = new TranspositionTable(board, TRANSPOSITION_TABLE_SIZE);
	}

	public override Tuple<Move, SearchStatistics> FindBestMove(uint fixedSearchDepth)
	{
		_aboordSearch = false;

		_bestEvaluation = 0;
		_positionsEvaluated = 0;
		_cutoffs = 0;
		_transpositions = 0;

		Search(_pieceManager.CurrentPieces, fixedSearchDepth, -10000000, 10000000, fixedSearchDepth);

		return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(fixedSearchDepth, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
	}

	public int Search(PieceSet currentPlayerPieces, uint depth, int alpha, int beta, uint maxDepth)
	{
		if (_aboordSearch)
		{
			return 0;
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
			if (useQuiescenceSearch) return QuiescenceSearch(currentPlayerPieces, alpha, beta);
			return Evaluate(currentPlayerPieces.Color);
		}

		List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

		if (legalMoves.Count == 0) // no legal moves
		{
			if (currentPlayerPieces.IsKingChecked())
				return -1000000;
			return 0;
		}

		MoveOrderer.EvaluateAndSort(legalMoves);

		PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

		int bestEvaluation = -10000000;
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
			nodeType = TranspositionTable.UPPER_BOUND;
		else if (bestEvaluation >= beta)
			nodeType = TranspositionTable.LOWER_BOUND;
		else
			nodeType = TranspositionTable.EXACT;

		_transpositionTable.StoreEntry(depth, bestEvaluation, nodeType, bestMoveInNode);

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
