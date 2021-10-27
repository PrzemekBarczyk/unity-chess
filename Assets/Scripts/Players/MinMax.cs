using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotPlayer))]
public class MinMax : SearchAlgorithm
{
    new void Start()
    {
        base.Start();
    }

    public override MoveData FindBestMove(PieceSet botPieces)
    {
        Search(botPieces, _maxDepth, true);

        return _bestMove;
    }

    public int Search(PieceSet currentPlayerPieces, int depth, bool maximizingPlayer)
    {
		if (depth == 0)
		{
			return Evaluate(maximizingPlayerColor: _botPlayer.Color);
		}

		currentPlayerPieces.GenerateLegalMoves();
		List<MoveData> allValidMoves = currentPlayerPieces.GetLegalMoves();

		if (allValidMoves.Count == 0) // no legal moves
		{
			if (currentPlayerPieces.King.IsChecked())
				return maximizingPlayer ? -10000000 : 10000000;
			return 0;
		}

		PieceSet nextDepthPlayerPieces = currentPlayerPieces == _whitePieces ? _blackPieces : _whitePieces;

		if (maximizingPlayer)
		{
			int maxEvaluation = -10000000;

			foreach (MoveData move in allValidMoves)
			{
				move.Piece.Move(move);

				int evaluation = Search(nextDepthPlayerPieces, depth - 1, false);

				if (evaluation > maxEvaluation)
				{
					maxEvaluation = evaluation;
					if (currentPlayerPieces == _botPlayer.Pieces && depth == _maxDepth) _bestMove = move;
				}

				move.Piece.UndoMove(move);
			}

			return maxEvaluation;
		}
		else // minimizing player
		{
			int minEvaluation = 10000000;

			foreach (MoveData move in allValidMoves)
			{
				move.Piece.Move(move);

				int evaluation = Search(nextDepthPlayerPieces, depth - 1, true);

				if (evaluation < minEvaluation)
				{
					minEvaluation = evaluation;
					if (currentPlayerPieces == _botPlayer.Pieces && depth == _maxDepth) _bestMove = move;
				}

				move.Piece.UndoMove(move);
			}

			return minEvaluation;
		}
	}
}
