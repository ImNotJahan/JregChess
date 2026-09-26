using Gameplay;
using Pieces;
using Pieces.NPCs;
using Pieces.Upgrades;
using UnityEngine;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    public class UISprites
    {
        private readonly PieceSpriteLibrary pieces;
        private readonly IconLibrary        icons;

        public UISprites(PieceSpriteLibrary pieces, IconLibrary icons)
        {
            this.pieces = pieces;
            this.icons  = icons;
        }

        public Sprite GetPiece(string id, Piece.Color color) => pieces.GetSprite(id, color);

        /// <summary>
        /// Looks in the icons first, then the pieces.
        /// </summary>
        public Sprite? Get(string? id)
        {
            if (id == null) return null;

            Sprite? icon = icons.GetIcon(id);

            if (icon != null) return icon;

            return PieceRegistry.IsRegistered(id) ? pieces.GetSprite(id, Piece.Color.White) : null;
        }

        public Sprite? GetRule(Rule rule) => rule switch
        {
            Rule.KingDiesInHell        => Get("hell"),
            Rule.SuicideBomberHeaven   => GetPiece(SuicideBomber.ID, Piece.Color.White),
            Rule.NextPieceExplodes     => Get("explosion"),
            Rule.PawnsMoveFour         => GetPiece(Pawn.ID, Piece.Color.White),
            Rule.BishopsGainNecromancy => GetPiece(Necromancer.ID, Piece.Color.White),
            Rule.ZombieApocalypse      => Get(Zombie.ID),
            Rule.WildLife              => Get(Wildlife.ID),
            Rule.WildHorse             => Get(WildHorse.ID),
            Rule.Whirlpool             => Get(Whirlpool.ID),
            Rule.Landmines             => Get(Landmine.ID),
            Rule.Void                  => Get(TheVoid.ID),
            Rule.Pittraps              => Get(Pittrap.ID),
            Rule.EveryoneUpgrades      => Get("upgrade-icon"),
            Rule.GoldRush              => Get(Coin.ID),
            Rule.MeteorShower          => Get(Meteor.ID),
            Rule.Unicorns              => GetPiece(Unicorn.ID, Piece.Color.White),
            Rule.PortalsOpen           => Get(Portal.ID),
            Rule.PawnUpgrade           => GetPiece(Centaur.ID, Piece.Color.Black),
            Rule.MoreGold              => Get(Coin.ID),
            Rule.Treasure              => Get(Treasure.ID),
            _                          => null
        };

        /// <summary>
        /// "rook-knight" becomes "Rook knight".
        /// </summary>
        public static string FormatName(string id)
        {
            string name = id.Replace('-', ' ');

            return name.Length == 0 ? name : char.ToUpper(name[0]) + name[1..];
        }

        public static Image CreateImage(Sprite? sprite, string className)
        {
            Image image = new()
            {
                sprite      = sprite,
                scaleMode   = ScaleMode.ScaleToFit,
                pickingMode = PickingMode.Ignore
            };

            image.AddToClassList(className);

            return image;
        }
    }
}
