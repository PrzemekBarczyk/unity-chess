using System.Collections.Generic;

public class NegaMax : SearchAlgorithm
{
	public NegaMax(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager) : base(moveGenerator, moveExecutor, pieceManager) { }

	public override Move FindBestMove()
	{
		Search(_pieceManager.CurrentPieces, MAX_DEPTH);

		return _bestMove;
	}

	public int Search(PieceSet currentPlayerPieces, int depth)
	{
		if (depth == 0)
		{
			return Evaluate(currentPlayerPieces.Color);
		}

		List<Move> legalMoves = new List<Move>(_moveGenerator.GenerateLegalMoves(currentPlayerPieces));

		if (legalMoves.Count == 0) // no legal moves
		{
			if (currentPlayerPieces.IsKingChecked())
				return -10000000;
			return 0;
		}

		PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

		int maxEvaluation = -10000000;

		for (int i = 0; i < legalMoves.Count; i++)
		{
			Move legalMove = legalMoves[i];

			_moveExecutor.MakeMove(legalMove);

			int evaluation = -Search(nextDepthPlayerPieces, depth - 1);

			_moveExecutor.UndoMove(legalMove);

			if (evaluation > maxEvaluation)
			{
				maxEvaluation = evaluation;
				if (depth == MAX_DEPTH) _bestMove = legalMove;
			}
		}

		return maxEvaluation;
	}
}
