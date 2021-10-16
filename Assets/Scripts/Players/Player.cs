using UnityEngine;

public abstract class Player : MonoBehaviour
{
	public ColorType Color { get; private set; }
	public abstract PlayerType Type { get; }

	public PieceSet Pieces { get; private set; }

	PlayerManager _playerManager;

	void Awake()
	{
		_playerManager = FindObjectOfType<PlayerManager>();
	}

	public void Initialize(ColorType color)
	{
		Color = color;
		Pieces = Color == ColorType.White ? GameObject.Find("White Pieces").GetComponent<PieceSet>() : GameObject.Find("Black Pieces").GetComponent<PieceSet>();
	}
}
