using System;
using System.Collections.Generic;
using Apocryphon.Persistence;
using Boards;
using Newtonsoft.Json.Linq;
using Pieces;
using Util;

namespace Gameplay.Decisions
{
    #nullable enable

    /// <summary>
    /// A choice a player must make before the game continues. Picked either by
    /// option index, or by clicking a square if <see cref="GetTargetBoard"/> isn't
    /// null.
    /// </summary>
    public abstract class Decision : ISerializable
    {
        private static readonly Dictionary<string, Func<JObject, Decision>> deserializers = new()
        {
            [RuleDecision      .ID] = obj => new RuleDecision      (DeserializePlayer(obj)),
            [RemoveRuleDecision.ID] = obj => new RemoveRuleDecision(DeserializePlayer(obj)),
            [AtheismDecision   .ID] = obj => new AtheismDecision   (DeserializePlayer(obj)),
            [DevilDecision     .ID] = obj => new DevilDecision     (DeserializePlayer(obj)),
            [SmiteDecision     .ID] = obj => new SmiteDecision     (DeserializePlayer(obj)),
            [FreeAngelDecision .ID] = FreeAngelDecision.DeserializeData,
        };

        protected readonly Piece.Color player;

        protected Decision(Piece.Color player)
        {
            this.player = player;
        }

        /// <summary>
        /// Who makes the decision.
        /// </summary>
        public Piece.Color GetPlayer() => player;

        public abstract string GetId();

        public abstract string GetTitle();

        /// <summary>
        /// Id of an icon or piece to show with the decision.
        /// </summary>
        public virtual string? GetIcon() => null;

        public virtual IReadOnlyList<string> GetOptions(GameState game) => Array.Empty<string>();

        /// <summary>
        /// The board whose squares can be picked, or null if the decision is made by
        /// option.
        /// </summary>
        public virtual BoardType? GetTargetBoard() => null;

        public virtual bool IsValid(GameState game, int option, Position? target) =>
            option >= 0 && option < GetOptions(game).Count;

        /// <summary>
        /// Only called if <see cref="IsValid"/> returned true.
        /// </summary>
        public abstract void Resolve(GameState game, int option, Position? target);

        public JToken Serialize()
        {
            JObject obj = new()
            {
                ["id"]     = GetId(),
                ["player"] = player.ToString()
            };

            SerializeData(obj);

            return obj;
        }

        protected virtual void SerializeData(JObject obj) {}

        public static Decision Deserialize(JToken token)
        {
            JObject obj = (JObject)token;
            string  id  = obj.Value<string>("id")!;

            if (!deserializers.TryGetValue(id, out Func<JObject, Decision>? deserializer))
                throw new ArgumentException($"No decision registered with id {id}");

            return deserializer(obj);
        }

        protected static Piece.Color DeserializePlayer(JObject obj) =>
            Enum.Parse<Piece.Color>(obj.Value<string>("player")!);
    }
}
