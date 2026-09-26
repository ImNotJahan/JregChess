using System;
using System.Collections.Generic;
using Boards;
using Util;

namespace Pieces
{
    public static class Movement
    {
        public static bool IsKingMove(int dx, int dy) => Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1;

        public static bool IsKnightMove(int dx, int dy) =>
            (Math.Abs(dx) == 2 && Math.Abs(dy) == 1) ||
            (Math.Abs(dx) == 1 && Math.Abs(dy) == 2);

        public static bool IsStraight(int dx, int dy) => (dx == 0) != (dy == 0);

        public static bool IsDiagonal(int dx, int dy) => dx != 0 && Math.Abs(dx) == Math.Abs(dy);

        /// <summary>
        /// Squares strictly between the two, which must be on a straight or diagonal
        /// line.
        /// </summary>
        public static IEnumerable<Position> GetSquaresBetween(Position from, Position to)
        {
            int stepX = Math.Sign(to.GetX() - from.GetX());
            int stepY = Math.Sign(to.GetY() - from.GetY());

            for (Position square = from.MoveBy(stepX, stepY); !square.SameSquare(to); square = square.MoveBy(stepX, stepY))
                yield return square;
        }

        /// <summary>
        /// Number of occupied squares strictly between the two, which must be on a
        /// straight or diagonal line.
        /// </summary>
        public static int CountBetween(Board board, Position from, Position to)
        {
            int count = 0;

            foreach (Position square in GetSquaresBetween(from, to))
                if (board.HasPieceAt(square)) count++;

            return count;
        }

        public static bool IsClearLine(Board board, Position from, Position to)
        {
            int dx = to.GetX() - from.GetX();
            int dy = to.GetY() - from.GetY();

            return (IsStraight(dx, dy) || IsDiagonal(dx, dy)) && CountBetween(board, from, to) == 0;
        }
    }
}
