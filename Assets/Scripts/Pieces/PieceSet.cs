using System.Collections.Generic;
using UnityEngine;

public class PieceSet : MonoBehaviour
{
    [SerializeField] ColorType _color;

    public ColorType Color => _color;

    [SerializeField] Piece[] _piecesPrefabs;

	public List<Piece> Pieces { get; } = new List<Piece>();

	public King King { get; private set; }

	public void CreatePieces(List<PieceData> piecesToCreate)
	{
		foreach (PieceData pieceToCreate in piecesToCreate)
		{
			if (pieceToCreate.Color == _color)
			{
				foreach (Piece piecePrefab in _piecesPrefabs)
				{
					if (pieceToCreate.Type == piecePrefab.Type)
					{
						Piece newPiece = Instantiate(piecePrefab, pieceToCreate.Position, transform.rotation, transform);
						newPiece.name = piecePrefab.name;

						Pieces.Add(newPiece);

						if (newPiece is King king)
							King = king;
					}
				}
			}
		}
	}

	public void FindLegalMoves()
	{
		foreach (Piece piece in AlivePieces())
			piece.FindLegalMoves();
	}

	public List<Piece> AlivePieces()
	{
		List<Piece> alivePieces = new List<Piece>();
		foreach (Piece piece in Pieces)
		{
			if (piece.IsAlive)
				alivePieces.Add(piece);
		}
		return alivePieces;
	}
}
