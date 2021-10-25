using UnityEngine;

public class PromotionPanel : MonoBehaviour
{
	public MoveType PromotionType { get; private set; }

	bool _isPromotionSelected;

	public void OnPromotionSelected(MoveType promotionSelected)
	{
		PromotionType = promotionSelected;
		_isPromotionSelected = true;
	}

	public bool IsPromotionSelected()
	{
		if (_isPromotionSelected)
		{
			_isPromotionSelected = false;
			return true;
		}
		return false;
	}
}
