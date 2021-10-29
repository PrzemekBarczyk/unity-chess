using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Perft")]
public class SinglePerftInfo : ScriptableObject
{
    [SerializeField] string _testedFEN;
    [SerializeField] ulong[] _correctResults;
    [SerializeField] [Range(0, 10)] ushort _maxDepth = 4;

    public string TestedFEN => _testedFEN;
    public ulong[] CorrectResults => _correctResults;
    public ushort MaxDepth => _maxDepth;
}
