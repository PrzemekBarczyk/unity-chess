using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public State State { get; private set; } = State.Playing;

    Dictionary<ulong, int> _repetitionHistory = new Dictionary<ulong, int>(16);

    ChessEngine _chessEngine;

    PlayerManager _playerManager;
    GraphicalBoard _graphicalBoard;

    HUD _hud;

    void Start()
    {
        _playerManager = PlayerManager.Instance;
        _graphicalBoard = GraphicalBoard.Instance;

        _hud = FindObjectOfType<HUD>(true);

        _graphicalBoard.CreateBoard();
    }

    public void StartGame(GameSettings gameSettings) // called after pressing PLAY button
    {
        ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(gameSettings.StartPositionInFEN);

        _graphicalBoard.CreatePieces(extractedFENData.PiecesToCreate);

        _playerManager.CreatePlayers(gameSettings.GameType, gameSettings.UseClocks, gameSettings.BaseTime, gameSettings.AddedTime);
        _playerManager.SetStartingPlayer(extractedFENData.PlayerToMoveColor);

        _chessEngine = new ChessEngine(gameSettings.StartPositionInFEN);

        StartCoroutine(GameLoop());
    }

    public IEnumerator GameLoop()
    {
        _repetitionHistory.Add(_chessEngine.Board.ZobristHash, 1);
        _hud.ChangeZobristKey(_chessEngine.Board.ZobristHash);
        _hud.ChangeEvaluation(_chessEngine.Evaluate());

        while (true)
        {
            Move? moveToMake = null;
            new Thread(() => moveToMake = _playerManager.CurrentPlayer.SelectMoveAndCountTime(_chessEngine)).Start();

            yield return new WaitUntil(() => moveToMake.HasValue); // waits until player selects his move

            _graphicalBoard.UpdateBoard(moveToMake.Value, _playerManager.NextPlayer.LastMove);

            _hud.AddMoveToHistory(moveToMake.Value);

            State = _chessEngine.MakeMove(moveToMake.Value);

            _hud.ChangeZobristKey(_chessEngine.Board.ZobristHash);
            _hud.ChangeEvaluation(_chessEngine.Evaluate());

            try
            {
                _repetitionHistory.Add(_chessEngine.Board.ZobristHash, 1);
            }
            catch (ArgumentException)
            {
                _repetitionHistory[_chessEngine.Board.ZobristHash] = _repetitionHistory[_chessEngine.Board.ZobristHash] + 1;
                if (_repetitionHistory[_chessEngine.Board.ZobristHash] >= 3)
                    State = State.DrawByRepetitions;
            }

            if (State == State.Playing)
            {
                _playerManager.SwitchTurn();
            }
            else
            {
                EndGame(State);
                yield break;
            }
        }
    }

    public void EndGame(State result)
    {
        State = result;
        Time.timeScale = 0f;
        Debug.Log("Game over: " + result);
    }

    public void TimeElapsed()
    {
        EndGame(State.TimeElapsed);
    }
}
