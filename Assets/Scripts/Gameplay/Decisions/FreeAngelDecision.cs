using System.Collections.Generic;
using Boards;
using Newtonsoft.Json.Linq;
using Pieces;
using Pieces.NPCs;
using Util;

namespace Gameplay.Decisions
{
    #nullable enable

    /// <summary>
    /// Freeing the angel takes it out of Heaven and puts an
    /// <see cref="AggroAngel"/> on the Normal board.
    /// </summary>
    public class FreeAngelDecision : Decision
    {
        public const string ID = "free-angel";

        private static readonly string[] Options = { "Free him", "Leave him be" };

        private readonly Position angel;

        /// <param name="angel">Any square the angel covers.</param>
        public FreeAngelDecision(Piece.Color player, Position angel) : base(player)
        {
            this.angel = angel;
        }

        public override string GetId() => ID;

        public override string GetTitle() => "Free him?";

        public override string? GetIcon() => AggroAngel.ID;

        public override IReadOnlyList<string> GetOptions(GameState game) => Options;

        public override void Resolve(GameState game, int option, Position? target)
        {
            if (option != 0) return;
            if (!game.TryGetBoard(angel.GetLocation(), out Board? heaven)) return;
            if (heaven.GetPieceAt(angel) is not Angel captive) return;
            if (!game.TryGetBoard(BoardType.Normal, out Board? normal)) return;

            heaven.RemovePiece(captive);

            normal.ForcePlace(new AggroAngel(), new Position(3, 3));
        }

        protected override void SerializeData(JObject obj) => obj["angel"] = angel.Serialize();

        public static FreeAngelDecision DeserializeData(JObject obj) =>
            new(DeserializePlayer(obj), Position.Deserialize(obj["angel"]!));
    }
}
