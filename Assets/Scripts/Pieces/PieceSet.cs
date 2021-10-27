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

	public void GenerateLegalMoves()
	{
		foreach (Piece piece in AlivePieces())
			piece.GenerateLegalMoves();
	}

	public bool HasLegalMoves()
	{
		foreach (Piece piece in AlivePieces())
		{
			if (piece.LegalMoves.Count > 0)
				return true;
		}
		return false;
	}

	public List<MoveData> GetLegalMoves()
	{
		List<MoveData> legalMoves = new List<MoveData>();
		foreach (Piece piece in Pieces)
		{
			if (piece.IsAlive)
			{
				foreach (MoveData move in piece.LegalMoves)
					legalMoves.Add(move);
			}
		}
		return legalMoves;
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
