using System.Collections.Generic;
using Pieces;
using Pieces.NPCs;
using Pieces.Shop;
using Pieces.Upgrades;

namespace Gameplay
{
    #nullable enable

    public readonly struct ShopItem
    {
        public readonly string Id;
        public readonly int    Cost;
        /// <summary>
        /// Placed as an NPC instead of the buyer's piece.
        /// </summary>
        public readonly bool   Neutral;

        public ShopItem(string id, int cost, bool neutral = false)
        {
            Id      = id;
            Cost    = cost;
            Neutral = neutral;
        }
    }

    public static class ShopCatalog
    {
        public static readonly IReadOnlyList<ShopItem> Items = new ShopItem[]
        {
            new(Pawn        .ID,  2),
            new(Rook        .ID,  7),
            new(Knight      .ID,  5),
            new(Bishop      .ID,  6),
            new(King        .ID,  9),
            new(Queen       .ID, 12),
            new(Zebra       .ID,  7),
            new(RookKnight  .ID,  9),
            new(BishopKnight.ID,  6),
            new(KnightQueen .ID, 15),
            new(Giraffe     .ID,  6),
            new(Landmine    .ID,  4, true),
            new(Bomb        .ID, 15, true),
            new(Jester      .ID,  8),
            new(AngryRook   .ID,  9),
            new(Unicorn     .ID,  6),
            new(Portal      .ID,  4, true),
        };

        public static bool TryGet(string id, out ShopItem item)
        {
            foreach (ShopItem candidate in Items)
            {
                if (candidate.Id != id) continue;

                item = candidate;

                return true;
            }

            item = default;

            return false;
        }
    }
}
