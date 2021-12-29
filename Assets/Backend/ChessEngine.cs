using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public enum ColorType { Undefinied, White, Black }

namespace Backend
{
	public sealed class ChessEngine
	{
		internal uint HalfMoveClock { get; private set; }
		internal uint FullMoveNumber { get; private set; }

		public Stack<ulong> RepetitionHistory { get; private set; } = new Stack<ulong>(32);

		Board _board;

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

			_board = new Board(extractedFENData);

			_pieceManager = new PieceManager(_board, extractedFENData);

			_moveExecutor = new MoveExecutor(_board, _pieceManager, extractedFENData.PlayerToMoveColor);
			_moveGenerator = new MoveGenerator(_board, _moveExecutor);

			_minMax = new MinMax(_moveGenerator, _moveExecutor, _pieceManager);
			_negaMax = new NegaMax(_moveGenerator, _moveExecutor, _pieceManager);
			_alphaBeta = new AlphaBeta(_moveGenerator, _moveExecutor, _pieceManager);
			_negaBeta = new NegaBeta(_moveGenerator, _moveExecutor, _pieceManager);
			_negaBetaTT = new NegaBetaTT(this, _board, _moveGenerator, _moveExecutor, _pieceManager);

			Perft = new Perft(_moveGenerator, _moveExecutor, _pieceManager);

			RepetitionHistory.Push(_board.ZobristHash);
		}

		public Tuple<Move, SearchStatistics> FindBestMove(uint fixedSearchDepth)
		{
			return _negaBetaTT.FindBestMove(fixedSearchDepth);
		}

		public Tuple<Move, SearchStatistics> FindBestMove(float timeForSearchInMilliseconds)
		{
			Stopwatch timer = Stopwatch.StartNew();

			Tuple<Move, SearchStatistics> bestMoveDataThisIteration = new Tuple<Move, SearchStatistics>(new Move(), new SearchStatistics());
			Tuple<Move, SearchStatistics> bestMoveData = new Tuple<Move, SearchStatistics>(new Move(), new SearchStatistics());

			uint currentSearchDepth = 1;

			while (true) // search deeper until runs out of time
			{
				Thread t = new Thread(() => bestMoveDataThisIteration = _negaBetaTT.FindBestMove(currentSearchDepth));
				t.Start();

				while (t.IsAlive)
				{
					if (timer.ElapsedMilliseconds >= timeForSearchInMilliseconds)
					{
						_negaBetaTT.AbordSearch();
						t.Join();
						return bestMoveData;
					}
				}

				bestMoveData = bestMoveDataThisIteration;
				currentSearchDepth++;
			}
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

			if (moveToMake.Piece.Type == PieceType.Pawn || moveToMake.EncounteredPiece != null)
			{
				RepetitionHistory.Clear();
				HalfMoveClock = 0;
			}
			else
			{
				RepetitionHistory.Push(_board.ZobristHash);
				HalfMoveClock++;
			}

			if (moveToMake.Piece.Color == ColorType.Black)
			{
				FullMoveNumber++;
			}

			_pieceManager.SwitchPlayer();
		}

		public bool IsKingChecked(ColorType kingColor)
		{
			PieceSet pieces = kingColor == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
			return pieces.IsKingChecked();
		}

		internal bool IsMaterialSufficient(ColorType playerColor)
		{
			PieceSet pieces = playerColor == ColorType.White ? _pieceManager.WhitePieces : _pieceManager.BlackPieces;
			bool isMaterialSufficient = false;
			int counter = 0;
			foreach (var piece in pieces.AllPieces)
			{
				if (piece.IsAlive)
				{
					if (piece.Type == PieceType.Queen || piece.Type == PieceType.Rook || piece.Type == PieceType.Pawn)
					{
						isMaterialSufficient = true;
						break;
					}
					counter++;
				}
				if (counter > 2)
				{
					isMaterialSufficient = true;
					break;
				}
			}
			return isMaterialSufficient;
		}

		internal ulong ZobristHash()
		{
			return _board.ZobristHash;
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
																	  _board.EnPassantTarget?.Position, HalfMoveClock, FullMoveNumber));
		}
	}
}
