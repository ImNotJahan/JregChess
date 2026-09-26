using System.Collections.Generic;
using Boards;

namespace Pieces.NPCs
{
    public class AggroAngel : Npc
    {
        public const string ID = "aggro-angel";

        public override string GetId() => ID;

        public override string GetDescription() => "";

        public override int GetWidth () => 2;
        public override int GetHeight() => 2;

        /// <summary>
        /// Blows every 1x1 piece on the board one square in a random direction.
        /// </summary>
        public override void OnTurnEnded(Board board)
        {
            (int dx, int dy) = board.GetGame().GetRandom().Range(0, 3) switch
            {
                0 => (1, 0),
                1 => (-1, 0),
                2 => (0, 1),
                _ => (0, -1)
            };

            board.GetGame().Announce("The winds blow!", "winds");

            // furthest downwind first
            List<Piece> order = new(board.GetPieces());
            
            order.Sort((a, b) => (b.GetPosition().GetX() * dx + b.GetPosition().GetY() * dy)
                       .CompareTo(a.GetPosition().GetX() * dx + a.GetPosition().GetY() * dy));

            foreach (Piece piece in order)
            {
                if (piece.GetWidth() != 1 || piece.GetHeight() != 1) continue;

                board.Relocate(piece, piece.GetPosition().MoveBy(dx, dy));
            }
        }
    }
}
