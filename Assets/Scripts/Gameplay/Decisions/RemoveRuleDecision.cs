using System.Collections.Generic;
using Pieces;
using Util;

namespace Gameplay.Decisions
{
    #nullable enable

    public class RemoveRuleDecision : Decision
    {
        public const string ID = "remove-rule";

        public RemoveRuleDecision(Piece.Color player) : base(player) {}

        public override string GetId() => ID;

        public override string GetTitle() => "Remove any rule";

        public override string? GetIcon() => "devil";

        public override IReadOnlyList<string> GetOptions(GameState game)
        {
            List<string> options = new();

            foreach (Rule rule in game.GetRules())
                options.Add(Rules.GetDescription(rule));

            return options;
        }

        public override void Resolve(GameState game, int option, Position? target) =>
            game.RemoveRule(game.GetRules()[option]);
    }
}
