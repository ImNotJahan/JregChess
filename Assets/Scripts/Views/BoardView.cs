using System;
using System.Collections.Generic;
using Boards;
using Pieces;
using UnityEngine;
using Util;

namespace Views
{
    #nullable enable

    /// <summary>
    /// Centered on this transform.
    /// </summary>
    public class BoardView : MonoBehaviour
    {
        public enum Highlight { None, Selected, Move, Capture }

        [Serializable]
        private struct Theme
        {
            public BoardType board;
            public Color     light;
            public Color     dark;
        }

        [SerializeField] private PieceSpriteLibrary sprites  = null!;
        [SerializeField] private IconLibrary        icons    = null!;
        [SerializeField] private float              tileSize = 1;

        [SerializeField] private Theme[] themes =
        {
            new() { board = BoardType.Normal, light = new Color32(255, 207, 159, 255), dark = new Color32(210, 140,  69, 255) },
            new() { board = BoardType.Heaven, light = new Color32(196, 237, 242, 255), dark = new Color32(104, 210, 222, 255) },
            new() { board = BoardType.Hell,   light = new Color32(255,   0,   0, 255), dark = new Color32(153,   0,   0, 255) },
        };

        [Header("Highlights")]
        [SerializeField] private Color selectedColor = new(0.3f, 0.85f, 1f);
        [SerializeField] private Color moveColor     = new(0.3f, 0.9f, 0.3f);
        [SerializeField] private Color captureColor  = new(0.95f, 0.2f, 0.2f);
        [SerializeField, Range(0, 1)] private float highlightStrength = 0.6f;

        [Header("Effects")]
        [SerializeField] private float explosionDuration = 0.6f;

        private readonly Dictionary<Piece, PieceView> pieceViews = new();

        private Board?            board;
        private SpriteRenderer[,] tiles = new SpriteRenderer[0, 0];
        private Highlight[,]      highlights = new Highlight[0, 0];
        private Transform?        tileRoot;
        private Transform?        pieceRoot;

        private Transform TileRoot  => tileRoot  ??= CreateChild("Tiles");
        private Transform PieceRoot => pieceRoot ??= CreateChild("Pieces");

        private void OnDestroy() => Unbind();

        public Board? GetBoard() => board;

        public void Bind(Board board)
        {
            Unbind();

            this.board = board;

            board.PieceAdded   += OnPieceAdded;
            board.PieceRemoved += OnPieceRemoved;
            board.PieceMoved   += OnPieceMoved;
            board.Exploded     += OnExploded;

            BuildTiles();

            foreach (Piece piece in board.GetPieces())
                OnPieceAdded(piece);
        }

        public void Unbind()
        {
            if (board == null) return;

            board.PieceAdded   -= OnPieceAdded;
            board.PieceRemoved -= OnPieceRemoved;
            board.PieceMoved   -= OnPieceMoved;
            board.Exploded     -= OnExploded;

            foreach (PieceView view in pieceViews.Values)
                Destroy(view.gameObject);

            pieceViews.Clear();

            board = null;
        }

        /// <summary>
        /// In world space.
        /// </summary>
        public Vector3 GetSquareCenter(Position square) =>
            transform.TransformPoint(GetLocalCenter(square, 1, 1));

        public bool TryGetSquare(Vector3 worldPoint, out Position square)
        {
            square = default;

            if (board == null) return false;

            Vector3 local = transform.InverseTransformPoint(worldPoint);

            square = new Position(
                Mathf.FloorToInt(local.x / tileSize + board.GetWidth () / 2f),
                Mathf.FloorToInt(local.y / tileSize + board.GetHeight() / 2f),
                board.GetBoardType()
            );

            return board.IsInBounds(square);
        }

        public Bounds GetWorldBounds()
        {
            if (board == null) return new Bounds(transform.position, Vector3.zero);

            Vector3 size = new(board.GetWidth() * tileSize, board.GetHeight() * tileSize, 0);

            return new Bounds(transform.position, Vector3.Scale(size, transform.lossyScale));
        }

