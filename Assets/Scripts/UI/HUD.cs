using Backend;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Frontend
{
    public class HUD : MonoBehaviour
    {
        [Header("Move History")]
        [SerializeField] Scrollbar _scrollbar;
        [SerializeField] Text _whitePlayerMoveList;
        [SerializeField] Text _blackPlayerMoveList;

        [Header("Bot statistics")]
        [SerializeField] GameObject _botStatistics;
        [SerializeField] Text _depth;
        [SerializeField] Text _bestEvalutation;
        [SerializeField] Text _positionsEvaluated;
        [SerializeField] Text _cutoffs;
        [SerializeField] Text _tranpositions;

        [Header("Board statistics")]
        [SerializeField] Text _zobristKey;
        [SerializeField] Text _evaluation;
        [SerializeField] Text _fen;

        [Header("Clocks")]
        [SerializeField] Text _whitePlayerClock;
        [SerializeField] Text _blackPlayerClock;

        [Header("Result message")]
        [SerializeField] Image _resultMessageBackground;
        [SerializeField] Text _resultMessageText;

        public void SetUp(GameType gameType)
        {
            if (gameType == GameType.BotVsBot || gameType == GameType.HumanVsBot || gameType == GameType.BotVsHuman)
            {
                _botStatistics.SetActive(true);
            }

            _whitePlayerMoveList.text = "";
            _blackPlayerMoveList.text = "";

            _zobristKey.text = "Zobrist Key: ";
            _evaluation.text = "Evaluation: ";
            _fen.text = "FEN: ";

            _depth.text = "Depth: ";
            _bestEvalutation.text = "Best Evaluation: ";
            _positionsEvaluated.text = "Positions Evaluated: ";
            _cutoffs.text = "Cutoffs: ";
            _tranpositions.text = "Transpositions: ";

            _whitePlayerClock.text = "";
            _blackPlayerClock.text = "";

            _resultMessageBackground.gameObject.SetActive(false);
            _resultMessageText.text = "";
        }

        public void AddMoveToHistory(Move move)
        {
            if (move.Piece.Color == ColorType.White)
            {
                _whitePlayerMoveList.text += SimplifiedAlgebraicNotation.MoveToLongSAN(move) + "\n";
            }
            else
            {
                _blackPlayerMoveList.text += SimplifiedAlgebraicNotation.MoveToLongSAN(move) + "\n";
            }
            _scrollbar.value = 0f;
        }

        public void ChangeDepth(uint depth)
        {
            if (depth == 0)
            {
                _depth.text = "Depth: book";
            }
            else
            {
                _depth.text = "Depth: " + depth;
            }
        }

        public void ChangeBestEvaluation(int bestEvaluation)
        {
            _bestEvalutation.text = "Best evaluation: " + bestEvaluation;
        }

        public void ChangePositionsEvaluated(uint positionsEvaluated)
        {
            _positionsEvaluated.text = "Positions evaluated: " + positionsEvaluated;
        }

        public void ChangeCutoffs(uint cutoffs)
        {
            _cutoffs.text = "Cutoffs: " + cutoffs;
        }

        public void ChangeTranspositions(uint transpositions)
        {
            _tranpositions.text = "Transpositions: " + transpositions;
        }

        public void ChangeZobristKey(ulong zobristKey)
        {
            _zobristKey.text = "Zobrist Key: " + zobristKey;
        }

        public void ChangeEvaluation(int boardEvaluation)
        {
            _evaluation.text = "Evaluation: " + boardEvaluation;
        }

        public void ChangeFEN(string newFEN)
        {
            _fen.text = "FEN: " + newFEN;
        }

        public void HandleQuitButton()
        {
            GameManager.Instance.StopGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void DisplayResultMessage(State result)
        {
            _resultMessageBackground.gameObject.SetActive(true);

            switch (result)
            {
                case State.Checkmate:
                    _resultMessageText.text = "Checkmate";
                    break;
                case State.DrawByStalemate:
                    _resultMessageText.text = "Draw by stalemate";
                    break;
                case State.DrawByFiftyMoveRule:
                    _resultMessageText.text = "Draw by fifty move rule";
                    break;
                case State.DrawByRepetitions:
                    _resultMessageText.text = "Draw by repetitions";
                    break;
                case State.DrawByInsufficientMaterial:
                    _resultMessageText.text = "Draw by insufficient material";
                    break;
                case State.TimeElapsed:
                    _resultMessageText.text = "Time elapsed";
                    break;
                default:
                    throw new System.Exception("Undefined result");
            }
        }
    }
}
