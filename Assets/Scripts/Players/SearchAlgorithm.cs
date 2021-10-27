using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotPlayer))]
public abstract class SearchAlgorithm : MonoBehaviour
{
	[SerializeField] [Min(1f)] protected int _maxDepth = 3;

	protected BotPlayer _botPlayer;

	protected PieceSet _whitePieces;
	protected PieceSet _blackPieces;

	protected MoveData _bestMove;

	protected void Start()
	{
		_botPlayer = GetComponent<BotPlayer>();

		_whitePieces = PieceManager.Instance.WhitePieces;
		_blackPieces = PieceManager.Instance.BlackPieces;
	}

	public abstract MoveData FindBestMove(PieceSet botPieces);

	protected int Evaluate(ColorType maximizingPlayerColor)
	{
		int whiteEvaluation = EvaluateSide(_whitePieces.AlivePieces());
		int blackEvaluation = EvaluateSide(_blackPieces.AlivePieces());

		int evaluation = blackEvaluation - whiteEvaluation;

		int evaluationModifier = maximizingPlayerColor == ColorType.Black ? 1 : -1;

		return evaluation * evaluationModifier;
	}

	public int EvaluateSide(List<Piece> alivePiecesToEvaluate)
	{
		int score = 0;

		foreach (Piece piece in alivePiecesToEvaluate)
		{
			score += piece.Value;

			if (alivePiecesToEvaluate[0].Color == ColorType.Black)
			{
				score += piece.PositionsValues[piece.Square.Position.y, piece.Square.Position.x];
			}
			else // piece is white
			{
				score += piece.PositionsValues[Board.RANKS - 1 - piece.Square.Position.y, piece.Square.Position.x];
			}
		}

		return score;
	}
}
