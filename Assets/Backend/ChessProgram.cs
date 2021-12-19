using System;
using System.Collections.Generic;
using System.Threading;

public enum State { Undefinied, Playing, Checkmate, DrawByStalemate, DrawByFiftyMoveRule, DrawByRepetitions, DrawByInsufficientMaterial, TimeElapsed }

namespace Backend
{
	public sealed class ChessProgram
	{
		public event Action<BoardStatistics> OnGameStart;
		public event HumanMoveHandler OnHumanMove;
		public event Action<SearchStatistics> OnBotMove;
		public event Action<ColorType> OnTurnStarted;
		public event Action<Move, BoardStatistics> OnTurnEnded;
		public event Action<State> OnGameEnded;

		public delegate Move HumanMoveHandler(List<Move> legalMoves);

		Dictionary<string, int> _repetitionHistory = new Dictionary<string, int>(32);

		State _state;

		PlayerManager _playerManager;

		ChessEngine _chessEngine;

		bool _useBook = true;
		OpeningBook _book;

		uint _fixedSearchedDepth = 5;
		bool _useIterativeDeepening = true;
		float _timerForSearch = 1000f; // in milliseconds

		public ChessProgram(GameSettings gameSettings, string openingBook)
		{
			FENDataAdapter extractedFENData = FENConverter.FENToBoardPositionData(gameSettings.StartPositionInFEN);

			_state = State.Playing;

			_playerManager = new PlayerManager(gameSettings.GameType, extractedFENData.PlayerToMoveColor);

			_chessEngine = new ChessEngine(gameSettings.StartPositionInFEN);

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
			ulong startingZobristHash = _chessEngine.ZobristHash();
			string startingFEN = _chessEngine.FEN();
			int startingEvaluation = _chessEngine.Evaluation();

			_repetitionHistory.Add(startingFEN.Remove(startingFEN.Length - 4, 4), 1);

			OnGameStart?.Invoke(new BoardStatistics(startingEvaluation, startingFEN, startingZobristHash));

			while (_state == State.Playing)
			{
				OnTurnStarted?.Invoke(_playerManager.CurrentPlayer.Color);

				Move moveToMake = new Move();
				if (_playerManager.CurrentPlayer.Type == PlayerType.Human)
				{
					Move? selectedMove = OnHumanMove?.Invoke(_chessEngine.GenerateLegalMoves(_playerManager.CurrentPlayer.Color)); // send legal moves to GUI and wait for human to choose one
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

							OnBotMove?.Invoke(new SearchStatistics(0, 0, 0, 0, 0));
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

						OnBotMove?.Invoke(moveToMakeWithStats.Item2);
					}
				}

				_chessEngine.MakeMove(moveToMake);

				int evaluation = _chessEngine.Evaluation();
				string fen = _chessEngine.FEN();
				ulong zobristKey = _chessEngine.ZobristHash();

				OnTurnEnded?.Invoke(moveToMake, new BoardStatistics(evaluation, fen, zobristKey));

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

				CheckState(fen);

				_playerManager.SwitchTurn();
			}

			OnGameEnded?.Invoke(_state);
		}

		void CheckState(string fen)
		{
			string formattedFEN = fen.Remove(fen.Length - 4, 4);

			List<Move> legalMoves = _chessEngine.GenerateLegalMoves(_playerManager.NextPlayer.Color);

			if (!_chessEngine.IsMaterialSufficient(_playerManager.NextPlayer.Color) && !_chessEngine.IsMaterialSufficient(_playerManager.CurrentPlayer.Color))
			{
				_state = State.DrawByInsufficientMaterial;
				return;
			}

			if (_chessEngine.HalfMoveClock >= 50)
			{
				_state = State.DrawByFiftyMoveRule;
				return;
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
	}
}
