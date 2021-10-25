using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PromotionSquare : MonoBehaviour
{
	[SerializeField] MoveType _promotionType;

	PromotionPanel _promotionPanel;

	void Start()
	{
		_promotionPanel = transform.parent.GetComponent<PromotionPanel>();

		if (_promotionPanel == null) Debug.LogWarning("Couldn't find PromotionPanel");
		if (_promotionType == MoveType.Undefinied) Debug.LogWarning("Promotion type is undefinied");
	}

	void OnMouseDown()
	{
		_promotionPanel.OnPromotionSelected(_promotionType);
	}
}
