using System;
using System.Collections.Generic;

public class Perft
{
    PieceManager _pieceManager;
    PieceSet _whitePieces;
    PieceSet _blackPieces;

    List<string> _divideResults = new List<string>();

    public Perft(PieceManager pieceManager)
    {
        _pieceManager = pieceManager;
        _whitePieces = _pieceManager.WhitePieces;
        _blackPieces = _pieceManager.BlackPieces;
    }

    public ulong RunSinglePerft(int maxDepth)
    {
        return FastSearch(_pieceManager.CurrentPieces, maxDepth);
    }

    public List<string> RunDivide(int maxDepth)
    {
        _divideResults.Clear();
        _divideResults.Add("Nodes searched: " + Divide(_pieceManager.CurrentPieces, maxDepth, maxDepth));
        return _divideResults;
    }

    ulong Search(PieceSet currentPieces, int depth)
    {
        if (depth == 0)
        {
            return 1;
        }

        List<Move> legalMoves = new List<Move>(currentPieces.GenerateLegalMoves());

        PieceSet nextPieces = currentPieces.Color == ColorType.White ? _blackPieces : _whitePieces;

        ulong nodes = 0;

        for (int i = 0; i < legalMoves.Count; i++)
        {
            Move legalMove = legalMoves[i];

            legalMove.Piece.Move(legalMove);
            nodes += Search(nextPieces, depth - 1);
            legalMove.Piece.UndoMove(legalMove);
        }

        return nodes;
    }

    ulong FastSearch(PieceSet currentPieces, int depth)
    {
        if (depth <= 0)
        {
            return 1;
        }

        List<Move> legalMoves = new List<Move>(currentPieces.GenerateLegalMoves());

        if (depth == 1)
        {
            return Convert.ToUInt64(legalMoves.Count);
        }

        PieceSet nextPieces = currentPieces.Color == ColorType.White ? _blackPieces : _whitePieces;

        ulong nodes = 0;

        for (int i = 0; i < legalMoves.Count; i++)
        {
            Move legalMove = legalMoves[i];

            legalMove.Piece.Move(legalMove);
            nodes += FastSearch(nextPieces, depth - 1);
            legalMove.Piece.UndoMove(legalMove);
        }

        return nodes;
    }

    ulong Divide(PieceSet currentPieces, int depth, int maxDepth)
    {
        if (depth == 0)
        {
            return 1;
        }

        List<Move> legalMoves = new List<Move>(currentPieces.GenerateLegalMoves());

        PieceSet nextPieces = currentPieces.Color == ColorType.White ? _blackPieces : _whitePieces;

        ulong nodes = 0;

        for (int i = 0; i < legalMoves.Count; i++)
        {
            Move legalMove = legalMoves[i];

            legalMove.Piece.Move(legalMove);
            ulong localNodes = Divide(nextPieces, depth - 1, maxDepth);
            nodes += localNodes;
            legalMove.Piece.UndoMove(legalMove);

            if (depth == maxDepth)
            {
                _divideResults.Add(AlgebraicNotation.MoveToAlgebraicNotation(legalMove) + ": " + localNodes);
            }
        }

        return nodes;
    }
}
