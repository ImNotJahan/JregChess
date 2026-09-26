using System.Collections.Generic;
using Gameplay;
using Gameplay.Commands;
using Pieces;

namespace AI
{
    #nullable enable

    public abstract class Bot
    {
        public abstract string GetName();

        public abstract GameCommand? HandleTurn(GameState state, Piece.Color player);

        protected static List<MoveCommand> GetMoves(GameState state, Piece.Color player) =>
            Actions.GetMoves(state, player);

        protected static List<BuyCommand> GetBuys(GameState state, Piece.Color player) =>
            Actions.GetBuys(state, player);

        protected static List<UpgradeCommand> GetUpgrades(GameState state, Piece.Color player) =>
            Actions.GetUpgrades(state, player);

        protected static List<ResolveDecisionCommand> GetDecisionChoices(GameState state, Piece.Color player) =>
            Actions.GetDecisionChoices(state, player);
    }
}
