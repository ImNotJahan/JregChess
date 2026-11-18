using System.Collections;
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

        /// <summary>
        /// Starts working out <paramref name="player"/>'s next command. Nothing runs
        /// until the turn is worked on.
        /// </summary>
        public BotTurn StartTurn(GameState state, Piece.Color player) => new(turn => Think(state, player, turn));

        /// <summary>
        /// Works out the command in steps (see <see cref="BotTurn"/>), and gives it to
        /// <see cref="BotTurn.SetCommand"/>.
        /// </summary>
        protected abstract IEnumerator Think(GameState state, Piece.Color player, BotTurn turn);

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
