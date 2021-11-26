using System.Collections.Generic;

public enum State { Undefinied, Playing, Checkmate, DrawByStalemate, DrawByFiftyMoveRule, DrawByRepetitions, TimeElapsed }
public enum ColorType {	Undefinied, White, Black }

public sealed class ChessEngine
{
	uint _halfMoveClock;
	uint _fullMoveNumber;

	ColorType _sideToMove = ColorType.White;

	public Board Board { get; }
	PieceManager _pieceManager;
	MoveGenerator _moveGenerator;
	MoveExecutor _moveExecutor;
	SearchAlgorithm _minMax;
	SearchAlgorithm _negaMax;
	SearchAlgorithm _alphaBeta;
	SearchAlgorithm _negaBeta;
	SearchAlgorithm _negaBetaTT;

	public Perft Perft { get; private set; }

	public ChessEngine() : this("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") { }

	public ChessEngine(string fen)
	{
		FENDataAdapter extractedFENData = FENConverter.FENToBoardPositionData(fen);

		_halfMoveClock = extractedFENData.HalfMovesClock;
		_fullMoveNumber = extractedFENData.FullMovesNumber;

		Board = new Board(extractedFENData);
		_pieceManager = new PieceManager(Board, extractedFENData);
		_moveExecutor = new MoveExecutor(Board, _pieceManager, extractedFENData.PlayerToMoveColor);
		_moveGenerator = new MoveGenerator(Board, _moveExecutor);
		_minMax = new MinMax(_moveGenerator, _moveExecutor, _pieceManager);
		_negaMax = new NegaMax(_moveGenerator, _moveExecutor, _pieceManager);
		_alphaBeta = new AlphaBeta(_moveGenerator, _moveExecutor, _pieceManager);
		_negaBeta = new NegaBeta(_moveGenerator, _moveExecutor, _pieceManager);
		_negaBetaTT = new NegaBetaTT(_moveGenerator, _moveExecutor, _pieceManager, Board);
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
			_halfMoveClock = 0;
		else
			_halfMoveClock++;

		if (_pieceManager.CurrentPieces.Color == ColorType.Black)
			_fullMoveNumber++;

		if (_halfMoveClock >= 50)
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

	public int Evaluate()
	{
		return _minMax.Evaluate();
	}

	public string FEN()
	{
		List<PieceData> pieces = new List<PieceData>(16);
		foreach (Piece piece in _pieceManager.BlackPieces.AllPieces)
			pieces.Add(new PieceData(piece));
		foreach (Piece piece in _pieceManager.WhitePieces.AllPieces)
			pieces.Add(new PieceData(piece));
		return FENConverter.BoardPositionToFEN(new FENDataAdapter(pieces, _sideToMove,
																  _pieceManager.WhitePieces.CanKingCastleKingside,
																  _pieceManager.WhitePieces.CanKingCastleQueenside,
																  _pieceManager.BlackPieces.CanKingCastleKingside,
																  _pieceManager.BlackPieces.CanKingCastleQueenside,
																  Board.EnPassantTarget?.Position, _halfMoveClock, _fullMoveNumber));
	}
}
