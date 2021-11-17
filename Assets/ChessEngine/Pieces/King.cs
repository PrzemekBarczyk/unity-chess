using Vector2Int = UnityEngine.Vector2Int;

public class King : Piece
{
    public override PieceType Type => PieceType.King;

    public static int VALUE => 9000;
    public override int Value { get => VALUE; }

    public readonly static int[,] POSITION_VALUES = {
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -30, -40, -40, -50, -50, -40, -40, -30 },
            { -20, -30, -30, -40, -40, -30, -30, -20 },
            { -10, -20, -20, -20, -20, -20, -20, -10 },
            {  20,  20,   0,   0,   0,   0,  20,  20 },
            {  20,  30,  10,   0,   0,  10,  30,  20 }
    };
    public override int[,] PositionsValues => POSITION_VALUES;

    public bool CanCastleKingside { get; set; }
    public bool CanCastleQueenside { get; set; }

    public static readonly Vector2Int WHITE_KING_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(6, Board.BOTTOM_RANK_INDEX);
    public static readonly Vector2Int WHITE_KING_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(2, Board.BOTTOM_RANK_INDEX);
    public static readonly Vector2Int BLACK_KING_AFTER_KINGSIDE_CASTLE_POSITION = new Vector2Int(6, Board.TOP_RANK_INDEX);
    public static readonly Vector2Int BLACK_KING_AFTER_QUEENSIDE_CASTLE_POSITION = new Vector2Int(2, Board.TOP_RANK_INDEX);

    public King(Board board, PieceSet pieces, ColorType color, Vector2Int position) : base(board, pieces, color, position) { }

	public bool IsChecked()
    {
        ColorType attackerColor = Color == ColorType.White ? ColorType.Black : ColorType.White;
        return Square.IsAttackedBy(attackerColor);
    }
}
