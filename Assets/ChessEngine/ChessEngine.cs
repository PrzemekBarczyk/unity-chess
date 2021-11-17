using System.Collections.Generic;

public enum State { Undefinied, Playing, Checkmate, Draw, TimeElapsed }
public enum ColorType {	Undefinied, White, Black }

public class ChessEngine
{
	Board _board;
	PieceManager _pieceManager;
	MoveGenerator _moveGenerator;
	SearchAlgorithm _minMax;

	public Perft Perft { get; private set; }

	public ChessEngine() : this("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") { }

	public ChessEngine(string fen)
	{
		ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(fen);

		_board = new Board(extractedFENData);
		_pieceManager = new PieceManager(_board, extractedFENData);
		_moveGenerator = new MoveGenerator(_board);
		_minMax = new MinMax(_moveGenerator, _pieceManager);
		Perft = new Perft(_moveGenerator, _pieceManager);
	}

	public Move FindBestMove()
	{
		return _minMax.FindBestMove();
	}

	public List<Move> GenerateLegalMoves(ColorType color)
	{
		PieceSet pieces = color == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
		return _moveGenerator.GenerateLegalMoves(pieces);
	}

	public State MakeMove(Move move)
	{
		move.Piece.Move(move);

		List<Move> legalMoves = _moveGenerator.GenerateLegalMoves(_pieceManager.NextPieces);

		if (legalMoves.Count == 0)
		{
			if (_pieceManager.NextPieces.King.IsChecked())
			{
				return State.Checkmate;
			}
			else
			{
				return State.Draw;
			}
		}

		_pieceManager.SwitchPlayer();

		return State.Playing;
	}
}
