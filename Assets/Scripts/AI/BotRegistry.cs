using System;
using System.Collections.Generic;
using System.Linq;

namespace AI
{
    #nullable enable

    public static class BotRegistry
    {
        private static readonly Func<Bot>[] factories =
        {
            () => new RandomBot(),
            () => new Jahan()
        };

        public static List<Bot> CreateAll() => factories.Select(create => create()).ToList();
    }
}
