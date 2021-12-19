using UnityEngine;

namespace Frontend
{
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

        public bool OnTopRank { get => transform.position.y == GraphicalBoard.TOP_RANK_INDEX; }
        public bool OnBottomRank { get => transform.position.y == GraphicalBoard.BOTTOM_RANK_INDEX; }

        GameManager _gameManager;

        MoveSelector _moveSelector;

        void Awake()
        {
            _gameManager = GameManager.Instance;
        }

        void Start()
        {
            if (OnTopRank || OnBottomRank)
                CreatePromotionPanel();

            _moveSelector = FindObjectOfType<MoveSelector>();
        }

        void CreatePromotionPanel()
        {
            PromotionPanel = Instantiate(OnTopRank ? _whitePromotionPanelPrefab : _blackPromotionPanelPrefab, transform.position, transform.rotation, transform);

            PromotionPanel.name = PromotionPanel.name.Replace("(Clone)", "");
            PromotionPanel.gameObject.SetActive(false);
        }

        void OnMouseDown()
        {
            if (_gameManager.State == State.Playing)
            {
                StartCoroutine(_moveSelector.OnSquareSelected(this));
            }
        }

        void OnMouseUp()
        {
            if (_gameManager.State == State.Playing)
            {
                StartCoroutine(_moveSelector.OnPieceDrop());
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
}
