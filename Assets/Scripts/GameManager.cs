using System.Collections;
using System.Threading;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField] string _startChessPositionInFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public State State { get; private set; } = State.Playing;

    ChessEngine _chessEngine;

	PlayerManager _playerManager;
    GraphicalBoard _graphicalBoard;

	new void Awake()
	{
		base.Awake();

        _chessEngine = new ChessEngine(_startChessPositionInFEN);

		_playerManager = PlayerManager.Instance;
        _graphicalBoard = GraphicalBoard.Instance;

        ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(_startChessPositionInFEN);
        _playerManager.SetStartingPlayerColor(extractedFENData.PlayerToMoveColor);
        _graphicalBoard.CreateBoard(extractedFENData.PiecesToCreate);
    }

    public void StartGame() // called after color selection
    {
        StartCoroutine(GameLoop());
    }

    public IEnumerator GameLoop()
    {
        while (true)
        {
            Move? moveToMake = null;
            new Thread(() => moveToMake = _playerManager.CurrentPlayer.SelectMove(_chessEngine)).Start();

            yield return new WaitUntil(() => moveToMake.HasValue); // waits until player selects his move

            _graphicalBoard.UpdateBoard(moveToMake.Value, _playerManager.NextPlayer.LastMove);

            State = _chessEngine.MakeMove(moveToMake.Value);

            if (State == State.Playing)
            {
                _playerManager.SwitchTurn();
            }
            else
            {
                EndGame(State);
            }
        }
    }

    public void EndGame(State result)
    {
        State = result;
        Time.timeScale = 0f;
        Debug.Log("Game over: " + result);
    }
}
