using System;
using System.Collections.Generic;
using System.Threading;

namespace Backend
{
	public enum State { Undefinied, Playing, Checkmate, DrawByStalemate, DrawByFiftyMoveRule, DrawByRepetitions, DrawByInsufficientMaterial, TimeElapsed }
	public enum GameType { HumanVsBot, BotVsHuman, HumanVsHuman, BotVsBot }

	public delegate Move HumanMoveHandler(List<Move> legalMoves);

	public sealed class ChessProgram
	{
		public event Action<BoardStatistics> OnGameStarted;
		public event Action<State> OnGameEnded;

		public event Action<ColorType, uint> OnTurnStarted;
		public event Action<Move, BoardStatistics> OnTurnEnded;

		public event HumanMoveHandler OnHumanTurnStarted;
		public event Action<SearchStatistics> OnBotSearchEnded;

		Dictionary<string, int> _repetitionHistory = new Dictionary<string, int>(64);
		uint _plyCounter = 1;

		PlayerManager _playerManager;
		ChessEngine _chessEngine;
		OpeningBook _book;

		bool _useBook = true;
		uint _fixedSearchedDepth = 5;
		bool _useIterativeDeepening = true;
		float _timerForSearch = 1000f; // in milliseconds

		public ChessProgram(string startPositionInFEN, GameType gameType, ColorType playerToMoveColor, string openingBook)
		{
			_playerManager = new PlayerManager(gameType, playerToMoveColor);

			_chessEngine = new ChessEngine(startPositionInFEN);

			_book = new OpeningBook(_chessEngine, openingBook);
		}

		public Thread StartGame() // starts game in new thread
		{
			Thread engineThread = new Thread(() => GameLoop());
			engineThread.Start();
			return engineThread;
		}

		void GameLoop()
		{
			int startingEvaluation = _chessEngine.Evaluation();
			string startingFEN = _chessEngine.FEN();
			ulong startingZobristHash = _chessEngine.ZobristHash();

			_repetitionHistory.Add(string.Join(" ", startingFEN.Split(), 0, 4), 1);

			OnGameStarted?.Invoke(new BoardStatistics(startingEvaluation, startingFEN, startingZobristHash));

			State state = State.Playing;
			while (state == State.Playing)
			{
				OnTurnStarted?.Invoke(_playerManager.CurrentPlayer.Color, _plyCounter++);

				Move moveToMake = new Move();
				if (_playerManager.CurrentPlayer.Type == PlayerType.Human)
				{
					Move? selectedMove = OnHumanTurnStarted?.Invoke(_chessEngine.GenerateLegalMoves(_playerManager.CurrentPlayer.Color)); // send legal moves to GUI and wait for human to choose one
					moveToMake = selectedMove.Value;
				}
				else // bot turn
				{
					if (_useBook)
					{
						string fenPos = _chessEngine.FEN();

						var movesFromBook = _book.FindEntry(fenPos.Substring(0, fenPos.Length - 4));

						if (movesFromBook == null)
						{
							_useBook = false;
						}
						else
						{
							moveToMake = SimplifiedAlgebraicNotation.LongSANToMove(_chessEngine, movesFromBook[new Random().Next(0, movesFromBook.Count)]);

							OnBotSearchEnded?.Invoke(new SearchStatistics(0, 0, 0, 0, 0));
						}
					}

					if (!_useBook)
					{
						Tuple<Move, SearchStatistics> moveToMakeWithStats;

						if (!_useIterativeDeepening)
						{
							moveToMakeWithStats = _chessEngine.FindBestMove(_fixedSearchedDepth);
						}
						else
						{
							moveToMakeWithStats = _chessEngine.FindBestMove(_timerForSearch);
						}

						moveToMake = moveToMakeWithStats.Item1;

						OnBotSearchEnded?.Invoke(moveToMakeWithStats.Item2);
					}
				}

				_chessEngine.MakeMove(moveToMake);

				int evaluation = _chessEngine.Evaluation();
				string fen = _chessEngine.FEN();
				ulong zobristKey = _chessEngine.ZobristHash();

				OnTurnEnded?.Invoke(moveToMake, new BoardStatistics(evaluation, fen, zobristKey));

				state = CheckState(fen);

				_playerManager.SwitchTurn();
			}

			OnGameEnded?.Invoke(state);
		}

		State CheckState(string fen)
		{
			string formattedFEN = string.Join(" ", fen.Split(), 0, 4); // removes half moves clock and full move number
			UnityEngine.Debug.Log(formattedFEN);

			List<Move> legalMoves = _chessEngine.GenerateLegalMoves(_playerManager.NextPlayer.Color);

			if (!_chessEngine.IsMaterialSufficient(_playerManager.NextPlayer.Color) && !_chessEngine.IsMaterialSufficient(_playerManager.CurrentPlayer.Color))
			{
				return State.DrawByInsufficientMaterial;
			}

			if (_chessEngine.HalfMoveClock >= 50)
			{
				return State.DrawByFiftyMoveRule;
			}

			try
			{
				_repetitionHistory.Add(formattedFEN, 1);
			}
			catch (ArgumentException)
			{
				_repetitionHistory[formattedFEN] = _repetitionHistory[formattedFEN] + 1;
				if (_repetitionHistory[formattedFEN] >= 3)
				{
					return State.DrawByRepetitions;
				}
			}

			if (legalMoves.Count == 0)
			{
				if (_chessEngine.IsKingChecked(_playerManager.NextPlayer.Color))
				{
					return State.Checkmate;
				}
				else
				{
					return State.DrawByStalemate;
				}
			}

			return State.Playing;
		}
	}
}