        public void SetHighlight(Position square, Highlight highlight)
        {
            if (board == null || !board.IsInBounds(square)) return;

            highlights[square.GetX(), square.GetY()] = highlight;

            ColorTile(square.GetX(), square.GetY());
        }

        public void ClearHighlights()
        {
            if (board == null) return;

            for (int y = 0; y < highlights.GetLength(1); y++)
            {
                for (int x = 0; x < highlights.GetLength(0); x++)
                {
                    highlights[x, y] = Highlight.None;

                    ColorTile(x, y);
                }
            }
        }

        private void BuildTiles()
        {
            int width  = board!.GetWidth();
            int height = board.GetHeight();

            if (tiles.GetLength(0) != width || tiles.GetLength(1) != height)
            {
                foreach (SpriteRenderer tile in tiles)
                    Destroy(tile.gameObject);

                tiles = new SpriteRenderer[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        GameObject tile = new($"Tile {x}, {y}");

                        tile.transform.SetParent(TileRoot, false);
                        tile.transform.localPosition = GetLocalCenter(new Position(x, y), 1, 1);
                        tile.transform.localScale    = new Vector3(tileSize, tileSize, 1);

                        tiles[x, y] = tile.AddComponent<SpriteRenderer>();
                        tiles[x, y].sprite = SolidSprite.Get();
                    }
                }
            }

            highlights = new Highlight[width, height];

            ClearHighlights();
        }

        private void ColorTile(int x, int y)
        {
            Theme theme = GetTheme(board!.GetBoardType());
            // (0, 0) is dark, like a1
            Color color = (x + y) % 2 == 0 ? theme.dark : theme.light;

            color = highlights[x, y] switch
            {
                Highlight.Selected => Color.Lerp(color, selectedColor, highlightStrength),
                Highlight.Move     => Color.Lerp(color, moveColor,     highlightStrength),
                Highlight.Capture  => Color.Lerp(color, captureColor,  highlightStrength),
                _                  => color
            };

            tiles[x, y].color = color;
        }

        private Theme GetTheme(BoardType type)
        {
            foreach (Theme theme in themes)
                if (theme.board == type) return theme;

            return new Theme { board = type, light = Color.white, dark = Color.gray };
        }

        private void OnPieceAdded(Piece piece)
        {
            PieceView view = PieceView.Create(piece, sprites, PieceRoot);

            pieceViews[piece] = view;

            PlacePiece(view);
        }

        private void OnPieceRemoved(Piece piece)
        {
            if (!pieceViews.Remove(piece, out PieceView? view)) return;

            Destroy(view.gameObject);
        }

        private void OnPieceMoved(Piece piece, Position from)
        {
            if (pieceViews.TryGetValue(piece, out PieceView? view)) PlacePiece(view, true);
        }

        private void OnExploded(Position center)
        {
            Sprite? explosion = icons.GetIcon("explosion");

            if (explosion == null) return;

            FadeEffect.Create(explosion, PieceRoot, GetLocalCenter(center, 1, 1), 3 * tileSize, explosionDuration, 50);
        }

        private void PlacePiece(PieceView view, bool animate = false)
        {
            Piece piece = view.GetPiece();

            view.Place(
                GetLocalCenter(piece.GetPosition(), piece.GetWidth(), piece.GetHeight()),
                tileSize,
                animate
            );
        }

        /// <summary>
        /// Center of the area with its bottom left at <paramref name="anchor"/>, in
        /// local space.
        /// </summary>
        private Vector3 GetLocalCenter(Position anchor, int width, int height)
        {
            int boardWidth  = board?.GetWidth () ?? Board.DefaultSize;
            int boardHeight = board?.GetHeight() ?? Board.DefaultSize;

            return new Vector3(
                (anchor.GetX() + width  / 2f - boardWidth  / 2f) * tileSize,
                (anchor.GetY() + height / 2f - boardHeight / 2f) * tileSize,
                0
            );
        }

        private Transform CreateChild(string childName)
        {
            Transform child = new GameObject(childName).transform;

            child.SetParent(transform, false);

            return child;
        }
    }
}
