using System;
using System.Collections.Generic;

public class AlphaBeta : SearchAlgorithm
{
	public AlphaBeta(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

	public override Move FindBestMove()
	{
		if (_pieceManager.CurrentPieces.Color == MAXIMIZING_COLOR)
		{
			Search(_pieceManager.CurrentPieces, MAX_DEPTH, -10000000, 10000000, true);
		}
		else
		{
			Search(_pieceManager.CurrentPieces, MAX_DEPTH, -10000000, 10000000, false);
		}

		return _bestMove;
	}

	public int Search(PieceSet currentPlayerPieces, int depth, int alpha, int beta, bool maximizingPlayer)
	{
		if (depth == 0)
		{
			return Evaluate();
		}

		List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

		if (legalMoves.Count == 0) // no legal moves
		{
			if (currentPlayerPieces.IsKingChecked())
				return maximizingPlayer ? -10000000 : 10000000;
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

				int evaluation = Search(nextDepthPlayerPieces, depth - 1, alpha, beta, false);

				_moveExecutor.UndoMove(legalMove);

				if (evaluation > maxEvaluation)
				{
					maxEvaluation = evaluation;
					if (depth == MAX_DEPTH) _bestMove = legalMove;
				}

				alpha = Math.Max(alpha, maxEvaluation);

				if (alpha >= beta)
				{
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

				int evaluation = Search(nextDepthPlayerPieces, depth - 1, alpha, beta, true);

				_moveExecutor.UndoMove(legalMove);

				if (evaluation < minEvaluation)
				{
					minEvaluation = evaluation;
					if (depth == MAX_DEPTH) _bestMove = legalMove;
				}

				beta = Math.Min(beta, minEvaluation);

				if (alpha >= beta)
				{
					break;
				}
			}

			return minEvaluation;
		}
	}
}
