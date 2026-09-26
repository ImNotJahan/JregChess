#nullable enable

using System.Collections.Generic;
using Gameplay.Commands;

namespace AI
{
    public sealed class Jahan : HeuristicBot
    {
        public override string GetName() => "Jahan";

        protected override ResolveDecisionCommand HandleDecision(List<ResolveDecisionCommand> choices)
        {
            return choices[0];
        }
    }
}