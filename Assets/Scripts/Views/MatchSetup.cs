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

        public static Bot?        GetBot     () => bot;
        public static Piece.Color GetBotColor() => botColor;

        public static void SetLocal() => bot = null;

        public static void SetBot(Bot newBot, Piece.Color color)
        {
            bot      = newBot;
            botColor = color;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => bot = null;
    }
}
