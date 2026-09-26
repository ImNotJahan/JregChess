using System;
using System.Collections.Generic;
using Apocryphon.Persistence;
using Newtonsoft.Json.Linq;
using Pieces;

namespace Gameplay.Commands
{
    #nullable enable

    public abstract class GameCommand : ISerializable
    {
        private static readonly Dictionary<string, Func<JObject, GameCommand>> deserializers = new()
        {
            [MoveCommand           .ID] = MoveCommand           .DeserializeData,
            [BuyCommand            .ID] = BuyCommand            .DeserializeData,
            [UpgradeCommand        .ID] = UpgradeCommand        .DeserializeData,
            [ResolveDecisionCommand.ID] = ResolveDecisionCommand.DeserializeData,
        };

        protected readonly Piece.Color player;

        protected GameCommand(Piece.Color player)
        {
            this.player = player;
        }

        /// <summary>
        /// Who sent the command.
        /// </summary>
        public Piece.Color GetPlayer() => player;

        public abstract string GetId();

        /// <summary>
        /// Doesn't check whose turn it is, or for pending decisions.
        /// </summary>
        public abstract bool IsValid(GameState state);

        /// <summary>
        /// Only called if <see cref="IsValid"/> returned true.
        /// </summary>
        public abstract void Execute(GameState state);

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

        protected abstract void SerializeData(JObject obj);

        public static GameCommand Deserialize(JToken token)
        {
            JObject obj = (JObject)token;
            string  id  = obj.Value<string>("id")!;

            if (!deserializers.TryGetValue(id, out Func<JObject, GameCommand>? deserializer))
            {
                throw new ArgumentException($"No command registered with id {id}");
            }

            return deserializer(obj);
        }

        protected static Piece.Color DeserializePlayer(JObject obj) => Enum.Parse<Piece.Color>(obj.Value<string>("player")!);
    }
}
