#nullable enable

using System.Collections.Generic;
using Gameplay.Commands;
using Gameplay;
using Pieces;
using System.Linq;
using Util;

namespace AI
{
    public sealed class Jahan : HeuristicBot
    {
        public override string GetName() => "Jahan";

        private static readonly List<string> buys = new()
        {
            ""
        };

        protected override ResolveDecisionCommand HandleDecision(List<ResolveDecisionCommand> choices)
        {
            return choices[0];
        }

        /*protected override List<BuyCommand> GetBuyCandidates(GameState state, Piece.Color player, int ply)
        {
            if (ply == 0) return new();

            List<BuyCommand> candidates = new();

            foreach (Piece    piece in buys .Select  (x => PieceRegistry.Create(x, player)))
            foreach (Position at    in state.GetBoard(Boards.BoardType.Normal).GetFreeAnchors(piece))
            {
                candidates.Add(new BuyCommand(player, piece.GetId(), at));
            }

            return candidates;
            }*/
    }
}
