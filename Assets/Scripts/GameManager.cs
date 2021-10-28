using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField] string _startChessPositionInFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public string StartChessPositionInFEN { get => _startChessPositionInFEN; set => _startChessPositionInFEN = value; }

    public ExtractedFENData ExtractedFENData { get; private set; }

    public State State { get; private set; } = State.Playing;
    public Stack<HistoryData> History { get; private set; } = new Stack<HistoryData>();

	PieceManager _pieces;
	PlayerManager _players;
    Board _board;

	new void Awake()
	{
		base.Awake();

		_pieces = PieceManager.Instance;
		_players = PlayerManager.Instance;
        _board = Board.Instance;

		ExtractDataFromFEN();
    }

    void ExtractDataFromFEN()
	{
		ExtractedFENData = FENExtractor.FENToBoardPositionData(_startChessPositionInFEN);

		_pieces.WhitePieces.CreatePieces(ExtractedFENData.PiecesToCreate);
		_pieces.BlackPieces.CreatePieces(ExtractedFENData.PiecesToCreate);
		_players.SetStartingPlayerColor(ExtractedFENData.PlayerToMoveColor);
		_pieces.WhitePieces.King.CanCastleKingside = ExtractedFENData.HasWhiteCastleKingsideRights;
		_pieces.WhitePieces.King.CanCastleQueenside = ExtractedFENData.HasWhiteCastleQueensideRights;
		_pieces.BlackPieces.King.CanCastleKingside = ExtractedFENData.HasBlackCastleKingsideRights;
		_pieces.BlackPieces.King.CanCastleQueenside = ExtractedFENData.HasBlackCastleQueensideRights;
		_pieces.SetEnPassantTarget(ExtractedFENData.EnPassantTargetPiecePosition);
	}

    public void StartGame() // called after color selection
    {
        StartCoroutine(GameLoop());
    }

    public IEnumerator GameLoop()
    {
        while (true)
        {
            OnMoveBegin();
            yield return _players.CurrentPlayer.Move();
            OnMoveEnded();

            State = CheckGameState();

            if (State == State.Playing)
            {
                _players.SwitchTurn();
            }
            else if (State == State.Checkmate)
            {
                EndGame(State.Checkmate);
            }
            else if (State == State.Draw)
            {
                EndGame(State.Draw);
            }
        }
    }

    void OnMoveBegin()
    {
        MoveData? lastMove = _players.CurrentPlayer.LastMove;
        if (lastMove.HasValue) _board.HideLastMoveIndicators(lastMove.Value);
    }

    void OnMoveEnded()
    {
        MoveData? lastMove = _players.CurrentPlayer.LastMove;
        _board.DisplayLastMoveIndicators(lastMove.Value);
    }

    State CheckGameState()
    {
        _players.NextPlayer.Pieces.GenerateLegalMoves();

        if (!_players.NextPlayer.Pieces.HasLegalMoves())
        {
            if (_players.NextPlayer.Pieces.King.IsChecked())
            {
                return State.Checkmate;
            }
            else
            {
                return State.Draw;
            }
        }

        return State.Playing;
    }

    public void EndGame(State result)
    {
        State = result;
        Time.timeScale = 0f;
        Debug.Log("Game over: " + result);
    }
}
