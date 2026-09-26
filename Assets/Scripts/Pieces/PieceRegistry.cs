using System;
using System.Collections.Generic;
using Pieces.NPCs;
using Pieces.Shop;
using Pieces.Upgrades;

namespace Pieces
{
    #nullable enable

    public static class PieceRegistry
    {
        private static readonly Dictionary<string, Func<Piece.Color, Piece>> constructors = new()
        {
            [Pawn  .ID] = color => new Pawn  (color),
            [Rook  .ID] = color => new Rook  (color),
            [Knight.ID] = color => new Knight(color),
            [Bishop.ID] = color => new Bishop(color),
            [Queen .ID] = color => new Queen (color),
            [King  .ID] = color => new King  (color),

            [SuicideBomber.ID] = color => new SuicideBomber(color),
            [Centaur      .ID] = color => new Centaur      (color),
            [Unicorn      .ID] = color => new Unicorn      (color),
            [TrojanHorse  .ID] = color => new TrojanHorse  (color),
            [RookKnight   .ID] = color => new RookKnight   (color),
            [BishopKnight .ID] = color => new BishopKnight (color),
            [KnightQueen  .ID] = color => new KnightQueen  (color),
            [Necromancer  .ID] = color => new Necromancer  (color),
            [SuperBishop  .ID] = color => new SuperBishop  (color),
            [SuperKing    .ID] = color => new SuperKing    (color),
            [BallQueen    .ID] = color => new BallQueen    (color),
            [AngryRook    .ID] = color => new AngryRook    (color),
            [RookTower    .ID] = color => new RookTower    (color),

            [Zebra  .ID] = color => new Zebra  (color),
            [Giraffe.ID] = color => new Giraffe(color),
            [Jester .ID] = color => new Jester (color),

            [Coin      .ID] = _ => new Coin      (),
            [Treasure  .ID] = _ => new Treasure  (),
            [Portal    .ID] = _ => new Portal    (),
            [Angel     .ID] = _ => new Angel     (),
            [AggroAngel.ID] = _ => new AggroAngel(),
            [Atheism   .ID] = _ => new Atheism   (),
            [Church    .ID] = _ => new Church    (),
            [Devil     .ID] = _ => new Devil     (),
            [AggroDevil.ID] = _ => new AggroDevil(),
            [Bomb      .ID] = _ => new Bomb      (),
            [Landmine  .ID] = _ => new Landmine  (),
            [Pittrap   .ID] = _ => new Pittrap   (),
            [Whirlpool .ID] = _ => new Whirlpool (),
            [TheVoid   .ID] = _ => new TheVoid   (),
            [Meteor    .ID] = _ => new Meteor    (),
            [WildHorse .ID] = _ => new WildHorse (),
            [Wildlife  .ID] = _ => new Wildlife  (),
            [Zombie    .ID] = _ => new Zombie    (),
        };

        public static Piece Create(string id, Piece.Color color)
        {
            if (!constructors.TryGetValue(id, out Func<Piece.Color, Piece>? constructor))
                throw new ArgumentException($"No piece registered with id {id}");

            return constructor(color);
        }

        public static bool IsRegistered(string id) => constructors.ContainsKey(id);
    }
}
