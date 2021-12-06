using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    [Header("Move History")]
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

    [Header("Clocks")]
    [SerializeField] Text _whitePlayerClock;
    [SerializeField] Text _blackPlayerClock;

    void Awake()
    {
        RemovePlaceholders();
    }

    void RemovePlaceholders()
	{
        _whitePlayerMoveList.text = "";
        _blackPlayerMoveList.text = "";

        _zobristKey.text = "Zobrist Key: ";
        _evaluation.text = "Evaluation: ";

        _depth.text = "Depth: ";
        _bestEvalutation.text = "Best Evaluation: ";
        _positionsEvaluated.text = "Positions Evaluated: ";
        _cutoffs.text = "Cutoffs: ";
        _tranpositions.text = "Transpositions: ";

        _whitePlayerClock.text = "";
        _blackPlayerClock.text = "";
    }

    public void SetUp(GameSettings gameSettings)
	{
        if (gameSettings.GameType == GameType.BotVsBot || gameSettings.GameType == GameType.HumanVsBot || gameSettings.GameType == GameType.BotVsHuman)
        {
            _botStatistics.SetActive(true);
        }
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

    public void HandleQuitButton()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

    public void HandleSaveButton()
	{
        GameManager.Instance.ConvertPositionToFEN();
	}
}
