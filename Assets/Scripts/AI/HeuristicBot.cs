using System.Collections.Generic;
using Boards;
using Gameplay;
using Gameplay.Commands;
using Pieces;
using Pieces.NPCs;
using Pieces.Shop;
using Pieces.Upgrades;

#nullable enable

namespace AI
{
    public abstract class HeuristicBot : Bot 
    {
        // just a big number (bigger than our win score)
        private const int INFINITY = 100_000;

        protected int depthLimit = 2;

        public override GameCommand? HandleTurn(GameState state, Piece.Color player) => FindBestMove(state, player).bestMove;

        private (GameCommand? bestMove, int bestMoveScore) FindBestMove(
            GameState   state, 
            Piece.Color player, 
            int         depth = 0, 
            int         ply   = 0,
            int         alpha = -INFINITY, // best score we have so far
            int         beta  = INFINITY   // best score optimal opponent will allow
        )
        {
            if (depth == depthLimit || state.IsOver()) return (null, ScoreTerminal(state, player, depth));

            int          bestMoveScore = 0;
            GameCommand? bestMove      = null;

            foreach (GameCommand command in GetCandidates(state, player, ply))
            {
                // TODO it can see the future....
                GameState? newState = state.ExecuteOnCopy(command);

                if (newState is null) continue;

                int newDepth = command is MoveCommand ? depth + 1 : depth;

                Piece.Color next = newState.GetDecision()?.GetPlayer() ?? newState.GetTurn();

                int score = next == player
                    ? FindBestMove(newState, player, newDepth, ply + 1, alpha, beta).bestMoveScore
                    // we negate it as this finds the best score for *our opponent*, which is our worst score
                    : -FindBestMove(newState, next, newDepth, ply + 1, -beta, -alpha).bestMoveScore;

                if (bestMove is null || score > bestMoveScore)
                {
                    bestMoveScore = score;
                    bestMove      = command;

                    if (score > alpha) alpha = score;

                    // if we have a better score than opponent will allow, give up on this path
                    if (alpha >= beta) break;
                }
            }

            // couldn't do anything: just check what the board's like
            if (bestMove is null) return (null, ScoreTerminal(state, player, depth));

            return (bestMove, bestMoveScore);
        }

        private IEnumerable<GameCommand> GetCandidates(GameState state, Piece.Color player, int ply)
        {
            if (state.GetDecision() is not null)
            {
                foreach (ResolveDecisionCommand choice in GetDecisionCandidates(state, player, ply)) yield return choice;

                yield break;
            }

            foreach (MoveCommand    move    in GetMoves            (state, player     )) yield return move;
            foreach (BuyCommand     buy     in GetBuyCandidates    (state, player, ply)) yield return buy;
            foreach (UpgradeCommand upgrade in GetUpgradeCandidates(state, player, ply)) yield return upgrade;
        }

        protected virtual List<ResolveDecisionCommand> GetDecisionCandidates(GameState state, Piece.Color player, int ply)
        {
            List<ResolveDecisionCommand> choices = GetDecisionChoices(state, player);

            if (choices.Count == 0) return choices;

            return new List<ResolveDecisionCommand> { HandleDecision(choices) };
        }

        protected virtual List<BuyCommand> GetBuyCandidates(GameState state, Piece.Color player, int ply) => ply == 0 
                                                                                                           ? GetBuys(state, player) 
                                                                                                           : new List<BuyCommand>();

        protected virtual List<UpgradeCommand> GetUpgradeCandidates(GameState state, Piece.Color player, int ply) => ply == 0 
                                                                                                                   ? GetUpgrades(state, player) 
                                                                                                                   : new List<UpgradeCommand>();

        protected virtual int ScoreTerminal(GameState state, Piece.Color player, int depth)
        {
            if (state.GetWinner() is Piece.Color winner) return (1000 + (depthLimit - depth)) 
                                                              * (winner == player ? 1 : -1);

            if (state.IsOver()) return 0;

            int score = 0;

            Piece.Color opponent = GameState.GetOpponent(player);

            foreach (Board board in state.GetBoards())
            {
                int boardMultiplier = board.GetBoardType() switch
                {
                    BoardType.Normal => 3,
                    BoardType.Heaven => 1,
                    BoardType.Hell   => 1,
                    _                => 0
                };

                foreach (Piece piece in board.GetPieces())
                {
                    int colorMultiplier = piece.GetColor() != player ? piece.GetColor() == Piece.Color.NPC ? 0 : -1 : 1;

                    score += ScorePiece(piece) * colorMultiplier * boardMultiplier;
                }
            }

            score += state.GetGold(player) - state.GetGold(opponent);

            return score;
        }

        protected virtual int ScorePiece(Piece piece) => piece.GetId() switch
        {
            // standard
            Pawn  .ID => 1,
            Knight.ID => 3,
            Bishop.ID => 3,
            Rook  .ID => 5,
            Queen .ID => 9,
            King  .ID => 200,

            // shop
            Jester .ID => 2,
            Giraffe.ID => 3,
            Zebra  .ID => 3,

            // upgrades
            SuicideBomber.ID => 2,
            Necromancer  .ID => 3,
            Unicorn      .ID => 4,
            Centaur      .ID => 5,
            TrojanHorse  .ID => 5,
            SuperBishop  .ID => 5,
            AngryRook    .ID => 6,
            RookTower    .ID => 7,
            BishopKnight .ID => 7,
            RookKnight   .ID => 8,
            BallQueen    .ID => 11,
            KnightQueen  .ID => 12,
            SuperKing    .ID => 210,

            // NPCs
            Coin      .ID => 1,
            Treasure  .ID => 3,
            Zombie    .ID => 1,
            WildHorse .ID => 2,
            Wildlife  .ID => 2,
            Angel     .ID => 3,
            AggroAngel.ID => 3,
            Devil     .ID => 3,
            AggroDevil.ID => 3,
            Church    .ID => 2,
            Atheism   .ID => 2,
            Portal    .ID => 0,
            Whirlpool .ID => 0,
            TheVoid   .ID => 0,
            Pittrap   .ID => 0,
            Landmine  .ID => 0,
            Bomb      .ID => 0,
            Meteor    .ID => 0,

            _ => 1
        };
        
        protected abstract ResolveDecisionCommand HandleDecision(List<ResolveDecisionCommand> choices);
    }
}