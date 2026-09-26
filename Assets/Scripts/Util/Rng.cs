using System.Collections.Generic;

namespace Util
{
    /// <summary>
    /// SplitMix64. Resuming from <see cref="GetState"/> continues the same sequence.
    /// </summary>
    public class Rng
    {
        private ulong state;

        public Rng(ulong seed)
        {
            state = seed;
        }

        public ulong GetState() => state;

        /// <summary>
        /// Inclusive of both ends.
        /// </summary>
        public int Range(int min, int max) => min + (int)(Next() % (ulong)(max - min + 1));

        public bool NextBool() => (Next() & 1) == 0;

        /// <summary>
        /// 1 or -1
        /// </summary>
        public int NextSign() => NextBool() ? 1 : -1;

        public T Pick<T>(IReadOnlyList<T> list) => list[Range(0, list.Count - 1)];

        private ulong Next()
        {
            ulong z = state += 0x9E3779B97F4A7C15UL;

            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;

            return z ^ (z >> 31);
        }
    }
}
