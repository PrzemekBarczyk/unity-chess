using Backend;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Frontend
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Header("Oppening Book")]
        [SerializeField] TextAsset _openingBookFile;

        [Header("SFX")]
        [SerializeField] SFXManager _sfxManager;

        [Header("Board")]
        [SerializeField] GraphicalBoard _graphicalBoard;
        [SerializeField] MoveSelector _moveSelector;

        [Header("UI")]
        [SerializeField] HUD _hud;
        [SerializeField] Clocks _clocks;
        [SerializeField] CapturedPieces _capturedPieces;

        public State State { get; private set; }

        Thread _chessProgramThread;

        void Start()
        {
            _graphicalBoard.CreateBoard();
        }

        public void StartGame(string startPositionInFEN, GameType gameType, bool useClocks, uint baseTime, uint addedTime) // called after pressing PLAY button
        {
            FENDataAdapter extractedFENData;
            try
            {
                extractedFENData = FENConverter.FENToBoardPositionData(startPositionInFEN);
            }
            catch (FormatException e)
            {
                Debug.Log(e.Message);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }

            _graphicalBoard.CreatePieces(extractedFENData.Pieces);

            _clocks.SetUp(useClocks, baseTime, addedTime);
            _clocks.OnTimeElapsed += OnTimeElaped;

            ChessProgram chessProgram = new ChessProgram(startPositionInFEN, gameType, extractedFENData.PlayerToMoveColor, _openingBookFile.text);

            chessProgram.OnGameStarted += OnGameStarted;
            chessProgram.OnGameEnded += OnGameEnded;
            chessProgram.OnTurnStarted += OnTurnStarted;
            chessProgram.OnTurnEnded += OnTurnEnded;
            chessProgram.OnHumanTurnStarted += OnHumanTurnStarted;
            chessProgram.OnBotSearchEnded += OnBotSearchEnded;

            _chessProgramThread = chessProgram.StartGame();

            State = State.Playing;
        }

        public void StopGame()
        {
            _chessProgramThread.Abort();
        }

        public void OnTimeElaped()
        {
            GameOver(State.TimeElapsed);
        }

        public void OnGameStarted(BoardStatistics boardStatistics)
        {
            ThreadDispatcher.RunOnMainThread(() =>
            {
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

        public void OnTurnStarted(ColorType color, uint plyCounter)
        {
            ThreadDispatcher.RunOnMainThread(() =>
            {
                if (plyCounter > 1) // run opponent clock after first move
                {
                    _clocks.Run(color);
                }
            });
        }

        public void OnTurnEnded(Move move, BoardStatistics boardStatistics)
        {
            ThreadDispatcher.RunOnMainThread(() =>
            {
                _clocks.Stop(move.Piece.Color);

                if (move.EncounteredPiece != null)
                {
                    _sfxManager.PlayCaptureSFX();

                    _capturedPieces.AddCaptureIcon(move.EncounteredPiece.Type, move.EncounteredPiece.Color);
                }
                else
                {
                    _sfxManager.PlayMoveSFX();
                }

                _graphicalBoard.UpdateBoard(move);

                _hud.AddMoveToHistory(move);
                _hud.ChangeZobristKey(boardStatistics.ZobristKey);
                _hud.ChangeEvaluation(boardStatistics.Evaluation);
                _hud.ChangeFEN(boardStatistics.FEN);
            });
        }

        public Move OnHumanTurnStarted(List<Move> legalMoves)
        {
            _moveSelector.SetLegalMoves(legalMoves);

            while (!_moveSelector.IsMoveSelected())
                continue;

            return _moveSelector.GetSelectedMove();
        }

        public void OnBotSearchEnded(SearchStatistics searchStatistics)
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

        void GameOver(State result)
        {
            State = result;
            StopGame();
            _hud.DisplayResultMessage(result);
        }
    }
}
