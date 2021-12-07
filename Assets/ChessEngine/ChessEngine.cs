using System;
using System.Collections.Generic;

public enum ColorType { Undefinied, White, Black }

public sealed class ChessEngine
{
	internal uint HalfMoveClock;
	internal uint FullMoveNumber;

	internal Board Board { get; }

	PieceManager _pieceManager;

	MoveGenerator _moveGenerator;
	MoveExecutor _moveExecutor;

	SearchAlgorithm _minMax;
	SearchAlgorithm _negaMax;
	SearchAlgorithm _alphaBeta;
	SearchAlgorithm _negaBeta;
	SearchAlgorithm _negaBetaTT;

	public Perft Perft { get; private set; }

	public ChessEngine(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
	{
		FENDataAdapter extractedFENData = FENConverter.FENToBoardPositionData(fen);

		HalfMoveClock = extractedFENData.HalfMovesClock;
		FullMoveNumber = extractedFENData.FullMovesNumber;

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

	public Tuple<Move, SearchStatistics> FindBestMove()
	{
		return _alphaBeta.FindBestMove();
	}

	public List<Move> GenerateLegalMoves()
	{
		return _moveGenerator.GenerateLegalMoves(_pieceManager.CurrentPieces);
	}

	public List<Move> GenerateLegalMoves(ColorType player)
	{
		PieceSet pieces = player == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
		return _moveGenerator.GenerateLegalMoves(pieces);
	}

	public void MakeMove(Move moveToMake)
	{
		_moveExecutor.MakeMove(moveToMake);
		_pieceManager.SwitchPlayer();
	}

	public bool IsKingChecked(ColorType kingColor)
	{
		PieceSet pieces = kingColor == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
		return pieces.IsKingChecked();
	}


	internal ulong ZobristHash()
	{
		return Board.ZobristHash;
	}

	internal int Evaluation()
	{
		return _minMax.Evaluate();
	}

	public string FEN()
	{
		List<PieceData> alivePieces = new List<PieceData>(16);

		foreach (Piece piece in _pieceManager.BlackPieces.AllPieces)
		{
			if (piece.IsAlive)
				alivePieces.Add(new PieceData(piece));
		}
		foreach (Piece piece in _pieceManager.WhitePieces.AllPieces)
		{
			if (piece.IsAlive)
				alivePieces.Add(new PieceData(piece));
		}

		return FENConverter.BoardPositionToFEN(new FENDataAdapter(alivePieces, _pieceManager.CurrentPieces.Color,
																  _pieceManager.WhitePieces.CanKingCastleKingside,
																  _pieceManager.WhitePieces.CanKingCastleQueenside,
																  _pieceManager.BlackPieces.CanKingCastleKingside,
																  _pieceManager.BlackPieces.CanKingCastleQueenside,
																  Board.EnPassantTarget?.Position, HalfMoveClock, FullMoveNumber));
	}
}
