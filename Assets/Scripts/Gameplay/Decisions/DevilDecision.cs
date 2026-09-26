using System.Collections.Generic;
using Boards;
using Pieces;
using Pieces.NPCs;
using Util;

namespace Gameplay.Decisions
{
    #nullable enable

    public class DevilDecision : Decision
    {
        public const string ID = "devil";

        public const int GoldForPlayer   = 10;
        public const int GoldForOpponent = 5;

        private static readonly string[] Options =
        {
            "Release me",
            "Remove any rule",
            "Smite any piece",
            $"You get {GoldForPlayer} gold. Your opponent gets {GoldForOpponent}"
        };

        public DevilDecision(Piece.Color player) : base(player) {}

        public override string GetId() => ID;

        public override string GetTitle() => "The Devil";

        public override string? GetIcon() => Devil.ID;

        public override IReadOnlyList<string> GetOptions(GameState game) => Options;

        public override void Resolve(GameState game, int option, Position? target)
        {
            switch (option)
            {
                case 0:
                    if (game.TryGetBoard(BoardType.Normal, out Board? normal))
                        normal.ForcePlace(new AggroDevil(), new Position(3, 3));
                    break;

                case 1:
                    game.PushDecision(new RemoveRuleDecision(player));
                    break;

                case 2:
                    game.PushDecision(new SmiteDecision(player));
                    break;

                case 3:
                    game.AddGold(player, GoldForPlayer);
                    game.AddGold(GameState.GetOpponent(player), GoldForOpponent);
                    break;
            }
        }
    }
}
