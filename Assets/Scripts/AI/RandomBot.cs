using System;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Commands;
using Pieces;

namespace AI
{
    #nullable enable

    /// <summary>
    /// Makes a random valid move or decision.
    /// </summary>
    public class RandomBot : Bot
    {
        private readonly Random random = new();

        public override string GetName() => "Randy";

        public override GameCommand? HandleTurn(GameState state, Piece.Color player)
        {
            List<ResolveDecisionCommand> choices = GetDecisionChoices(state, player);

            if (choices.Count > 0) return choices[random.Next(choices.Count)];

            List<MoveCommand> moves = GetMoves(state, player);

            return moves.Count > 0 ? moves[random.Next(moves.Count)] : null;
        }
    }
}
