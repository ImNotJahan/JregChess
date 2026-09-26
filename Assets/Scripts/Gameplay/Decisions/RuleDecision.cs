using System.Collections.Generic;
using Pieces;
using Util;

namespace Gameplay.Decisions
{
    #nullable enable

    /// <summary>
    /// Pick a new rule from the rule pool.
    /// </summary>
    public class RuleDecision : Decision
    {
        public const string ID = "rule";

        public RuleDecision(Piece.Color player) : base(player) {}

        public override string GetId() => ID;

        public override string GetTitle() => "Pick a new rule";

        public override IReadOnlyList<string> GetOptions(GameState game)
        {
            List<string> options = new();

            foreach (Rule rule in game.GetRulePool())
                options.Add(Rules.GetDescription(rule));

            return options;
        }

        public override void Resolve(GameState game, int option, Position? target) =>
            game.AddRule(game.GetRulePool()[option]);
    }
}
