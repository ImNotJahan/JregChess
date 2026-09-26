using System;
using System.Collections.Generic;
using Apocryphon.Persistence;
using Boards;
using Newtonsoft.Json.Linq;
using Util;

namespace Pieces
{
    public abstract class Piece : ISerializable
    {
        #nullable enable

        public enum Color { White, Black, NPC }

        /// <summary>
        /// Its color or health changed.
        /// </summary>
        public event Action? Changed;

        protected Color    color;
        protected Position position;

        protected int maxHealth = 1;
        protected int health;

        protected Piece(Color color = default)
        {
            this.color = color;

            Initialize();

            health = maxHealth;
        }

        /// <summary>
        /// Called on construction, before health is set to maxHealth.
        /// </summary>
        protected virtual void Initialize() {}

        /// <summary>
        /// Must match the piece's id in the <see cref="PieceRegistry"/>.
        /// </summary>
        public abstract string GetId();

        public abstract string GetDescription();

        /// <summary>
        /// In squares. The piece's position is its bottom left square.
        /// </summary>
        public virtual int GetWidth () => 1;
        public virtual int GetHeight() => 1;

        /// <summary>
        /// The board has already checked bounds, friendly pieces, and that
        /// <paramref name="to"/> isn't the current position.
        /// </summary>
        public abstract bool IsValidMove(Board board, Position to);

        public virtual void OnMoved(Board board, Position from) {}

        /// <summary>
        /// Called after the move which killed <paramref name="captured"/>.
        /// </summary>
        public virtual void OnCapture(Board board, Piece captured, Position from) {}

        /// <summary>
        /// Called on every piece at the end of each turn.
        /// </summary>
        public virtual void OnTurnEnded(Board board) {}

        /// <summary>
        /// Returns if the piece died. The board removes it afterwards.
        /// </summary>
        public virtual bool Kill(Board board, Piece? killer)
        {
            Hurt();

            return health == 0;
        }

        public void Hurt()
        {
            health = Math.Max(0, health - 1);

            Changed?.Invoke();
        }

        public void Heal()
        {
            health = maxHealth;

            Changed?.Invoke();
        }

        /// <summary>
        /// Where the piece goes when it dies on the Normal board, or null if it's
        /// gone for good.
        /// </summary>
        public virtual BoardType? GetAfterlife() => BoardType.Hell;

        /// <summary>
        /// Gold given to the killer's owner.
        /// </summary>
        public virtual int GetBounty(Board board) => board.GetBoardType() == BoardType.Normal ? 1 : 0;

        /// <summary>
        /// Its owner loses if it dies in Hell.
        /// </summary>
        public virtual bool IsRoyal() => false;

        public bool IsPlayerPiece() => color != Color.NPC;

        public bool IsEnemyOf(Piece other) => color != other.color;

        public IEnumerable<Position> GetFootprint(Position anchor)
        {
            for (int y = 0; y < GetHeight(); y++)
                for (int x = 0; x < GetWidth(); x++)
                    yield return anchor.MoveBy(x, y);
        }

        /// <summary>
        /// Only for use by <see cref="Board"/>.
        /// </summary>
        public void SetPosition(Position to) => position = to;

        public void SetColor(Color to)
        {
            color = to;

            Changed?.Invoke();
        }

        /// <summary>
        /// A copy without event listeners. Shallow, so subclasses with reference
        /// type fields must override this to copy them.
        /// </summary>
        public virtual Piece Clone()
        {
            Piece copy = (Piece)MemberwiseClone();

            copy.Changed = null;

            return copy;
        }

        public Color    GetColor    () => color;
        public Position GetPosition () => position;
        public int      GetHealth   () => health;
        public int      GetMaxHealth() => maxHealth;

        /// <summary>
        /// x and y from this piece's position to <paramref name="to"/>.
        /// </summary>
        protected (int dx, int dy) GetOffset(Position to) =>
            (to.GetX() - position.GetX(), to.GetY() - position.GetY());

        public JToken Serialize()
        {
            JObject obj = new()
            {
                ["id"]       = GetId(),
                ["color"]    = color.ToString(),
                ["position"] = position.Serialize(),
                ["health"]   = health
            };

            SerializeState(obj);

            return obj;
        }

        protected virtual void SerializeState(JObject obj) {}

        protected virtual void DeserializeState(JObject obj) {}

        public static Piece Deserialize(JToken token)
        {
            JObject obj = (JObject)token;

            Piece piece = PieceRegistry.Create(
                obj.Value<string>("id")!,
                Enum.Parse<Color>(obj.Value<string>("color")!)
            );

            piece.position = Position.Deserialize(obj["position"]!);
            piece.health   = obj.Value<int>("health");

            piece.DeserializeState(obj);

            return piece;
        }
    }
}
