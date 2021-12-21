using Backend;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Frontend
{
	public class CapturedPieces : MonoBehaviour
	{
		[Header("Icon Prefab")]
		[SerializeField] Image _capturedPieceIconPrefab;

		[Header("Icons Positions")]
		[SerializeField] Transform _whiteIconsParent;
		[SerializeField] Transform _blackIconsParent;

		[Header("Pieces Sprites")]
		[SerializeField] PiecesSprites _piecesSprites;

		List<Tuple<PieceType, Image>> _whiteCapturedPiecesIcons = new List<Tuple<PieceType, Image>>();
		List<Tuple<PieceType, Image>> _blackCapturedPiecesIcons = new List<Tuple<PieceType, Image>>();

		Vector2 _whiteCurrentIconPosition;
		Vector2 _blackCurrentIconPosition;
		Vector2 _offset = new Vector2(-30, 0);

		void Start()
		{
			_whiteCurrentIconPosition = _whiteIconsParent.position;
			_blackCurrentIconPosition = _blackIconsParent.position;
		}

		public void AddCaptureIcon(PieceType pieceType, ColorType pieceColor)
		{
			if (pieceColor == ColorType.White)
			{
				Image newPieceIcon = Instantiate(_capturedPieceIconPrefab, _whiteCurrentIconPosition, _whiteIconsParent.rotation, _whiteIconsParent);

				newPieceIcon.sprite = _piecesSprites.GetSprite(pieceColor, pieceType);
				newPieceIcon.SetNativeSize();

				_whiteCurrentIconPosition += _offset;

				_whiteCapturedPiecesIcons.Add(new Tuple<PieceType, Image>(pieceType, newPieceIcon));

				SortIcons(_whiteCapturedPiecesIcons);
			}
			else
			{
				Image newPieceIcon = Instantiate(_capturedPieceIconPrefab, _blackCurrentIconPosition, _blackIconsParent.rotation, _blackIconsParent);

				newPieceIcon.sprite = _piecesSprites.GetSprite(pieceColor, pieceType);
				newPieceIcon.SetNativeSize();

				_blackCurrentIconPosition += _offset;

				_blackCapturedPiecesIcons.Add(new Tuple<PieceType, Image>(pieceType, newPieceIcon));

				SortIcons(_blackCapturedPiecesIcons);
			}
		}

		void SortIcons(List<Tuple<PieceType, Image>> iconsToSort)
		{
			for (int i = 0; i < iconsToSort.Count; i++)
			{
				for (int j = 0; j < iconsToSort.Count - 1; j++)
				{
					if ((int)iconsToSort[j + 1].Item1 > (int)iconsToSort[j].Item1)
					{
						var positionTemp = iconsToSort[j + 1].Item2.rectTransform.localPosition;
						var listTemp = iconsToSort[j + 1];

						iconsToSort[j + 1].Item2.rectTransform.localPosition = iconsToSort[j].Item2.rectTransform.localPosition;
						iconsToSort[j + 1] = iconsToSort[j];

						iconsToSort[j].Item2.rectTransform.localPosition = positionTemp;
						iconsToSort[j] = listTemp;
					}
				}
			}
		}
	}
}
