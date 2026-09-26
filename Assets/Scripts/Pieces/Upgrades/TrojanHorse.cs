using Boards;
using Util;

namespace Pieces.Upgrades
{
    #nullable enable

    public class TrojanHorse : Piece
    {
        public const string ID = "trojan-horse";

        private const int SpawnedPawns = 3;

        public TrojanHorse(Color color = default) : base(color) {}

        public override string GetId() => ID;

        public override string GetDescription() => "Carries a few surprises inside";

        /// <summary>
        /// One square forward.
        /// </summary>
        public override bool IsValidMove(Board board, Position to)
        {
            (int dx, int dy) = GetOffset(to);

            return dx == 0 && dy == (color == Color.Black ? -1 : 1);
        }

        /// <summary>
        /// Spawns pawns in the empty squares around it when it dies.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            if (!base.Kill(board, killer)) return false;

            int spawned = 0;

            for (int dy = 1; dy >= -1 && spawned < SpawnedPawns; dy--)
            {
                for (int dx = -1; dx <= 1 && spawned < SpawnedPawns; dx++)
                {
                    if (board.AddPiece(new Pawn(color), position.MoveBy(dx, dy))) spawned++;
                }
            }

            return true;
        }
    }
}
