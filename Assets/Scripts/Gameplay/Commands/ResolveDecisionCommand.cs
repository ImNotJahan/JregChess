using Gameplay.Decisions;
using Newtonsoft.Json.Linq;
using Pieces;
using Util;

namespace Gameplay.Commands
{
    #nullable enable

    /// <summary>
    /// Makes the current <see cref="Decision"/>. The only command allowed while one
    /// is pending.
    /// </summary>
    public class ResolveDecisionCommand : GameCommand
    {
        public const string ID = "resolve-decision";

        private readonly int       option;
        private readonly Position? target;

        public ResolveDecisionCommand(Piece.Color player, int option, Position? target = null) : base(player)
        {
            this.option = option;
            this.target = target;
        }

        public override string GetId() => ID;

        public override bool IsValid(GameState state)
        {
            Decision? decision = state.GetDecision();

            return decision != null &&
                   decision.GetPlayer() == player &&
                   decision.IsValid(state, option, target);
        }

        public override void Execute(GameState state) => state.ResolveDecision(option, target);

        protected override void SerializeData(JObject obj)
        {
            obj["option"] = option;
            obj["target"] = target?.Serialize();
        }

        public static ResolveDecisionCommand DeserializeData(JObject obj)
        {
            JToken? targetToken = obj["target"];

            return new ResolveDecisionCommand(
                DeserializePlayer(obj),
                obj.Value<int>("option"),
                targetToken == null || targetToken.Type == JTokenType.Null ? null : Position.Deserialize(targetToken)
            );
        }
    }
}
