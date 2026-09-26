using Boards;
using Util;

namespace Pieces.NPCs
{
    public class WildHorse : Npc
    {
        public const string ID = "wild-horse";

        public override string GetId() => ID;

        public override bool IsValidMove(Board board, Position to) => true;

        /// <summary>
        /// Makes a random knight move.
        /// </summary>
        public override void OnTurnEnded(Board board)
        {
            Rng random = board.GetGame().GetRandom();

            int a = random.NextSign();
            int b = random.NextSign();

            board.MovePiece(this, random.NextBool() ? position.MoveBy(2 * a, b) : position.MoveBy(a, 2 * b));
        }
    }
}
