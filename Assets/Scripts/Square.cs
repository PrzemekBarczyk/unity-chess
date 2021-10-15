using UnityEngine;

public class Square : MonoBehaviour
{
	[SerializeField] ColorType _colorType;

	public ColorType ColorType => _colorType;

	public Vector2Int Position { get; private set; }

	public Piece Piece { get; set; }

	void Awake()
	{
		Position = Vector2Int.RoundToInt(transform.position);
	}
}
