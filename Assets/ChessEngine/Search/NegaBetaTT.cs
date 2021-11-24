using System;
using System.Collections.Generic;

public class NegaBetaTT : SearchAlgorithm
{
	const int TRANSPOSITION_TABLE_SIZE = 64000;

	TranspositionTable _transpositionTable;

	public NegaBetaTT(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager, Board board) : base(moveGenerator, moveExecutor, pieceManager)
	{
		_transpositionTable = new TranspositionTable(board, TRANSPOSITION_TABLE_SIZE);
	}

	public override Move FindBestMove()
	{
		Search(_pieceManager.CurrentPieces, MAX_DEPTH, -10000000, 10000000);

		return _bestMove;
	}

	public int Search(PieceSet currentPlayerPieces, int depth, int alpha, int beta)
	{
		int alphaOrig = alpha;

		Entry ttEntry = _transpositionTable.GetEntry();
		if (!Entry.IsEntryInvalid(ttEntry) && ttEntry.depth >= depth)
		{
			if (ttEntry.nodeType == TranspositionTable.EXACT)
			{
				if (depth == MAX_DEPTH) _bestMove = ttEntry.move;
				return ttEntry.evaluation;
			}
			if (ttEntry.nodeType == TranspositionTable.LOWER_BOUND && ttEntry.evaluation > alpha)
			{
				alpha = ttEntry.evaluation;
			}
			else if (ttEntry.nodeType == TranspositionTable.UPPER_BOUND && ttEntry.evaluation < beta)
			{
				beta = ttEntry.evaluation;
			}

			if (alpha >= beta)
			{
				return ttEntry.evaluation;
			}
		}

		if (depth == 0)
		{
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

			int evaluation = -Search(nextDepthPlayerPieces, depth - 1, -beta, -alpha);

			_moveExecutor.UndoMove(legalMove);

			if (evaluation > bestEvaluation)
			{
				bestEvaluation = evaluation;
				bestMoveInNode = legalMove;
				if (depth == MAX_DEPTH) _bestMove = legalMove;
			}

			alpha = Math.Max(alpha, bestEvaluation);

			if (alpha >= beta)
			{
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
}
