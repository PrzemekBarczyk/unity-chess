# if UNITY_EDITOR

using Backend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Frontend
{
    public class BookCreator : MonoBehaviour
    {
        [Header("Input files with grandmasters games in PGN")]
        [SerializeField] TextAsset[] _inputFiles;

        [Header("File with merged and formatted input data")]
        [SerializeField] TextAsset _mergedAndFormattedInputFile;

        [Header("Opening book")]
        [SerializeField] TextAsset _bookFile;
        [SerializeField] uint _maxBookDepth = 10;

        [Header("Results")]
        [SerializeField] Text _resultText;

        const string START_POSITION_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        void Start()
        {
            _resultText.text = "";
        }

        public void MergeAndFormatInput()
        {
            _resultText.text += DateTime.Now.ToString("HH:mm:ss") + ": Merging and formating input...\n";

            Stopwatch timer = new Stopwatch();
            timer.Start();

            StringBuilder output = new StringBuilder();

            foreach (var inputFile in _inputFiles)
            {
                StringReader reader = new StringReader(inputFile.text);

                bool isReadingMoves = false;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("["))
                    {
                        continue;
                    }
                    else if (line.Equals(""))
                    {
                        if (isReadingMoves)
                        {
                            output.Remove(output.Length - 1, 1); // removes space from end of line
                            output.Append("\n");
                            isReadingMoves = false;
                        }
                        continue;
                    }
                    else // reading line with moves
                    {
                        isReadingMoves = true;

                        string[] moves = line.Split(' ');

                        foreach (string move in moves)
                        {
                            if (move == "") // double space before result
                            {
                                break;
                            }

                            if (move.Contains("."))
                            {
                                output.Append(move.Split('.')[1] + " ");
                            }
                            else
                            {
                                output.Append(move + " ");
                            }
                        }
                    }
                }
            }

            string outputPath = AssetDatabase.GetAssetPath(_mergedAndFormattedInputFile);

            StreamWriter writer = new StreamWriter(outputPath);

            writer.Write(output);
            writer.Close();

            timer.Stop();

            _resultText.text += DateTime.Now.ToString("HH:mm:ss") + ": Input merged and formatted successfully in " + timer.ElapsedMilliseconds / 1000 + "s\n";
        }

        public void CreateOpeningBookFile()
        {
            _resultText.text += DateTime.Now.ToString("HH:mm:ss") + ": Creating opening book file...\n";

            Stopwatch timer = new Stopwatch();
            timer.Start();

            StringReader reader = new StringReader(_mergedAndFormattedInputFile.text);

            Dictionary<string, HashSet<string>> positionsWithMoves = new Dictionary<string, HashSet<string>>(4096);
            string line;

            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
            {
                string currentFen = START_POSITION_FEN;
                int depthCounter = 1;

                string[] movesInAN = line.Split(' ');

                foreach (string moveInAN in movesInAN)
                {
                    if (depthCounter++ > _maxBookDepth)
                        break;

                    ChessEngine chessEngine = new ChessEngine(currentFen);

                    List<Move> legalMoves = chessEngine.GenerateLegalMoves();

                    string formattedMoveInAN = moveInAN.Replace("+", "").Replace("x", "").Replace("-", "");

                    bool didFindMove = false;
                    Move foundedMove = new Move();

                    foreach (Move move in legalMoves)
                    {
                        if (formattedMoveInAN == "OO" && move.Type == MoveType.Castle && move.NewSquare.Position.x == 6) // kingside castle
                        {
                            foundedMove = move;
                            didFindMove = true;
                            break;
                        }
                        else if (formattedMoveInAN == "OOO" && move.Type == MoveType.Castle && move.NewSquare.Position.x == 2) // queenside castle
                        {
                            foundedMove = move;
                            didFindMove = true;
                            break;
                        }
                        else if (!char.IsUpper(formattedMoveInAN[0]) && move.Piece.Type == PieceType.Pawn) // pawn move
                        {
                            if (formattedMoveInAN.Length == 3 && // move with capture
                                "abcdefgh".IndexOf(formattedMoveInAN[0]) == move.OldSquare.Position.x &&
                                "abcdefgh".IndexOf(formattedMoveInAN[1]) == move.NewSquare.Position.x
                            )
                            {
                                foundedMove = move;
                                didFindMove = true;
                                break;
                            }
                            else if (formattedMoveInAN.Length == 2 && // move without capture
                                     "abcdefgh".IndexOf(formattedMoveInAN[0]) == move.OldSquare.Position.x &&
                                     formattedMoveInAN[1].ToString() == (move.NewSquare.Position.y + 1).ToString()
                            )
                            {
                                foundedMove = move;
                                didFindMove = true;
                                break;
                            }
                            else if (formattedMoveInAN.Contains("=") && // promotion
                                     (move.NewSquare.Position.y == GraphicalBoard.BOTTOM_RANK_INDEX || move.NewSquare.Position.y == GraphicalBoard.TOP_RANK_INDEX) &&
                                     "abcdefgh".IndexOf(formattedMoveInAN[0]) == move.OldSquare.Position.x
                            )
                            {
                                if (formattedMoveInAN.Length == 5) // promotion with capture
                                {
                                    if ("abcdefgh".IndexOf(formattedMoveInAN[1]) != move.NewSquare.Position.x) // uncorrect end file
                                        continue;
                                }

                                if (!isCorrectPromotionType(formattedMoveInAN[formattedMoveInAN.Length - 1], move))
                                {
                                    continue;
                                }

                                foundedMove = move;
                                didFindMove = true;
                                break;
                            }
                        }
                        else if (char.IsUpper(formattedMoveInAN[0]) && move.Piece.Type != PieceType.Pawn) // piece diffrent than pawn
                        {
                            if (!IsCorrectPieceType(formattedMoveInAN[0], move))
                            {
                                continue;
                            }

                            char targetFile = formattedMoveInAN[formattedMoveInAN.Length - 2];
                            char targetRank = formattedMoveInAN[formattedMoveInAN.Length - 1];

                            if ("abcdefgh".IndexOf(targetFile) == move.NewSquare.Position.x && targetRank.ToString() == (move.NewSquare.Position.y + 1).ToString()) // correct new square position
                            {
                                if (formattedMoveInAN.Length == 4) // additional info in case of disambiguation
                                {
                                    char disambiguationChar = formattedMoveInAN[1];

                                    if ("abcdefgh".Contains(disambiguationChar.ToString())) // file disambiguation
                                    {
                                        if ("abcdefgh".IndexOf(disambiguationChar) == move.OldSquare.Position.x) // correct starting file
                                        {
                                            foundedMove = move;
                                            didFindMove = true;
                                            break;
                                        }
                                    }
                                    else // rank disambiguation
                                    {
                                        if (disambiguationChar.ToString() == (move.OldSquare.Position.y + 1).ToString()) // correct starting rank
                                        {
                                            foundedMove = move;
                                            didFindMove = true;
                                            break;
                                        }

                                    }
                                }
                                else
                                {
                                    foundedMove = move;
                                    didFindMove = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!didFindMove)
                    {
                        UnityEngine.Debug.Log(line);
                        throw new Exception("Move " + moveInAN + " isn't legal");
                    }
                    else
                    {
                        //UnityEngine.Debug.Log(foundedMove);
                        if (positionsWithMoves.ContainsKey(currentFen.Substring(0, currentFen.Length - 4)))
                        {
                            positionsWithMoves[currentFen.Substring(0, currentFen.Length - 4)].Add(SimplifiedAlgebraicNotation.MoveToLongSAN(foundedMove));
                        }
                        else
                        {
                            positionsWithMoves.Add(currentFen.Substring(0, currentFen.Length - 4), new HashSet<string>());
                            positionsWithMoves[currentFen.Substring(0, currentFen.Length - 4)].Add(SimplifiedAlgebraicNotation.MoveToLongSAN(foundedMove));
                        }

                        chessEngine.MakeMove(foundedMove);
                        currentFen = chessEngine.FEN();
                    }
                }
            }

            // convert dictionary results into string
            StringBuilder output = new StringBuilder();
            foreach (var positionAndMoves in positionsWithMoves)
            {
                output.Append(positionAndMoves.Key + "\n");
                foreach (string move in positionAndMoves.Value)
                    output.Append(move + " ");
                output.Remove(output.Length - 1, 1); // remove last space
                output.Append("\n");
            }
            output.Remove(output.Length - 1, 1); // remove last empty line

            // save string with results into file
            string outputPath = AssetDatabase.GetAssetPath(_bookFile);
            StreamWriter writer = new StreamWriter(outputPath);
            writer.Write(output);
            writer.Close();

            timer.Stop();
            _resultText.text += DateTime.Now.ToString("HH:mm:ss") + ": Created opening book file successfully in " + timer.ElapsedMilliseconds / 1000 + "s\n";
        }

        bool isCorrectPromotionType(char promotionSymbol, Move move)
        {
            switch (promotionSymbol)
            {
                case 'Q':
                    if (move.Type == MoveType.PromotionToQueen)
                        return true;
                    break;
                case 'R':
                    if (move.Type == MoveType.PromotionToRook)
                        return true;
                    break;
                case 'B':
                    if (move.Type == MoveType.PromotionToBishop)
                        return true;
                    break;
                case 'N':
                    if (move.Type == MoveType.PromotionToKnight)
                        return true;
                    break;
            }
            return false;
        }

        bool IsCorrectPieceType(char pieceSymbol, Move move)
        {
            switch (pieceSymbol)
            {
                case 'N':
                    if (move.Piece.Type == PieceType.Knight)
                        return true;
                    break;
                case 'B':
                    if (move.Piece.Type == PieceType.Bishop)
                        return true;
                    break;
                case 'R':
                    if (move.Piece.Type == PieceType.Rook)
                        return true;
                    break;
                case 'Q':
                    if (move.Piece.Type == PieceType.Queen)
                        return true;
                    break;
                case 'K':
                    if (move.Piece.Type == PieceType.King)
                        return true;
                    break;
            }
            return false;
        }
    }
}

#endif
