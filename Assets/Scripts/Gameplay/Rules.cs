using System;
using System.Collections.Generic;
using Boards;
using Pieces;
using Pieces.NPCs;
using Pieces.Shop;
using Pieces.Upgrades;
using Util;

namespace Gameplay
{
    #nullable enable

    public static class Rules
    {
        public static readonly IReadOnlyList<Rule> Starting = new[]
        {
            Rule.KingDiesInHell,
            Rule.SuicideBomberHeaven
        };

        /// <summary>
        /// Rules players can pick from, each only once.
        /// </summary>
        public static readonly IReadOnlyList<Rule> Selectable = new[]
        {
            Rule.EveryoneUpgrades,
            Rule.PawnsMoveFour,
            Rule.BishopsGainNecromancy,
            Rule.GoldRush,
            Rule.MeteorShower,
            Rule.ZombieApocalypse,
            Rule.WildLife,
            Rule.WildHorse,
            Rule.NextPieceExplodes,
            Rule.Unicorns,
            Rule.PortalsOpen,
            Rule.PawnUpgrade,
            Rule.Whirlpool,
            Rule.Landmines,
            Rule.MoreGold,
            Rule.Void,
            Rule.Pittraps,
            Rule.Treasure
        };

        /// <summary>
        /// Events happen once and aren't kept in the list of active rules.
        /// </summary>
        public static bool IsEvent(Rule rule) => rule is
            Rule.MoreGold or Rule.EveryoneUpgrades or Rule.BishopsGainNecromancy;

        public static string GetDescription(Rule rule) => rule switch
        {
            Rule.KingDiesInHell        => "Kings must die in Hell",
            Rule.SuicideBomberHeaven   => "Suicide bombers go to Heaven",
            Rule.NextPieceExplodes     => "The next piece taken explodes",
            Rule.PawnsMoveFour         => "Pawns can move up to four squares on their first move",
            Rule.BishopsGainNecromancy => "Bishops gain necromancy",
            Rule.ZombieApocalypse      => "Zombie apocalypse",
            Rule.WildLife              => "Wildlife appears",
            Rule.WildHorse             => "A wild horse appears",
            Rule.Whirlpool             => "A whirlpool appears",
            Rule.Landmines             => "Landmines spawn",
            Rule.Void                  => "VOIDVOIDVOIDVOID",
            Rule.Pittraps              => "Pittraps spawn",
            Rule.EveryoneUpgrades      => "Everyone upgrades some pieces",
            Rule.GoldRush              => "Gold rush",
            Rule.MeteorShower          => "Meteor shower",
            Rule.Unicorns              => "Knights become unicorns",
            Rule.PortalsOpen           => "Portals appear",
            Rule.PawnUpgrade           => "A pawn on each side becomes a centaur",
            Rule.MoreGold              => "Everyone gets 10 gold",
            Rule.Treasure              => "Treasure appears",
            _                          => rule.ToString()
        };

