using System.Collections.Generic;
using Boards;
using Gameplay;
using Util;

namespace Pieces.NPCs
{
    #nullable enable

    public class TheVoid : Npc
    {
        public const string ID = "void";

        public override string GetId() => ID;

        public override int GetWidth () => 2;
        public override int GetHeight() => 2;

        /// <summary>
        /// Survives, sending the killer to a random empty spot on the Normal board.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            GameState game = board.GetGame();

            if (killer == null || !game.TryGetBoard(BoardType.Normal, out Board? normal)) return false;

            List<Position> free = normal.GetFreeAnchors(killer);

            if (free.Count > 0) game.Transfer(killer, BoardType.Normal, game.GetRandom().Pick(free));

            return false;
        }
    }
}
