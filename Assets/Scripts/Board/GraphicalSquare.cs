using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GraphicalSquare : MonoBehaviour
{
    [SerializeField] ColorType _colorType;

    [Header("Piece")]
    [SerializeField] SpriteRenderer _piece;
    public Sprite PieceSprite { get => _piece.sprite; set => _piece.sprite = value; }

    [Header("Promotion panels")]
    [SerializeField] PromotionPanel _whitePromotionPanelPrefab;
    [SerializeField] PromotionPanel _blackPromotionPanelPrefab;
    public PromotionPanel PromotionPanel { get; private set; }

    [Header("Indicators")]
    [SerializeField] GameObject _attackIndicator;
    [SerializeField] GameObject _emptyFieldIndicator;
    [SerializeField] GameObject _lastMoveIndicator;
    [SerializeField] GameObject _selectionIndicator;

    public bool OnTopRank { get => transform.position.y == Board.TOP_RANK_INDEX; }
    public bool OnBottomRank { get => transform.position.y == Board.BOTTOM_RANK_INDEX; }

    GameManager _gameManager;
    PlayerManager _playerManager;

	void Awake()
	{
        _gameManager = GameManager.Instance;
        _playerManager = PlayerManager.Instance;
    }

    void Start()
    {
        if (OnTopRank || OnBottomRank)
            CreatePromotionPanel();
    }

    void CreatePromotionPanel()
	{
        PromotionPanel = Instantiate(OnTopRank ? _whitePromotionPanelPrefab : _blackPromotionPanelPrefab, transform.position, transform.rotation, transform);

        PromotionPanel.name = PromotionPanel.name.Replace("(Clone)", "");
        PromotionPanel.gameObject.SetActive(false);
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

    public void DisplayValidForAttackIndicator() => _attackIndicator.SetActive(true);

    public void DisplayValidForMoveIndicator() => _emptyFieldIndicator.SetActive(true);

    public void HideValidMovementIndicators()
    {
        _attackIndicator.SetActive(false);
        _emptyFieldIndicator.SetActive(false);
    }

    public void DisplayLastMoveIndicator() => _lastMoveIndicator.SetActive(true);

    public void HideLastMoveIndicator() => _lastMoveIndicator.SetActive(false);

    public void DisplaySelectionIndicator() => _selectionIndicator.SetActive(true);

    public void HideSelectionIndicator() => _selectionIndicator.SetActive(false);

    public void DisplayPromotionPanel()
    {
        if (PromotionPanel != null) PromotionPanel.gameObject.SetActive(true);
    }
    public void HidePromotionPanel()
    {
        if (PromotionPanel != null) PromotionPanel.gameObject.SetActive(false);
    }
}
