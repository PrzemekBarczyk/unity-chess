using System;
using System.Collections.Generic;

public sealed class MinMax : SearchAlgorithm
{
    public MinMax(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

    public override Tuple<Move, SearchStatistics> FindBestMove()
    {
		_bestEvaluation = 0;
		_positionsEvaluated = 0;
		_cutoffs = 0;
		_transpositions = 0;

		if (_pieceManager.CurrentPieces.Color == MAXIMIZING_COLOR)
		{
			Search(_pieceManager.CurrentPieces, MAX_DEPTH, true);
		}
		else
		{
			Search(_pieceManager.CurrentPieces, MAX_DEPTH, false);
		}

        return new Tuple<Move, SearchStatistics>(_bestMove, new SearchStatistics(MAX_DEPTH, _bestEvaluation, _positionsEvaluated, _cutoffs, _transpositions));
	}

    public int Search(PieceSet currentPlayerPieces, int depth, bool maximizingPlayer)
    {
		if (depth == 0)
		{
			return Evaluate();
		}

		List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

		if (legalMoves.Count == 0) // no legal moves
		{
			if (currentPlayerPieces.IsKingChecked())
				return maximizingPlayer ? -1000000 : 1000000;
			return 0;
		}

		PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

		if (maximizingPlayer)
		{
			int maxEvaluation = -10000000;

			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				_moveExecutor.MakeMove(legalMove);

				int evaluation = Search(nextDepthPlayerPieces, depth - 1, false);

				_moveExecutor.UndoMove(legalMove);

				_positionsEvaluated++;

				if (evaluation > maxEvaluation)
				{
					maxEvaluation = evaluation;
					if (depth == MAX_DEPTH)
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
			int minEvaluation = 10000000;

			for (int i = 0; i < legalMoves.Count; i++)
			{
				Move legalMove = legalMoves[i];

				_moveExecutor.MakeMove(legalMove);

				int evaluation = Search(nextDepthPlayerPieces, depth - 1, true);

				_moveExecutor.UndoMove(legalMove);

				_positionsEvaluated++;

				if (evaluation < minEvaluation)
				{
					minEvaluation = evaluation;
					if (depth == MAX_DEPTH)
					{
						_bestMove = legalMove;
						_bestEvaluation = minEvaluation;
					}
				}
			}

			return minEvaluation;
		}
	}
}
