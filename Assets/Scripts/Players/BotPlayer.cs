using UnityEngine;

[RequireComponent(typeof(SearchAlgorithm))]
public class BotPlayer : Player
{
    public override PlayerType Type => PlayerType.Bot;

	[SerializeField] SearchAlgorithm _searchAlgorithm;

	new void Awake()
	{
		base.Awake();

		if (_searchAlgorithm == null) Debug.LogWarning("Search algorithm wasn't assigned");
	}

	protected override MoveData SelectMove()
	{
		return _searchAlgorithm.FindBestMove(Pieces);
	}
}
