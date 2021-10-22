using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	[SerializeField] string _startChessPositionInFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public State State { get; private set; } = State.Playing;
    public Stack<HistoryData> History { get; private set; } = new Stack<HistoryData>();

	PieceManager _pieces;
	PlayerManager _players;

	new void Awake()
	{
		base.Awake();

		_pieces = PieceManager.Instance;
		_players = PlayerManager.Instance;

		ExtractDataFromFEN();
    }

    void ExtractDataFromFEN()
	{
		ExtractedFENData extractedFENData = FENExtractor.FENToBoardPositionData(_startChessPositionInFEN);

		_pieces.WhitePieces.CreatePieces(extractedFENData.PiecesToCreate);
		_pieces.BlackPieces.CreatePieces(extractedFENData.PiecesToCreate);
		_players.SetStartingPlayerColor(extractedFENData.PlayerToMoveColor);
		_pieces.WhitePieces.King.CanCastleKingside = extractedFENData.HasWhiteCastleKingsideRights;
		_pieces.WhitePieces.King.CanCastleQueenside = extractedFENData.HasWhiteCastleQueensideRights;
		_pieces.BlackPieces.King.CanCastleKingside = extractedFENData.HasBlackCastleKingsideRights;
		_pieces.BlackPieces.King.CanCastleQueenside = extractedFENData.HasBlackCastleQueensideRights;
		_pieces.SetEnPassantTarget(extractedFENData.EnPassantTargetPiecePosition);
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
        
    }

    void OnMoveEnded()
    {
        
    }

    State CheckGameState()
    {
        _players.NextPlayer.Pieces.FindLegalMoves();

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
