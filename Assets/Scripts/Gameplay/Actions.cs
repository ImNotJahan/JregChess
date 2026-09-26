using System.Collections.Generic;
using System.Linq;
using Boards;
using Gameplay.Commands;
using Gameplay.Decisions;
using Pieces;
using Util;

namespace Gameplay
{
    #nullable enable

    public static class Actions
    {
        public static bool HasAny(GameState state, Piece.Color player)
        {
            if (state.GetDecision() != null) return GetDecisionChoices(state, player).Count > 0;

            if (state.GetTurn() != player) return false;

            foreach (Board board in state.GetBoards())
            foreach (Piece piece in board.GetPieces().Where(piece => piece.GetColor() == player))
            {
                if (board.GetValidMoves(piece).Any()) return true;
            }

            return GetBuys(state, player).Count > 0 || GetUpgrades(state, player).Count > 0;
        }

        public static List<MoveCommand> GetMoves(GameState state, Piece.Color player)
        {
            List<MoveCommand> moves = new();

            foreach (Board    board in state.GetBoards())
            foreach (Piece    piece in board.GetPieces().Where(piece => piece.GetColor() == player))
            foreach (Position to    in board.GetValidMoves(piece))
            {
                moves.Add(new MoveCommand(player, piece.GetPosition(), to));
            }

            return moves;
        }

        public static List<BuyCommand> GetBuys(GameState state, Piece.Color player)
        {
            List<BuyCommand> buys = new();

            if (!state.TryGetBoard(BoardType.Normal, out Board? board)) return buys;

            int gold = state.GetGold(player);

            foreach (ShopItem item in ShopCatalog.Items.Where(item => item.Cost <= gold))
            {
                Piece piece = PieceRegistry.Create(item.Id, item.Neutral ? Piece.Color.NPC : player);

                foreach (Position at in board.GetFreeAnchors(piece)) buys.Add(new BuyCommand(player, item.Id, at));
            }

            buys.RemoveAll(buy => !buy.IsValid(state));

            return buys;
        }

        public static List<UpgradeCommand> GetUpgrades(GameState state, Piece.Color player)
        {
            List<UpgradeCommand> upgrades = new();

            if (state.GetGold(player) < UpgradeCatalog.Cost) return upgrades;

            foreach (Board board in state.GetBoards())
            foreach (Piece piece in board.GetPieces())
            {
                if (piece.GetColor() != player) continue;

                foreach (Upgrade upgrade in UpgradeCatalog.Upgrades)
                foreach (string from in upgrade.From.Where(from => from == piece.GetId()))
                {
                    upgrades.Add(new UpgradeCommand(player, upgrade.Id, piece.GetPosition()));
                }
            }

            upgrades.RemoveAll(upgrade => !upgrade.IsValid(state));

            return upgrades;
        }

        public static List<ResolveDecisionCommand> GetDecisionChoices(GameState state, Piece.Color player)
        {
            List<ResolveDecisionCommand> choices = new();
            Decision?                    decision = state.GetDecision();

            if (decision == null || decision.GetPlayer() != player) return choices;

            if (decision.GetTargetBoard() is BoardType type)
            {
                if (!state.TryGetBoard(type, out Board? board)) return choices;

                for (int x = 0; x < board.GetWidth (); x++)
                for (int y = 0; y < board.GetHeight(); y++)
                {
                    choices.Add(new ResolveDecisionCommand(player, 0, new Position(x, y, type)));
                }
            }
            else
            {
                for (int i = 0; i < decision.GetOptions(state).Count; i++) choices.Add(new ResolveDecisionCommand(player, i));
            }

            choices.RemoveAll(choice => !choice.IsValid(state));

            return choices;
        }
    }
}
