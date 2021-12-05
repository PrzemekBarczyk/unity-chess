using System;
using System.Collections.Generic;
using System.Threading;

public enum State { Undefinied, Playing, Checkmate, DrawByStalemate, DrawByFiftyMoveRule, DrawByRepetitions, TimeElapsed }

public sealed class ChessProgram
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

	PlayerManager _playerManager;

	ChessEngine _chessEngine;

	public ChessProgram(GameSettings gameSettings)
	{
		FENDataAdapter extractedFENData = FENConverter.FENToBoardPositionData(gameSettings.StartPositionInFEN);

		_state = State.Playing;

		_playerManager = new PlayerManager(gameSettings.GameType, extractedFENData.PlayerToMoveColor);

		_chessEngine = new ChessEngine(gameSettings.StartPositionInFEN);
	}

	public Thread StartGame() // starts game in new thread
	{
		Thread engineThread = new Thread(() => GameLoop());
		engineThread.Start();
		return engineThread;
	}

	void GameLoop()
	{
		ulong startingZobristHash = _chessEngine.ZobristHash();
		int startingEvaluation = _chessEngine.Evaluation();

		_repetitionHistory.Add(startingZobristHash, 1);

		OnGameStart?.Invoke(new BoardStatistics(startingEvaluation, startingZobristHash));

		while (_state == State.Playing)
		{
			OnTurnStarted?.Invoke(_playerManager.CurrentPlayer.Color);

			Move moveToMake;
			if (_playerManager.CurrentPlayer.Type == PlayerType.Human)
			{
				Move? selectedMove = OnHumanMove?.Invoke(_chessEngine.GenerateLegalMoves(_playerManager.CurrentPlayer.Color)); // send legal moves to GUI and wait for human to choose one
				moveToMake = selectedMove.Value;
			}
			else // bot turn
			{
				moveToMake = _chessEngine.FindBestMove();
				OnBotMove?.Invoke(new SearchStatistics());
			}

			_chessEngine.MakeMove(moveToMake);

			int evaluation = _chessEngine.Evaluation();
			ulong zobristKey = _chessEngine.ZobristHash();

			OnTurnEnded?.Invoke(moveToMake, new BoardStatistics(evaluation, zobristKey));

			if (moveToMake.Piece.Type == PieceType.Pawn || moveToMake.EncounteredPiece != null)
			{
				_chessEngine.HalfMoveClock = 0;
			}
			else
			{
				_chessEngine.HalfMoveClock++;
			}

			if (_playerManager.CurrentPlayer.Color == ColorType.Black)
			{
				_chessEngine.FullMoveNumber++;
			}

			CheckState(zobristKey);

			_playerManager.SwitchTurn();
		}

		OnGameEnded?.Invoke(_state);
	}

	void CheckState(ulong zobristHash)
	{
		List<Move> legalMoves = _chessEngine.GenerateLegalMoves(_playerManager.NextPlayer.Color);

		if (_chessEngine.HalfMoveClock >= 50)
		{
			_state = State.DrawByFiftyMoveRule;
			return;
		}

		try
		{
			_repetitionHistory.Add(zobristHash, 1);
		}
		catch (ArgumentException)
		{
			_repetitionHistory[zobristHash] = _repetitionHistory[zobristHash] + 1;
			if (_repetitionHistory[zobristHash] >= 3)
			{
				_state = State.DrawByRepetitions;
				return;
			}
		}

		if (legalMoves.Count == 0)
		{
			if (_chessEngine.IsKingChecked(_playerManager.NextPlayer.Color))
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

	public string FEN() => _chessEngine.FEN();
}
