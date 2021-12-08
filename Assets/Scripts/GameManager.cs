using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] TextAsset _openingBook;

    public State State { get; private set; }

    Move? _lastMove;

    ChessProgram _chessProgram;
    Thread _chessEngineThread;

    [SerializeField] MoveSelector _moveSelector;

    GraphicalBoard _graphicalBoard;

    [SerializeField] Clock _whitePlayerClock;
    [SerializeField] Clock _blackPlayerClock;

    [SerializeField] CapturedPieces _whitePlayerCapturedPieces;
    [SerializeField] CapturedPieces _blackPlayerCapturedPieces;

    HUD _hud;

    void Start()
    {
        _graphicalBoard = GraphicalBoard.Instance;

        _hud = FindObjectOfType<HUD>(true);

        _graphicalBoard.CreateBoard();
    }

    public void StartGame(GameSettings gameSettings) // called after pressing PLAY button
    {
        FENDataAdapter extractedFENData;
        try
        {
            extractedFENData = FENConverter.FENToBoardPositionData(gameSettings.StartPositionInFEN);
        }
        catch (FormatException e)
		{
            Debug.Log(e.Message);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
		}

        _graphicalBoard.CreatePieces(extractedFENData.Pieces);

        _whitePlayerClock.SetUp(gameSettings.UseClocks, gameSettings.BaseTime, gameSettings.AddedTime);
        _blackPlayerClock.SetUp(gameSettings.UseClocks, gameSettings.BaseTime, gameSettings.AddedTime);

        _whitePlayerClock.OnTimeElapsed += OnTimeElaped;
        _blackPlayerClock.OnTimeElapsed += OnTimeElaped;

        _chessProgram = new ChessProgram(gameSettings, _openingBook.text);
        _chessProgram.OnGameStart += OnGameStart;
        _chessProgram.OnTurnStarted += OnTurnStarted;
        _chessProgram.OnTurnEnded += OnTurnEnded;
        _chessProgram.OnGameEnded += OnGameEnded;
        _chessProgram.OnHumanMove += OnHumanMove;
        _chessProgram.OnBotMove += OnBotMove;
        _chessProgram.StartGame();

        State = State.Playing;
    }

    public void OnTimeElaped()
	{
        GameOver(State.TimeElapsed);
	}

    public void OnGameStart(BoardStatistics boardStatistics)
	{
        ThreadDispatcher.RunOnMainThread(() =>
        {
            _hud.ChangeZobristKey(boardStatistics.ZobristKey);
            _hud.ChangeEvaluation(boardStatistics.Evaluation);
            _hud.ChangeFEN(boardStatistics.FEN);
        });
    }

    public void OnTurnStarted(ColorType color)
	{
        ThreadDispatcher.RunOnMainThread(() =>
        {
            if (color == ColorType.White)
		    {
                _whitePlayerClock.Run();
		    }
            else
		    {
                _blackPlayerClock.Run();
		    }
        });
    }

    public void OnTurnEnded(Move move, BoardStatistics boardStatistics)
	{
        ThreadDispatcher.RunOnMainThread(() =>
        {
            if (move.Piece.Color == ColorType.White)
            {
                _whitePlayerClock.Stop();
            }
            else
            {
                _blackPlayerClock.Stop();
            }

            if (move.EncounteredPiece != null)
			{
                if (move.EncounteredPiece.Color == ColorType.White)
				{
                    _whitePlayerCapturedPieces.AddCaptureIcon(move.EncounteredPiece.Type);
				}
                else
				{
                    _blackPlayerCapturedPieces.AddCaptureIcon(move.EncounteredPiece.Type);
                }
			}

            _graphicalBoard.UpdateBoard(move, _lastMove);
            _lastMove = move;

            _hud.ChangeZobristKey(boardStatistics.ZobristKey);
            _hud.ChangeEvaluation(boardStatistics.Evaluation);
            _hud.ChangeFEN(boardStatistics.FEN);
        });
    }

    public void OnGameEnded(State result)
	{
        ThreadDispatcher.RunOnMainThread(() =>
        {
            GameOver(result);
        });
    }

    void GameOver(State result)
	{
        State = result;
        Time.timeScale = 0f;
        if (_chessEngineThread != null) _chessEngineThread.Abort();
        Debug.Log("Game over: " + result);
    }

    public Move OnHumanMove(List<Move> legalMoves)
	{
        _moveSelector.SetLegalMoves(legalMoves);

        while (!_moveSelector.IsMoveSelected())
            continue;

        return _moveSelector.GetSelectedMove();
    }

    public void OnBotMove(SearchStatistics searchStatistics)
	{
        ThreadDispatcher.RunOnMainThread(() =>
        {
            _hud.ChangeDepth(searchStatistics.Depth);
            _hud.ChangeBestEvaluation(searchStatistics.BestEvaluation);
            _hud.ChangePositionsEvaluated(searchStatistics.PositionsEvaluated);
            _hud.ChangeCutoffs(searchStatistics.Cutoffs);
            _hud.ChangeTranspositions(searchStatistics.Transpositions);
        });
    }
}
