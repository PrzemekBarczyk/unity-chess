using UnityEngine;
using UnityEngine.UI;

public class CapturedPieces : MonoBehaviour
{
	[SerializeField] ColorType _color;

	[SerializeField] Image _pieceIconPrefab;

	[SerializeField] PiecesSprites _piecesSprites;

	Vector2 _currentIconPosition;
	Vector2 _offset = new Vector2(-30, 0);

	void Start()
	{
		_currentIconPosition = transform.position;
	}

	public void AddCaptureIcon(PieceType pieceType)
	{
		Image newPieceIcon = Instantiate(_pieceIconPrefab, _currentIconPosition, transform.rotation, transform);
		newPieceIcon.sprite = _piecesSprites.GetSprite(_color, pieceType);
		newPieceIcon.SetNativeSize();
		_currentIconPosition += _offset;
	}
}
