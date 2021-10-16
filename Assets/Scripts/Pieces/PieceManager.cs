using UnityEngine;

public class PieceManager : MonoSingleton<PieceManager>
{
    [SerializeField] PieceSet _whitePieces;
    [SerializeField] PieceSet _blackPieces;

    public PieceSet WhitePieces => _whitePieces;
    public PieceSet BlackPieces => _blackPieces;

    new void Awake()
	{
        base.Awake();
	}
}
