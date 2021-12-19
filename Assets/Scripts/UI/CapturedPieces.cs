using Backend;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Frontend
{
	public class CapturedPieces : MonoBehaviour
	{
		[SerializeField] ColorType _color;

		[SerializeField] Image _pieceIconPrefab;

		[SerializeField] PiecesSprites _piecesSprites;

		List<Tuple<PieceType, Image>> _capturedPiecesIcons = new List<Tuple<PieceType, Image>>();

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

			_capturedPiecesIcons.Add(new Tuple<PieceType, Image>(pieceType, newPieceIcon));

			SortIcons();
		}

		void SortIcons()
		{
			for (int i = 0; i < _capturedPiecesIcons.Count; i++)
			{
				for (int j = 0; j < _capturedPiecesIcons.Count - 1; j++)
				{
					if ((int)_capturedPiecesIcons[j + 1].Item1 > (int)_capturedPiecesIcons[j].Item1)
					{
						var positionTemp = _capturedPiecesIcons[j + 1].Item2.rectTransform.localPosition;
						var listTemp = _capturedPiecesIcons[j + 1];

						_capturedPiecesIcons[j + 1].Item2.rectTransform.localPosition = _capturedPiecesIcons[j].Item2.rectTransform.localPosition;
						_capturedPiecesIcons[j + 1] = _capturedPiecesIcons[j];

						_capturedPiecesIcons[j].Item2.rectTransform.localPosition = positionTemp;
						_capturedPiecesIcons[j] = listTemp;
					}
				}
			}
		}
	}
}
