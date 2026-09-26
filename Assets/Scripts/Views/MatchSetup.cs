using AI;
using Pieces;
using UnityEngine;

namespace Views
{
    #nullable enable

    /// <summary>
    /// What the main menu chose, read by the Game scene.
    /// </summary>
    public static class MatchSetup
    {
        private static Bot?        bot;
        private static Piece.Color botColor;
        private static int         botDepth = HeuristicBot.DefaultDepth;

        public static Bot?        GetBot     () => bot;
        public static Piece.Color GetBotColor() => botColor;
        public static int         GetBotDepth() => botDepth;

        public static void SetBotDepth(int depth) => botDepth = depth;

        public static void SetLocal() => bot = null;

        public static void SetBot(Bot newBot, Piece.Color color)
        {
            bot      = newBot;
            botColor = color;

            if (bot is HeuristicBot heuristic) heuristic.SetDepthLimit(botDepth);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => bot = null;
    }
}
