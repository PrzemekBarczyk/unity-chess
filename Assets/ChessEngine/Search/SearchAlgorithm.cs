using System.Collections.Generic;

public abstract class SearchAlgorithm
{
	protected const int MAX_DEPTH = 4;

	protected const ColorType MAXIMIZING_COLOR = ColorType.Black;

	protected PieceManager _pieceManager;
	protected PieceSet _whitePieces;
	protected PieceSet _blackPieces;

	protected Move _bestMove;

	public SearchAlgorithm(PieceManager pieceManager)
	{
		_pieceManager = pieceManager;
		_whitePieces = _pieceManager.WhitePieces;
		_blackPieces = _pieceManager.BlackPieces;
	}

	public abstract Move FindBestMove();

	protected int Evaluate()
	{
		int whiteEvaluation = EvaluateSide(_whitePieces.AllPieces);
		int blackEvaluation = EvaluateSide(_blackPieces.AllPieces);

		int evaluation = blackEvaluation - whiteEvaluation;

		int evaluationModifier = MAXIMIZING_COLOR == ColorType.Black ? 1 : -1;

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
				score += piece.Value;

				if (piecesToEvaluate[0].Color == ColorType.Black)
				{
					score += piece.PositionsValues[piece.Square.Position.y, piece.Square.Position.x];
				}
				else // piece is white
				{
					score += piece.PositionsValues[Board.RANKS - 1 - piece.Square.Position.y, piece.Square.Position.x];
				}
			}
		}

		return score;
	}
}
