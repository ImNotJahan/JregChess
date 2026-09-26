using System;
using System.Collections.Generic;
using System.Linq;
using Apocryphon.Persistence;
using Gameplay;
using Newtonsoft.Json.Linq;
using Pieces;
using Util;

namespace Boards
{
    #nullable enable

    public class Board : ISerializable
    {
        public const int DefaultSize = 8;

        public enum MoveResult
        {
            Invalid,
            Moved,
            Blocked
        }

        public event Action<Piece>?           PieceAdded;
        public event Action<Piece>?           PieceRemoved;
        public event Action<Piece, Position>? PieceMoved;
        public event Action<Piece>?           PieceHurt;
        public event Action<Piece, Piece?>?   PieceKilled;
        public event Action<Position>?        Exploded;

        private readonly GameState   game;
        private readonly BoardType   type;
        private readonly int         width;
        private readonly int         height;
        private readonly Piece?[,]   squares;
        private readonly List<Piece> pieces = new();

        private readonly HashSet<Piece> dying = new();

        private bool exploding;

        public Board(GameState game, BoardType type, int width = DefaultSize, int height = DefaultSize)
        {
            this.game   = game;
            this.type   = type;
            this.width  = width;
            this.height = height;

            squares = new Piece?[width, height];
        }

        public GameState            GetGame     () => game;
        public BoardType            GetBoardType() => type;
        public int                  GetWidth    () => width;
        public int                  GetHeight   () => height;
        public IReadOnlyList<Piece> GetPieces   () => pieces;

        public bool IsInBounds(Position pos) =>
            pos.GetX() >= 0 && pos.GetX() < width &&
            pos.GetY() >= 0 && pos.GetY() < height;

        /// <summary>
        /// Whether the whole piece fits with its bottom left at
        /// <paramref name="anchor"/>.
        /// </summary>
        public bool IsInBounds(Piece piece, Position anchor) => IsInBounds(anchor) 
                                                             && IsInBounds(anchor.MoveBy(piece.GetWidth() - 1, piece.GetHeight() - 1));

        public Piece? GetPieceAt(Position pos) => IsInBounds(pos) 
                                                ? squares[pos.GetX(), pos.GetY()] 
                                                : null;

        public bool HasPieceAt(Position pos) => GetPieceAt(pos) != null;

        public bool Contains(Piece piece) => pieces.Contains(piece);

        /// <summary>
        /// Distinct other pieces <paramref name="piece"/> would overlap with its
        /// bottom left at <paramref name="anchor"/>.
        /// </summary>
        public List<Piece> GetPiecesUnder(Piece piece, Position anchor)
        {
            List<Piece> found = new();

            foreach (Position square in piece.GetFootprint(anchor))
            {
                Piece? other = GetPieceAt(square);

                if (other != null && other != piece && !found.Contains(other)) found.Add(other);
            }

            return found;
        }

        public bool CanPlace(Piece piece, Position anchor) => IsInBounds    (piece, anchor) 
                                                           && GetPiecesUnder(piece, anchor).Count == 0;

        /// <summary>
        /// Bottom lefts where <paramref name="piece"/> can be placed.
        /// </summary>
        public List<Position> GetFreeAnchors(Piece piece)
        {
            List<Position> free = new();

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width;  x++)
            {
                Position anchor = new(x, y, type);

                if (CanPlace(piece, anchor)) free.Add(anchor);
            }

            return free;
        }

        /// <summary>
        /// Fails instead of capturing if the squares are occupied.
        /// </summary>
        public bool AddPiece(Piece piece, Position anchor)
        {
            if (Contains(piece))          return false;
            if (!CanPlace(piece, anchor)) return false;

            piece.SetPosition(anchor.ChangeBoard(type));

            Fill(piece, piece);
            pieces.Add(piece);

            PieceAdded?.Invoke(piece);

            return true;
        }

        /// <summary>
        /// Captures whatever is in the way, then adds the piece if its squares were
        /// cleared.
        /// </summary>
        public bool ForcePlace(Piece piece, Position anchor)
        {
            if (Contains(piece)) return false;
            if (!IsInBounds(piece, anchor)) return false;

            foreach (Piece other in GetPiecesUnder(piece, anchor))
                Capture(other, piece);

            return AddPiece(piece, anchor);
        }

        /// <summary>
        /// Doesn't kill the piece.
        /// </summary>
        public bool RemovePiece(Piece piece)
        {
            if (!pieces.Remove(piece)) return false;

            Fill(piece, null);

            PieceRemoved?.Invoke(piece);

            return true;
        }

        /// <summary>
        /// Moves the piece without checking its movement rules or capturing.
        /// </summary>
        public bool Relocate(Piece piece, Position anchor)
        {
            anchor = anchor.ChangeBoard(type);

            if (!Contains(piece))         return false;
            if (!CanPlace(piece, anchor)) return false;

            Position from = piece.GetPosition();

            Fill(piece, null);
            piece.SetPosition(anchor);
            Fill(piece, piece);

            PieceMoved?.Invoke(piece, from);

            return true;
        }

        /// <summary>
        /// Whether <paramref name="replacement"/> can take the place of
        /// <paramref name="piece"/> while still covering its bottom left square.
        /// </summary>
        public bool CanReplace(Piece piece, Piece replacement) => Contains(piece) 
                                                               && TryGetReplacementAnchor(piece, replacement, out _);

