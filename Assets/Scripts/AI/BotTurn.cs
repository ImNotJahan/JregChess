using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Gameplay.Commands;

namespace AI
{
    #nullable enable

    /// <summary>
    /// A bot working out its command in small steps, so the work can be spread across
    /// frames where there are no threads (the web), or run to the end on another thread.
    /// </summary>
    /// <remarks>
    /// The steps are an <see cref="IEnumerator"/> like a coroutine: each yield is a place
    /// it can pause, and yielding another <see cref="IEnumerator"/> runs that to the end
    /// before carrying on.
    /// </remarks>
    public sealed class BotTurn
    {
        private readonly Stack<IEnumerator> steps = new();

        private GameCommand? command;
        private bool         done;

        public BotTurn(Func<BotTurn, IEnumerator> think) => steps.Push(think(this));

        public bool IsDone() => done;

        /// <summary>
        /// The bot's answer, once <see cref="IsDone"/>. Null if it has nothing to do.
        /// </summary>
        public GameCommand? GetCommand() => command;

        /// <summary>
        /// For the bot to give its answer.
        /// </summary>
        public void SetCommand(GameCommand? answer) => command = answer;

        /// <summary>
        /// Works for about <paramref name="milliseconds"/>, or until done.
        /// </summary>
        /// <returns>Whether it's done.</returns>
        public bool Work(double milliseconds)
        {
            Stopwatch timer = Stopwatch.StartNew();

            while (!done && timer.Elapsed.TotalMilliseconds < milliseconds) Step();

            return done;
        }

        /// <summary>
        /// Works until done, all at once.
        /// </summary>
        public GameCommand? Finish(CancellationToken cancellation = default)
        {
            while (!done)
            {
                cancellation.ThrowIfCancellationRequested();

                Step();
            }

            return command;
        }

        private void Step()
        {
            IEnumerator current = steps.Peek();

            if (current.MoveNext())
            {
                if (current.Current is IEnumerator nested) steps.Push(nested);

                return;
            }

            steps.Pop();

            if (steps.Count == 0) done = true;
        }
    }
}
