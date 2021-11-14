using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Graphical Board")]
public class PiecesSprites : ScriptableObject
{
	[SerializeField] Sprites _whiteSprites;
	[SerializeField] Sprites _blackSprites;

	public Sprites WhiteSprites => _whiteSprites;
	public Sprites BlackSprites => _blackSprites;

	public Sprite GetSprite(ColorType color, PieceType type)
	{
		Sprites sprites = color == ColorType.White ? _whiteSprites : _blackSprites;

		switch (type)
		{
			case PieceType.Pawn:
				return sprites.Pawn;
			case PieceType.Knight:
				return sprites.Knight;
			case PieceType.Bishop:
				return sprites.Bishop;
			case PieceType.Rook:
				return sprites.Rook;
			case PieceType.Queen:
				return sprites.Queen;
			case PieceType.King:
				return sprites.King;
			default:
				throw new Exception();
		}
	}

	[Serializable]
	public class Sprites
	{
		[SerializeField] Sprite _pawn, _knight, _bishop, _rook, _queen, _king;

		public Sprite Pawn => _pawn;
		public Sprite Knight => _knight;
		public Sprite Bishop => _bishop;
		public Sprite Rook => _rook;
		public Sprite Queen => _queen;
		public Sprite King => _king;
	}
}
