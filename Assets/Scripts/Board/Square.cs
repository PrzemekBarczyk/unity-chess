using System;
using UnityEngine;

public class Square : MonoBehaviour
{
	[SerializeField] ColorType _colorType;

	public ColorType ColorType => _colorType;

	public Vector2Int Position { get; private set; }

	public Piece Piece { get; set; }

	public bool IsOccupied => Piece != null;

    Board _board;

	void Awake()
	{
        _board = Board.Instance;
		Position = Vector2Int.RoundToInt(transform.position);
	}

	public bool IsPromotionSquare(ColorType pieceColor)
	{
		return pieceColor == ColorType.White ? (Position.y == Board.TOP_RANK) : (Position.y == Board.BOTTOM_RANK);
	}

    public bool IsAttackedBy(ColorType attackerColor)
    {
        if (IsAttackedFromDirection(new Vector2Int(0, 1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, 1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, 0), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(1, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(0, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, -1), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, 0), attackerColor)) return true;
        if (IsAttackedFromDirection(new Vector2Int(-1, 1), attackerColor)) return true;

        if (IsAttackedFromRelativePosition(new Vector2Int(-1, 2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(1, 2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(2, 1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(2, -1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-1, -2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(1, -2), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-2, -1), attackerColor)) return true;
        if (IsAttackedFromRelativePosition(new Vector2Int(-2, 1), attackerColor)) return true;

        return false;
    }

    bool IsAttackedFromDirection(Vector2Int direction, ColorType opponentColor)
    {
        Vector2Int checkedPosition = Position;

        for (int i = 0; i < 8; i++)
        {
            checkedPosition += direction;

            Square checkedSquare;
            try
            {
                checkedSquare = _board.Squares[checkedPosition.x, checkedPosition.y];
            }
            catch (IndexOutOfRangeException) // square outside board
            {
                return false;
            }

            if (checkedSquare.IsOccupied)
            {
                Piece encounteredPiece = checkedSquare.Piece;

                if (encounteredPiece.Color != opponentColor)
                    return false;

                Vector2Int distance = checkedPosition - Position;

                if (encounteredPiece.Type == PieceType.Pawn)
                {
                    if (Mathf.Abs(distance.x) == 1 && distance.y == -1 * encounteredPiece.DirectionModifier)
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Rook)
                {
                    if (distance.x == 0 || distance.y == 0)
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Bishop)
                {
                    if (Mathf.Abs(distance.x) == Mathf.Abs(distance.y))
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.Queen)
                {
                    if (distance.x == 0 || distance.y == 0 || (Mathf.Abs(distance.x) == Mathf.Abs(distance.y)))
                        return true;
                }
                else if (encounteredPiece.Type == PieceType.King)
                {
                    if (Mathf.Abs(distance.x) <= 1 && Mathf.Abs(distance.y) <= 1)
                        return true;
                }

                return false;
            }
        }

        return false;
    }

    bool IsAttackedFromRelativePosition(Vector2Int offset, ColorType opponentColor)
    {
        Vector2Int checkedPosition = Position + offset;

        Square checkedSquare;
        try
        {
            checkedSquare = _board.Squares[checkedPosition.x, checkedPosition.y];
        }
        catch (IndexOutOfRangeException) // square outside board
        {
            return false;
        }

        if (checkedSquare.IsOccupied)
        {
            Piece encounteredPiece = checkedSquare.Piece;

            if (encounteredPiece.Color == opponentColor) // square occupied by other color
            {
                if (encounteredPiece.Type == PieceType.Knight)
                    return true;
            }
        }

        return false;
    }
}
