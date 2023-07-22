using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;


public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 10, 300, 300, 1000, 3000, 15000 };

    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();

        int nbIteration = 2;
        //if (allMoves.Length < 6) nbIteration = 3;
        //else if (allMoves.Length < 15) nbIteration = 3;


        // Pick a random move to play if nothing better is found
        Random rng = new();
        Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
        int highestValueCapture = 0;

        foreach (Move move in allMoves)
        {
            // Always play checkmate in one
            if (MoveIsCheckmate(board, move))
            {
                moveToPlay = move;
                break;
            }

            // Find highest value capture
            int capturedPieceValue = EvaluateRecursive(board, move, nbIteration, true);

            if (capturedPieceValue > highestValueCapture)
            {
                moveToPlay = move;
                highestValueCapture = capturedPieceValue;
            }
        }
        return moveToPlay;
    }

    int EvaluateRecursive(Board board, Move move, int depth, bool isMyMove)
    {
        Piece capturedPiece = board.GetPiece(move.TargetSquare);
        int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType] * (isMyMove ? 1 : -10);

        if (depth > 0)
        {
            board.MakeMove(move);
            if (!board.IsInCheckmate())
            {
                Move[] allMoves = board.GetLegalMoves();
                Random rng = new();
                int highestValueCapture = 0;
                foreach (Move nextMove in allMoves)
                {
                    int eval = EvaluateRecursive(board, nextMove, depth - (isMyMove ? 0 : 1), !isMyMove);
                    if (capturedPieceValue > highestValueCapture)
                    {
                        highestValueCapture = capturedPieceValue;
                    }
                }
                capturedPieceValue += highestValueCapture;
            }
            board.UndoMove(move);
        }

        return capturedPieceValue;
    }

    // Test if this move gives checkmate
    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}