        /// <summary>
        /// Runs the effects of the rule being added.
        /// </summary>
        public static void Apply(Rule rule, GameState game)
        {
            if (!game.TryGetBoard(BoardType.Normal, out Board? normal)) return;

            Rng random = game.GetRandom();

            switch (rule)
            {
                case Rule.MoreGold:
                    game.AddGold(Piece.Color.White, 10);
                    game.AddGold(Piece.Color.Black, 10);
                    game.Announce("Everyone gets 10 gold!", "coin");
                    break;

                case Rule.GoldRush:
                    SpawnRandomly(normal, random, () => new Coin(), 5);
                    game.Announce("Gold rush!", "coin");
                    break;

                case Rule.Landmines:
                    SpawnRandomly(normal, random, () => new Landmine(), 3);
                    break;

                case Rule.Pittraps:
                    SpawnRandomly(normal, random, () => new Pittrap(), 3);
                    break;

                case Rule.Unicorns:
                    ReplaceAll(normal, Knight.ID, piece => new Unicorn(piece.GetColor()));
                    break;

                case Rule.BishopsGainNecromancy:
                    ReplaceAll(normal, Bishop.ID, piece => new Necromancer(piece.GetColor()));
                    break;

                case Rule.PortalsOpen:
                    normal.ForcePlace(new Portal(BoardType.Heaven), new Position(0, 3));
                    normal.ForcePlace(new Portal(BoardType.Hell),   new Position(7, 4));
                    break;

                case Rule.PawnUpgrade:
                    UpgradeFirstPawn(normal, Piece.Color.White);
                    UpgradeFirstPawn(normal, Piece.Color.Black);
                    break;

                case Rule.Treasure:
                    normal.ForcePlace(new Treasure(), new Position(random.Range(0, 7), random.Range(3, 4)));
                    break;

                case Rule.WildLife:
                    normal.ForcePlace(new Wildlife(true),  new Position(0, 4));
                    normal.ForcePlace(new Wildlife(false), new Position(7, 3));
                    break;

                case Rule.WildHorse:
                    normal.ForcePlace(new WildHorse(), new Position(4, 4));
                    break;

                case Rule.ZombieApocalypse:
                    normal.ForcePlace(new Zombie(true),  new Position(0, 3));
                    normal.ForcePlace(new Zombie(true),  new Position(0, 4));
                    normal.ForcePlace(new Zombie(false), new Position(7, 3));
                    normal.ForcePlace(new Zombie(false), new Position(7, 4));
                    break;

                case Rule.EveryoneUpgrades:
                    UpgradeRandomly(normal, random, 8);
                    break;

                case Rule.Whirlpool:
                    normal.ForcePlace(new Whirlpool(), new Position(3 + random.Range(0, 1), 3 + random.Range(0, 1)));
                    break;

                case Rule.Void:
                    normal.ForcePlace(new TheVoid(), new Position(3, 3));
                    break;

                case Rule.MeteorShower:
                    normal.ForcePlace(new Meteor(), new Position(0, 5));
                    break;
            }
        }

        /// <summary>
        /// Tries <paramref name="attempts"/> random squares, skipping occupied ones.
        /// </summary>
        private static void SpawnRandomly(Board board, Rng random, Func<Piece> create, int attempts)
        {
            for (int i = 0; i < attempts; i++)
            {
                Position at = new(random.Range(0, board.GetWidth() - 1), random.Range(0, board.GetHeight() - 1));

                board.AddPiece(create(), at);
            }
        }

        private static void ReplaceAll(Board board, string id, Func<Piece, Piece> replace)
        {
            foreach (Piece piece in new List<Piece>(board.GetPieces()))
                if (piece.GetId() == id) board.ReplacePiece(piece, replace(piece));
        }

        /// <summary>
        /// Scans from Black's side.
        /// </summary>
        private static void UpgradeFirstPawn(Board board, Piece.Color color)
        {
            for (int y = board.GetHeight() - 1; y >= 0; y--)
            {
                for (int x = 0; x < board.GetWidth(); x++)
                {
                    Piece? piece = board.GetPieceAt(new Position(x, y));

                    if (piece == null || piece.GetId() != Pawn.ID || piece.GetColor() != color) continue;

                    board.ReplacePiece(piece, new Centaur(color));

                    return;
                }
            }
        }

        /// <summary>
        /// Upgrades the player pieces on <paramref name="attempts"/> random squares.
        /// </summary>
        private static void UpgradeRandomly(Board board, Rng random, int attempts)
        {
            for (int i = 0; i < attempts; i++)
            {
                Position at    = new(random.Range(0, board.GetWidth() - 1), random.Range(0, board.GetHeight() - 1));
                Piece?   piece = board.GetPieceAt(at);

                if (piece == null || !piece.IsPlayerPiece()) continue;

                Piece.Color color = piece.GetColor();

                Piece upgraded = piece.GetId() switch
                {
                    Pawn  .ID => new SuicideBomber(color),
                    Knight.ID => new TrojanHorse  (color),
                    Rook  .ID => new RookTower    (color),
                    Queen .ID => new BallQueen    (color),
                    King  .ID => new King         (color),
                    Bishop.ID => new Necromancer  (color),
                    _         => new Jester       (color)
                };

                board.ReplacePiece(piece, upgraded);
            }
        }
    }
}
