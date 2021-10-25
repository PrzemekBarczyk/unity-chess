using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Pawn))]
[RequireComponent(typeof(Queen))]
[RequireComponent(typeof(Rook))]
[RequireComponent(typeof(Bishop))]
[RequireComponent(typeof(Knight))]
public class PawnPromotions: MonoBehaviour
{
	[Header("Sprites")]
	[SerializeField] Sprite _pawnSprite;
	[SerializeField] Sprite _queenSprite;
	[SerializeField] Sprite _rookSprite;
	[SerializeField] Sprite _bishopSprite;
	[SerializeField] Sprite _knightSprite;

	Pawn _pawnScript;
	Queen _queenScript;
	Rook _rookScript;
	Bishop _bishopScript;
	Knight _knightScript;

	public Piece CurrentPromotion { get; private set; }

	public Sprite CurrentSprite { get; private set; }

	SpriteRenderer _spriteRenderer;

	void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();

		_pawnScript = GetComponent<Pawn>();
		_queenScript = GetComponent<Queen>();
		_rookScript = GetComponent<Rook>();
		_bishopScript = GetComponent<Bishop>();
		_knightScript = GetComponent<Knight>();
	}

	public void Promote(MoveType promotionType, bool changeSprite)
	{
		switch (promotionType)
		{
			case MoveType.PromotionToQueen:
				PromoteToQueen();
				break;
			case MoveType.PromotionToRook:
				PromoteToRook();
				break;
			case MoveType.PromotionToBishop:
				PromoteToBishop();
				break;
			case MoveType.PromotionToKnight:
				PromoteToKnight();
				break;
		}

		if (changeSprite)
		{
			transform.localScale = new Vector3(0.7f, 0.7f);
			_spriteRenderer.sprite = CurrentSprite;
		}

		CurrentPromotion.Square = _pawnScript.Square;
		CurrentPromotion.Square.Piece = CurrentPromotion;
	}

	void PromoteToQueen()
	{
		CurrentPromotion = _queenScript;
		CurrentSprite = _queenSprite;
	}
	void PromoteToRook()
	{
		CurrentPromotion = _rookScript;
		CurrentSprite = _rookSprite;
	}
	void PromoteToBishop()
	{
		CurrentPromotion = _bishopScript;
		CurrentSprite = _bishopSprite;
	}
	void PromoteToKnight()
	{
		CurrentPromotion = _knightScript;
		CurrentSprite = _knightSprite;
	}

	public void UndoPromotion(bool changeSprite)
	{
		CurrentPromotion = null;
		CurrentSprite = _pawnSprite;

		if (changeSprite)
		{
			transform.localScale = new Vector3(0.6f, 0.6f);
			_spriteRenderer.sprite = CurrentSprite;
		}
	}
}
