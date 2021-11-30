using System;
using System.Threading;
using System.Collections.Generic;

public enum State { Undefinied, Playing, Checkmate, DrawByStalemate, DrawByFiftyMoveRule, DrawByRepetitions, TimeElapsed }
public enum ColorType {	Undefinied, White, Black }

public sealed class ChessEngine
{
	public event Action<BoardStatistics> OnGameStart;
	public event HumanMoveHandler OnHumanMove;
	public event Action<SearchStatistics> OnBotMove;
	public event Action<ColorType> OnTurnStarted;
	public event Action<Move, BoardStatistics> OnTurnEnded;
	public event Action<State> OnGameEnded;

	public delegate Move HumanMoveHandler(List<Move> legalMoves);

	Dictionary<ulong, int> _repetitionHistory = new Dictionary<ulong, int>(16);

	State _state;

	uint _halfMoveClock;
	uint _fullMoveNumber;

	PlayerManager _playerManager;

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

	public ChessEngine(GameSettings gameSettings)
	{
		FENDataAdapter extractedFENData = FENConverter.FENToBoardPositionData(gameSettings.StartPositionInFEN);

		_state = State.Playing;

		_halfMoveClock = extractedFENData.HalfMovesClock;
		_fullMoveNumber = extractedFENData.FullMovesNumber;

		_playerManager = new PlayerManager(gameSettings.GameType, extractedFENData.PlayerToMoveColor);

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

	public Thread StartGame() // starts game in new thread
	{
		Thread engineThread = new Thread(() => GameLoop());
		engineThread.Start();
		return engineThread;
	}

	void GameLoop()
	{
		_repetitionHistory.Add(Board.ZobristHash, 1);
		OnGameStart?.Invoke(new BoardStatistics(_minMax.Evaluate(), Board.ZobristHash));

		while (_state == State.Playing)
		{
			OnTurnStarted?.Invoke(_playerManager.CurrentPlayer.Color);

			Move moveToMake;
			if (_playerManager.CurrentPlayer.Type == PlayerType.Human)
			{
				PieceSet humanPieces = _playerManager.CurrentPlayer.Color == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
				Move ? selectedMove = OnHumanMove?.Invoke(_moveGenerator.GenerateLegalMoves(humanPieces)); // send legal moves to GUI and wait for human to choose one
				moveToMake = selectedMove.Value;
			}
			else // bot turn
			{
				moveToMake = _alphaBeta.FindBestMove();
				OnBotMove?.Invoke(new SearchStatistics());
			}

			_moveExecutor.MakeMove(moveToMake);

			int evaluation = _minMax.Evaluate();
			ulong zobristKey = Board.ZobristHash;

			OnTurnEnded?.Invoke(moveToMake, new BoardStatistics(evaluation, zobristKey));

			if (moveToMake.Piece.Type == PieceType.Pawn || moveToMake.EncounteredPiece != null)
			{
				_halfMoveClock = 0;
			}
			else
			{
				_halfMoveClock++;
			}

			if (_pieceManager.CurrentPieces.Color == ColorType.Black)
			{
				_fullMoveNumber++;
			}

			CheckState();

			_playerManager.SwitchTurn();
			_pieceManager.SwitchPlayer();
		}

		OnGameEnded?.Invoke(_state);
	}

	void CheckState()
	{
		List<Move> legalMoves = _moveGenerator.GenerateLegalMoves(_pieceManager.NextPieces);

		if (_halfMoveClock >= 50)
		{
			_state = State.DrawByFiftyMoveRule;
			return;
		}

		try
		{
			_repetitionHistory.Add(Board.ZobristHash, 1);
		}
		catch (ArgumentException)
		{
			_repetitionHistory[Board.ZobristHash] = _repetitionHistory[Board.ZobristHash] + 1;
			if (_repetitionHistory[Board.ZobristHash] >= 3)
			{
				_state = State.DrawByRepetitions;
				return;
			}
		}

		if (legalMoves.Count == 0)
		{
			if (_pieceManager.NextPieces.IsKingChecked())
			{
				_state = State.Checkmate;
				return;
			}
			else
			{
				_state = State.DrawByStalemate;
				return;
			}
		}
	}

	public string FEN()
	{
		List<PieceData> pieces = new List<PieceData>(16);
		foreach (Piece piece in _pieceManager.BlackPieces.AllPieces)
			if (piece.IsAlive) pieces.Add(new PieceData(piece));
		foreach (Piece piece in _pieceManager.WhitePieces.AllPieces)
			if (piece.IsAlive) pieces.Add(new PieceData(piece));
		return FENConverter.BoardPositionToFEN(new FENDataAdapter(pieces, _playerManager.CurrentPlayer.Color,
																  _pieceManager.WhitePieces.CanKingCastleKingside,
																  _pieceManager.WhitePieces.CanKingCastleQueenside,
																  _pieceManager.BlackPieces.CanKingCastleKingside,
																  _pieceManager.BlackPieces.CanKingCastleQueenside,
																  Board.EnPassantTarget?.Position, _halfMoveClock, _fullMoveNumber));
	}
}
