using System.Linq;
using Boards;
using Newtonsoft.Json.Linq;
using Pieces;
using Util;

namespace Gameplay.Commands
{
    #nullable enable

    public class UpgradeCommand : GameCommand
    {
        public const string ID = "upgrade";

        private readonly string   upgrade;
        private readonly Position at;

        /// <param name="at">Any square the upgraded piece covers.</param>
        public UpgradeCommand(Piece.Color player, string upgrade, Position at) : base(player)
        {
            this.upgrade = upgrade;
            this.at      = at;
        }

        public override string GetId() => ID;

        public override bool IsValid(GameState state)
        {
            if (!UpgradeCatalog.TryGet(upgrade, out Upgrade info))      return false;
            if (state.GetGold(player) < UpgradeCatalog.Cost)            return false;
            if (!state.TryGetBoard(at.GetLocation(), out Board? board)) return false;

            Piece? piece = board.GetPieceAt(at);

            if (piece == null || piece.GetColor() != player) return false;
            if (!Contains(info, piece.GetId())) return false;

            return board.CanReplace(piece, PieceRegistry.Create(upgrade, player));
        }

        public override void Execute(GameState state)
        {
            Board board = state.GetBoard(at.GetLocation());

            state.AddGold(player, -UpgradeCatalog.Cost);
            board.ReplacePiece(board.GetPieceAt(at)!, PieceRegistry.Create(upgrade, player));
        }

        private static bool Contains(Upgrade info, string id)
        {
            foreach (string from in info.From.Where(from => from == id)) return true;

            return false;
        }

        protected override void SerializeData(JObject obj)
        {
            obj["upgrade"] = upgrade;
            obj["at"]      = at.Serialize();
        }

        public static UpgradeCommand DeserializeData(JObject obj) => new(
            DeserializePlayer(obj),
            obj.Value<string>("upgrade")!,
            Position.Deserialize(obj["at"]!)
        );
    }
}
