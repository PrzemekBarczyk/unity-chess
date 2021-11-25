using System;
using System.Collections.Generic;

public sealed class NegaBeta : SearchAlgorithm
{
	bool useQuiescenceSearch = false;

	public NegaBeta(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

	public override Move FindBestMove()
	{
		Search(_pieceManager.CurrentPieces, MAX_DEPTH, -10000000, 10000000);

		return _bestMove;
	}

	public int Search(PieceSet currentPlayerPieces, int depth, int alpha, int beta)
	{
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
		for (int i = 0; i < legalMoves.Count; i++)
		{
			Move legalMove = legalMoves[i];

			_moveExecutor.MakeMove(legalMove);

			int evaluation = -Search(nextDepthPlayerPieces, depth - 1, -beta, -alpha);

			_moveExecutor.UndoMove(legalMove);

			if (evaluation > bestEvaluation)
			{
				bestEvaluation = evaluation;
				if (depth == MAX_DEPTH) _bestMove = legalMove;
			}

			alpha = Math.Max(alpha, bestEvaluation);

			if (alpha >= beta)
			{
				break;
			}
		}

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
