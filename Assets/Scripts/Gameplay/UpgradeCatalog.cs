using System.Collections.Generic;
using Pieces;
using Pieces.Upgrades;

namespace Gameplay
{
    #nullable enable

    public readonly struct Upgrade
    {
        public readonly string Id;
        /// <summary>
        /// Ids of the pieces which can be upgraded into this.
        /// </summary>
        public readonly IReadOnlyList<string> From;

        public Upgrade(string id, params string[] from)
        {
            Id   = id;
            From = from;
        }
    }

    public static class UpgradeCatalog
    {
        public const int Cost = 5;

        public static readonly IReadOnlyList<Upgrade> Upgrades = new Upgrade[]
        {
            new(SuicideBomber.ID, Pawn.ID),
            new(Centaur      .ID, Pawn.ID, Knight.ID),
            new(Unicorn      .ID, Knight.ID),
            new(TrojanHorse  .ID, Knight.ID),
            new(RookKnight   .ID, Knight.ID, Rook.ID),
            new(BishopKnight .ID, Bishop.ID, Knight.ID),
            new(Necromancer  .ID, Bishop.ID),
            new(SuperBishop  .ID, Bishop.ID),
            new(SuperKing    .ID, King.ID),
            new(BallQueen    .ID, Queen.ID),
            new(KnightQueen  .ID, Queen.ID, Knight.ID),
            new(AngryRook    .ID, Rook.ID),
        };

        public static bool TryGet(string id, out Upgrade upgrade)
        {
            foreach (Upgrade candidate in Upgrades)
            {
                if (candidate.Id != id) continue;

                upgrade = candidate;

                return true;
            }

            upgrade = default;

            return false;
        }
    }
}
