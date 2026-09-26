using AI;
using Networking;
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
        private static Bot?           bot;
        private static Piece.Color    botColor;
        private static int            botDepth = HeuristicBot.DefaultDepth;
        private static OnlineSession? online;
        private static string?        notice;

        public static Bot?        GetBot     () => bot;
        public static Piece.Color GetBotColor() => botColor;
        public static int         GetBotDepth() => botDepth;

        /// <summary>
        /// Null unless playing online, or if the session was left.
        /// </summary>
        public static OnlineSession? GetOnline() => online != null ? online : null;

        public static void SetBotDepth(int depth) => botDepth = depth;

        /// <summary>
        /// A message for the main menu to show, such as why a game ended.
        /// </summary>
        public static void SetNotice(string? text) => notice = text;

        /// <summary>
        /// Clears the notice.
        /// </summary>
        public static string? TakeNotice()
        {
            string? text = notice;

            notice = null;

            return text;
        }

        public static void SetLocal()
        {
            bot    = null;
            online = null;
        }

        public static void SetOnline(OnlineSession session)
        {
            bot    = null;
            online = session;
        }

        public static void SetBot(Bot newBot, Piece.Color color)
        {
            bot      = newBot;
            online   = null;
            botColor = color;

            if (bot is HeuristicBot heuristic) heuristic.SetDepthLimit(botDepth);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            SetLocal();

            notice = null;
        }
    }
}
