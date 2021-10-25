using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Square : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] PromotionPanel _whitePromotionPanelPrefab;
    [SerializeField] PromotionPanel _blackPromotionPanelPrefab;

    public PromotionPanel PromotionPanel { get; private set; }

    [Header("Indicators")]
    [SerializeField] GameObject _attackIndicator;
    [SerializeField] GameObject _emptyFieldIndicator;
    [SerializeField] GameObject _lastMoveIndicator;

    [SerializeField] ColorType _colorType;

	public ColorType ColorType => _colorType;

	public Vector2Int Position { get; private set; }

	public Piece Piece { get; set; }

	public bool IsOccupied => Piece != null;

    public bool OnTopRank { get => Position.y == Board.TOP_RANK; }
    public bool OnBottomRank { get => Position.y == Board.BOTTOM_RANK; }

    GameManager _gameManager;
    PlayerManager _playerManager;
    Board _board;

	void Awake()
	{
		Position = Vector2Int.RoundToInt(transform.position);
	}

    void Start()
    {
        _gameManager = GameManager.Instance;
        _playerManager = PlayerManager.Instance;
        _board = Board.Instance;

        CreatePromotionPanel();
    }

    void CreatePromotionPanel()
	{
        if (OnTopRank || OnBottomRank)
        {
            PromotionPanel = Instantiate(OnTopRank ? _whitePromotionPanelPrefab : _blackPromotionPanelPrefab, transform.position, transform.rotation, transform);
            PromotionPanel.name = PromotionPanel.name.Replace("(Clone)", "");
            PromotionPanel.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (_gameManager.State != State.Playing) return;
        if (_playerManager.CurrentPlayer is HumanPlayer)
        {
            HumanPlayer player = _playerManager.CurrentPlayer as HumanPlayer;
            player.OnSquareSelected(this);
        }
    }

    void OnMouseUp()
    {
        if (_gameManager.State != State.Playing) return;
        if (_playerManager.CurrentPlayer is HumanPlayer)
        {
            HumanPlayer player = _playerManager.CurrentPlayer as HumanPlayer;
            player.OnSuareDeselected();
        }
    }

    public bool IsPromotionSquare(ColorType pieceColor)
	{
		return pieceColor == ColorType.White ? (Position.y == Board.TOP_RANK) : (Position.y == Board.BOTTOM_RANK);
	}

    public bool IsAttackedBy(ColorType attackerColor)
    {
        if (IsAttackedFromDirection(new Vector2Int(0, 1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, 1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, 0), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(0, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, 0), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, 1), attackerColor)) return true;

        if (IsAttackedFromRelativePosition(new Vector2Int(-1, 2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(1, 2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(2, 1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(2, -1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-1, -2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(1, -2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-2, -1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-2, 1), attackerColor)) return true;

        return false;
    }

    bool IsAttackedFromDirection(Vector2Int direction, ColorType opponentColor)
    {
        Vector2Int checkedPosition = Position;

        for (int i = 0; i < 8; i++)
        {
            checkedPosition += direction;

            Square checkedSquare;
            try
            {
                checkedSquare = _board.Squares[checkedPosition.x, checkedPosition.y];
            }
            catch (IndexOutOfRangeException) // square outside board
            {
                return false;
            }

            if (checkedSquare.IsOccupied)
            {
                Piece encounteredPiece = checkedSquare.Piece;

                if (encounteredPiece.Color != opponentColor)
                    return false;

                Vector2Int distance = checkedPosition - Position;

                if (encounteredPiece is Pawn pawn)
                {
                    if (Mathf.Abs(distance.x) == 1 && distance.y == -1 * pawn.DirectionModifier)
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Rook)
                {
                    if (distance.x == 0 || distance.y == 0)
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Bishop)
                {
                    if (Mathf.Abs(distance.x) == Mathf.Abs(distance.y))
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Queen)
                {
                    if (distance.x == 0 || distance.y == 0 || (Mathf.Abs(distance.x) == Mathf.Abs(distance.y)))
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.King)
                {
                    if (Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.y) <= 1)
                        return true;
                }

                return false;
            }
        }

        return false;
    }

    bool IsAttackedFromRelativePosition(Vector2Int offset, ColorType opponentColor)
    {
        Vector2Int checkedPosition = Position + offset;

        Square checkedSquare;
        try
        {
            checkedSquare = _board.Squares[checkedPosition.x, checkedPosition.y];
        }
        catch (IndexOutOfRangeException) // square outside board
        {
            return false;
        }

        if (checkedSquare.IsOccupied)
        {
            Piece encounteredPiece = checkedSquare.Piece;

            if (encounteredPiece.Color == opponentColor) // square occupied by other color
            {
                if (encounteredPiece.Type == PieceType.Knight)
                    return true;
            }
        }

        return false;
    }

    public void DisplayValidForAttackIndicator() => _attackIndicator.SetActive(true);

    public void DisplayValidForMoveIndicator() => _emptyFieldIndicator.SetActive(true);

    public void HideValidMovementIndicators()
    {
        _attackIndicator.SetActive(false);
        _emptyFieldIndicator.SetActive(false);
    }

    public void DisplayLastMoveIndicator() => _lastMoveIndicator.SetActive(true);

    public void HideLastMoveIndicator() => _lastMoveIndicator.SetActive(false);

    public void DisplayPromotionPanel()
    {
        if (PromotionPanel != null) PromotionPanel.gameObject.SetActive(true);
    }
    public void HidePromotionPanel()
    {
        if (PromotionPanel != null) PromotionPanel.gameObject.SetActive(false);
    }
}
