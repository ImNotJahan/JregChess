using Boards;
using Newtonsoft.Json.Linq;
using Pieces;
using Util;

namespace Gameplay.Commands
{
    #nullable enable

    /// <summary>
    /// Places a <see cref="ShopCatalog"/> item on the buyer's half of the Normal
    /// board. Doesn't end the turn.
    /// </summary>
    public class BuyCommand : GameCommand
    {
        public const string ID = "buy";

        private readonly string   item;
        private readonly Position at;

        /// <param name="at">The bought piece's bottom left.</param>
        public BuyCommand(Piece.Color player, string item, Position at) : base(player)
        {
            this.item = item;
            this.at   = at;
        }

        public override string GetId() => ID;

        public override bool IsValid(GameState state)
        {
            if (!ShopCatalog.TryGet(item, out ShopItem shopItem)) return false;
            if (state.GetGold(player) < shopItem.Cost) return false;
            if (at.GetLocation() != BoardType.Normal) return false;
            if (!state.TryGetBoard(BoardType.Normal, out Board? board)) return false;

            Piece piece = CreatePiece(shopItem);

            return IsOnOwnHalf(board, piece) && board.CanPlace(piece, at);
        }

        /// <summary>
        /// Whether the whole piece is on the buyer's half. White's half is the bottom.
        /// </summary>
        private bool IsOnOwnHalf(Board board, Piece piece)
        {
            int half = board.GetHeight() / 2;
            int top  = at.GetY() + piece.GetHeight() - 1;

            return player == Piece.Color.Black ? at.GetY() >= half : top < half;
        }

        public override void Execute(GameState state)
        {
            ShopCatalog.TryGet(item, out ShopItem shopItem);

            state.AddGold(player, -shopItem.Cost);
            state.GetBoard(BoardType.Normal).AddPiece(CreatePiece(shopItem), at);
        }

        private Piece CreatePiece(ShopItem shopItem) => PieceRegistry.Create(item, shopItem.Neutral ? Piece.Color.NPC : player);

        protected override void SerializeData(JObject obj)
        {
            obj["item"] = item;
            obj["at"]   = at.Serialize();
        }

        public static BuyCommand DeserializeData(JObject obj) => new(
            DeserializePlayer(obj),
            obj.Value<string>("item")!,
            Position.Deserialize(obj["at"]!)
        );
    }
}
