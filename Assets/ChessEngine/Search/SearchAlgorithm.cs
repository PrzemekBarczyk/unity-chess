using System;
using System.Collections.Generic;

public abstract class SearchAlgorithm
{
	protected const int MAX_DEPTH = 4;

	protected const ColorType MAXIMIZING_COLOR = ColorType.Black;

	protected MoveGenerator _moveGenerator;
	protected MoveExecutor _moveExecutor;
	protected PieceManager _pieceManager;
	protected PieceSet _whitePieces;
	protected PieceSet _blackPieces;

	protected Move _bestMove;
	protected int _bestEvaluation;
	protected uint _positionsEvaluated;
	protected uint _cutoffs;
	protected uint _transpositions;

	public SearchAlgorithm(MoveGenerator moveGenerator, MoveExecutor moveExecutor, PieceManager pieceManager)
	{
		_moveGenerator = moveGenerator;
		_moveExecutor = moveExecutor;
		_pieceManager = pieceManager;
		_whitePieces = _pieceManager.WhitePieces;
		_blackPieces = _pieceManager.BlackPieces;
	}

	public abstract Tuple<Move, SearchStatistics> FindBestMove();

	public int Evaluate(ColorType maximizingPlayer = MAXIMIZING_COLOR)
	{
		int whiteEvaluation = EvaluateSide(_whitePieces.AllPieces);
		int blackEvaluation = EvaluateSide(_blackPieces.AllPieces);

		int evaluation = blackEvaluation - whiteEvaluation;

		int evaluationModifier = maximizingPlayer == ColorType.Black ? 1 : -1;

		return evaluation * evaluationModifier;
	}

	public int EvaluateSide(List<Piece> piecesToEvaluate)
	{
		int score = 0;

		for (int i = 0; i < piecesToEvaluate.Count; i++)
		{
			Piece piece = piecesToEvaluate[i];

			if (piece.IsAlive)
			{
				score += PiecesValues.GetValue(piece);

				if (piecesToEvaluate[0].Color == ColorType.Black)
				{
					score += PiecesPositionValues.GetValue(piece)[piece.Square.Position.y, piece.Square.Position.x];
				}
				else // piece is white
				{
					score += PiecesPositionValues.GetValue(piece)[Board.RANKS - 1 - piece.Square.Position.y, piece.Square.Position.x];
				}
			}
		}

		return score;
	}
}
