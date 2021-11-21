using System.Collections.Generic;

public enum State { Undefinied, Playing, Checkmate, DrawByStalemate, DrawByFiftyMoveRule, DrawByRepetitions, TimeElapsed }
public enum ColorType {	Undefinied, White, Black }

public class ChessEngine
{
	int _halfmoveClock;
	int _fullmoveNumber;

	public Board Board { get; }
	PieceManager _pieceManager;
	MoveGenerator _moveGenerator;
	MoveExecutor _moveExecutor;
	SearchAlgorithm _minMax;
	SearchAlgorithm _alphaBeta;

	public Perft Perft { get; private set; }

	public ChessEngine() : this("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") { }

	public ChessEngine(string fen)
	{
		ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(fen);

		_halfmoveClock = extractedFENData.HalfMovesClock;
		_fullmoveNumber = extractedFENData.FullMovesNumber;

		Board = new Board(extractedFENData);
		_pieceManager = new PieceManager(Board, extractedFENData);
		_moveExecutor = new MoveExecutor(Board, _pieceManager, extractedFENData.PlayerToMoveColor);
		_moveGenerator = new MoveGenerator(Board, _moveExecutor);
		_minMax = new MinMax(_moveGenerator, _moveExecutor, _pieceManager);
		_alphaBeta = new AlphaBeta(_moveGenerator, _moveExecutor, _pieceManager);
		Perft = new Perft(_moveGenerator, _moveExecutor, _pieceManager);
	}

	public Move FindBestMove()
	{
		return _alphaBeta.FindBestMove();
	}

	public List<Move> GenerateLegalMoves(ColorType color)
	{
		PieceSet pieces = color == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
		return _moveGenerator.GenerateLegalMoves(pieces);
	}

	public State MakeMove(Move move)
	{
		_moveExecutor.MakeMove(move);

		List<Move> legalMoves = _moveGenerator.GenerateLegalMoves(_pieceManager.NextPieces);

		if (move.Piece.Type == PieceType.Pawn || move.EncounteredPiece != null)
			_halfmoveClock = 0;
		else
			_halfmoveClock++;

		if (_pieceManager.CurrentPieces.Color == ColorType.Black)
			_fullmoveNumber++;

		if (_halfmoveClock >= 50)
		{
			return State.DrawByFiftyMoveRule;
		}

		if (legalMoves.Count == 0)
		{
			if (_pieceManager.NextPieces.IsKingChecked())
			{
				return State.Checkmate;
			}
			else
			{
				return State.DrawByStalemate;
			}
		}

		_pieceManager.SwitchPlayer();

		return State.Playing;
	}
}