        /// <summary>
        /// The replacement covers the original's bottom left square. Fails if it
        /// doesn't fit.
        /// </summary>
        public bool ReplacePiece(Piece piece, Piece replacement)
        {
            if (!Contains(piece)) return false;
            if (!TryGetReplacementAnchor(piece, replacement, out Position anchor)) return false;

            RemovePiece(piece);

            return AddPiece(replacement, anchor);
        }

        private bool TryGetReplacementAnchor(Piece piece, Piece replacement, out Position anchor)
        {
            for (int dy = 0; dy < replacement.GetHeight(); dy++)
            for (int dx = 0; dx < replacement.GetWidth();  dx++)
            {
                anchor = piece.GetPosition().MoveBy(-dx, -dy);

                if (!IsInBounds(replacement, anchor)) continue;

                List<Piece> under = GetPiecesUnder(replacement, anchor);

                if (under.Count == 0 || (under.Count == 1 && under[0] == piece)) return true;
            }

            anchor = default;

            return false;
        }

        public bool IsValidMove(Piece piece, Position to)
        {
            to = to.ChangeBoard(type);

            if (!Contains(piece))          return false;
            if (to == piece.GetPosition()) return false;
            if (!IsInBounds(piece, to))    return false;

            foreach (Piece other in GetPiecesUnder(piece, to).Where(other => !piece.IsEnemyOf(other))) return false;

            return piece.IsValidMove(this, to);
        }

        /// <summary>
        /// Returns bottom left positions.
        /// </summary>
        public List<Position> GetValidMoves(Piece piece)
        {
            List<Position> moves = new();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Position to = new(x, y, type);

                    if (IsValidMove(piece, to)) moves.Add(to);
                }
            }

            return moves;
        }

        public MoveResult MovePiece(Piece piece, Position to)
        {
            to = to.ChangeBoard(type);

            if (!IsValidMove(piece, to)) return MoveResult.Invalid;

            List<Piece> captured = new();
            bool        blocked  = false;

            foreach (Piece target in GetPiecesUnder(piece, to))
            {
                if (!Contains(piece)) break;

                if (Capture(target, piece)) captured.Add(target);
                else                        blocked = true;
            }

            if (blocked || !Contains(piece) || GetPiecesUnder(piece, to).Count > 0) return MoveResult.Blocked;

            Position from = piece.GetPosition();

            Fill(piece, null);
            piece.SetPosition(to);
            Fill(piece, piece);

            PieceMoved?.Invoke(piece, from);

            piece.OnMoved(this, from);

            if (Contains(piece))
            {
                foreach (Piece target in captured) piece.OnCapture(this, target, from);
            }

            return MoveResult.Moved;
        }

        /// <summary>
        /// Hurts the piece, removing it if it dies. Pieces whose death is already
        /// being handled are ignored.
        /// </summary>
        /// <returns>If the piece died and its squares are free for the killer</returns>
        public bool Capture(Piece piece, Piece? killer = null)
        {
            if (!Contains (piece)) return false;
            if (!dying.Add(piece)) return false;

            bool died = piece.Kill(this, killer);

            dying.Remove(piece);

            if (!died)
            {
                PieceHurt?.Invoke(piece);

                return false;
            }

            Position at = piece.GetPosition();

            RemovePiece(piece);

            PieceKilled?.Invoke(piece, killer);

            game.OnPieceKilled(this, piece, killer);

            if (!game.ConsumeRule(Rule.NextPieceExplodes)) return true;

            Explode(at, piece);

            return false;
        }

        /// <summary>
        /// Captures every piece in the 3x3 area around <paramref name="center"/>,
        /// other than <paramref name="source"/>. Does nothing if called during another
        /// explosion.
        /// </summary>
        public void Explode(Position center, Piece? source)
        {
            if (exploding) return;

            exploding = true;

            Exploded?.Invoke(center.ChangeBoard(type));

            List<Piece> targets = new();

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    Piece? target = GetPieceAt(center.MoveBy(dx, dy));

                    if (target != null && target != source && !targets.Contains(target))
                        targets.Add(target);
                }
            }

            foreach (Piece target in targets)
                Capture(target, source);

            exploding = false;
        }

        /// <summary>
        /// A deep copy belonging to <paramref name="owner"/>, without event listeners.
        /// </summary>
        public Board Clone(GameState owner)
        {
            Board copy = new(owner, type, width, height);

            foreach (Piece piece in pieces)
            {
                Piece clone = piece.Clone();

                copy.pieces.Add(clone);
                copy.Fill(clone, clone);
            }

            return copy;
        }

        private void Fill(Piece piece, Piece? with)
        {
            foreach (Position square in piece.GetFootprint(piece.GetPosition())) squares[square.GetX(), square.GetY()] = with;
        }

        public JToken Serialize()
        {
            JArray pieceArray = new();

            foreach (Piece piece in pieces) pieceArray.Add(piece.Serialize());

            return new JObject
            {
                ["type"]   = type.ToString(),
                ["width"]  = width,
                ["height"] = height,
                ["pieces"] = pieceArray
            };
        }

        public static Board Deserialize(GameState game, JToken token)
        {
            Board board = new(
                game,
                Enum.Parse<BoardType>(token.Value<string>("type")!),
                token.Value<int>("width"),
                token.Value<int>("height")
            );

            foreach (JToken pieceToken in token["pieces"]!)
            {
                Piece piece = Piece.Deserialize(pieceToken);

                board.AddPiece(piece, piece.GetPosition());
            }

            return board;
        }
    }
}
