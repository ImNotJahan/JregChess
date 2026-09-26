using System;
using Boards;
using Newtonsoft.Json.Linq;

namespace Util
{
    /// <summary>
    /// What board and position on that board is a piece on. The default Position is
    /// at 0, 0 on the Normal board. (0, 0) is White's bottom left corner.
    /// </summary>
    public readonly struct Position : IEquatable<Position>
    {
        private readonly BoardType location;
        private readonly int       x;
        private readonly int       y;

        public Position(int x, int y, BoardType location = BoardType.Normal)
        {
            this.x        = x;
            this.y        = y;
            this.location = location;
        }

        public Position MoveTo(int x, int y) => new(x, y, location);

        public Position MoveBy(int x, int y) => new(this.x + x, this.y + y, location);

        public Position ChangeBoard(BoardType to) => new(x, y, to);

        public int       GetX       () => x;
        public int       GetY       () => y;
        public BoardType GetLocation() => location;

        /// <summary>
        /// Ignores which board the positions are on.
        /// </summary>
        public bool SameSquare(Position other) => x == other.x && y == other.y;

        // keeps the left hand side's board
        public static Position operator +(Position a, Position b) =>
            new(a.x + b.x, a.y + b.y, a.location);

        public static Position operator -(Position a, Position b) =>
            new(a.x - b.x, a.y - b.y, a.location);

        public static bool operator ==(Position a, Position b) => a.Equals(b);
        public static bool operator !=(Position a, Position b) => !a.Equals(b);

        public bool Equals(Position other) =>
            x == other.x && y == other.y && location == other.location;

        public override bool Equals(object obj) => obj is Position other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(x, y, location);

        public override string ToString() => $"({x}, {y}, {location})";

        public JToken Serialize() => new JObject
        {
            ["x"]     = x,
            ["y"]     = y,
            ["board"] = location.ToString()
        };

        public static Position Deserialize(JToken token) => new(
            token.Value<int>("x"),
            token.Value<int>("y"),
            Enum.Parse<BoardType>(token.Value<string>("board"))
        );
    }
}
