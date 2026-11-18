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
            [Tooltip("Maps the tile texture's darkest pixel to dark and its lightest to light. Off uses the texture as is.")]
            public bool      recolor;
            public Color     light;
            public Color     dark;
        }

        private const int HighlightSortingOrder = 1;

        [SerializeField] private PieceSpriteLibrary sprites  = null!;
        [SerializeField] private IconLibrary        icons    = null!;
        [SerializeField] private float              tileSize = 1;

        [Header("Tiles")]
        [Tooltip("A checkerboard with a dark bottom left square, cut into one sprite per square. Must be readable, to be recolored.")]
        [SerializeField] private Texture2D tileTexture = null!;
        [Tooltip("Squares along each side of the tile texture")]
        [SerializeField] private int       tileTextureSquares = 4;

        [SerializeField] private Theme[] themes =
        {
            new() { board = BoardType.Normal },
            new() { board = BoardType.Heaven, recolor = true, light = new Color32(196, 237, 242, 255), dark = new Color32(104, 210, 222, 255) },
            new() { board = BoardType.Hell,   recolor = true, light = new Color32(255,   0,   0, 255), dark = new Color32(153,   0,   0, 255) },
        };

        [Header("Highlights")]
        [SerializeField] private Color selectedColor = new(0.3f, 0.85f, 1f);
        [SerializeField] private Color moveColor     = new(0.3f, 0.9f, 0.3f);
        [SerializeField] private Color captureColor  = new(0.95f, 0.2f, 0.2f);
        [SerializeField, Range(0, 1)] private float highlightStrength = 0.6f;

        [Header("Effects")]
        [SerializeField] private float explosionDuration = 0.6f;

        [Header("Sounds")]
        [SerializeField] private AudioClip? moveSound;
        [SerializeField] private AudioClip? explosionSound;

        private readonly Dictionary<Piece, PieceView> pieceViews = new();

        private Board?            board;
        private readonly Dictionary<BoardType, Sprite[,]> tileSprites = new();

        private SpriteRenderer[,] tiles = new SpriteRenderer[0, 0];
        /// <summary>
        /// Drawn over the tiles, as tinting would barely show on dark ones.
        /// </summary>
        private SpriteRenderer[,] highlightOverlays = new SpriteRenderer[0, 0];
        private Highlight[,]      highlights = new Highlight[0, 0];
        private Transform?        tileRoot;
        private Transform?        pieceRoot;
        private AudioSource?      audioSource;
        /// <summary>
        /// So pieces moving together, such as NPCs, make one sound.
        /// </summary>
        private int               lastMoveSoundFrame = -1;

        private Transform TileRoot  => tileRoot  ??= CreateChild("Tiles");
        private Transform PieceRoot => pieceRoot ??= CreateChild("Pieces");

        private void OnDestroy()
        {
            Unbind();

            foreach (Sprite[,] squareSprites in tileSprites.Values)
            {
                // Recolored textures belong to this view; the original is an asset
                Texture texture = squareSprites[0, 0].texture;

                foreach (Sprite sprite in squareSprites)
                    Destroy(sprite);

                if (texture != tileTexture) Destroy(texture);
            }

            tileSprites.Clear();
        }

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

            UpdateTile(square.GetX(), square.GetY());
        }

        public void ClearHighlights()
        {
            if (board == null) return;

            for (int y = 0; y < highlights.GetLength(1); y++)
            {
                for (int x = 0; x < highlights.GetLength(0); x++)
                {
                    highlights[x, y] = Highlight.None;

                    UpdateTile(x, y);
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

                tiles             = new SpriteRenderer[width, height];
                highlightOverlays = new SpriteRenderer[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        GameObject tile = new($"Tile {x}, {y}");

                        tile.transform.SetParent(TileRoot, false);
                        tile.transform.localPosition = GetLocalCenter(new Position(x, y), 1, 1);
                        tile.transform.localScale    = new Vector3(tileSize, tileSize, 1);

                        tiles[x, y] = tile.AddComponent<SpriteRenderer>();

                        GameObject overlay = new("Highlight");
                        overlay.transform.SetParent(tile.transform, false);

                        highlightOverlays[x, y] = overlay.AddComponent<SpriteRenderer>();
                        highlightOverlays[x, y].sprite       = SolidSprite.Get();
                        highlightOverlays[x, y].sortingOrder = HighlightSortingOrder;
                    }
                }
            }

            highlights = new Highlight[width, height];

            ClearHighlights();
        }

        private void UpdateTile(int x, int y)
        {
            Sprite[,] squareSprites = GetTileSprites(GetTheme(board!.GetBoardType()));

            // The texture's bottom left square is dark, like a1, so its squares line up with the board's
            tiles[x, y].sprite = squareSprites[x % squareSprites.GetLength(0), y % squareSprites.GetLength(1)];

            Color? highlightColor = highlights[x, y] switch
            {
                Highlight.Selected => selectedColor,
                Highlight.Move     => moveColor,
                Highlight.Capture  => captureColor,
                _                  => null
            };

            SpriteRenderer overlay = highlightOverlays[x, y];
            overlay.enabled = highlightColor.HasValue;

            if (highlightColor is Color color)
                overlay.color = new Color(color.r, color.g, color.b, highlightStrength);
        }

        private Theme GetTheme(BoardType type)
        {
            foreach (Theme theme in themes)
                if (theme.board == type) return theme;

            return new Theme { board = type };
        }

        /// <summary>
        /// Indexed by square in the tile texture, from the bottom left.
        /// </summary>
        private Sprite[,] GetTileSprites(Theme theme)
        {
            if (tileSprites.TryGetValue(theme.board, out Sprite[,]? cached)) return cached;

            Texture2D texture = theme.recolor ? Recolor(tileTexture, theme.dark, theme.light) : tileTexture;

            int   squares    = tileTextureSquares;
            float squareSize = (float)texture.width / squares;

            Sprite[,] squareSprites = new Sprite[squares, squares];

            for (int y = 0; y < squares; y++)
            {
                for (int x = 0; x < squares; x++)
                {
                    Rect rect = new(x * squareSize, y * squareSize, squareSize, squareSize);

                    // One square per world unit, like the tile transforms expect
                    squareSprites[x, y] = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), squareSize, 0, SpriteMeshType.FullRect);
                }
            }

            tileSprites[theme.board] = squareSprites;

            return squareSprites;
        }

        /// <summary>
        /// Maps brightness onto a gradient from <paramref name="dark"/> to <paramref name="light"/>.
        /// </summary>
        private static Texture2D Recolor(Texture2D source, Color dark, Color light)
        {
            Color32[] pixels = source.GetPixels32();
            float[]   values = new float[pixels.Length];

            float min = 1;
            float max = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                values[i] = ((Color)pixels[i]).grayscale;

                min = Mathf.Min(min, values[i]);
                max = Mathf.Max(max, values[i]);
            }

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.Lerp(dark, light, Mathf.InverseLerp(min, max, values[i]));

            Texture2D result = new(source.width, source.height, TextureFormat.RGBA32, false)
            {
                filterMode = source.filterMode,
                wrapMode   = TextureWrapMode.Clamp
            };

            result.SetPixels32(pixels);
            result.Apply(false, true);

            return result;
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
            if (!pieceViews.TryGetValue(piece, out PieceView? view)) return;

            PlacePiece(view, true);

            if (lastMoveSoundFrame == Time.frameCount) return;

            lastMoveSoundFrame = Time.frameCount;

            PlaySound(moveSound);
        }

        private void OnExploded(Position center)
        {
            PlaySound(explosionSound);

            Sprite? explosion = icons.GetIcon("explosion");

            if (explosion == null) return;

            FadeEffect.Create(explosion, PieceRoot, GetLocalCenter(center, 1, 1), 3 * tileSize, explosionDuration, 50);
        }

        private void PlaySound(AudioClip? clip)
        {
            if (clip == null) return;

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake  = false;
                audioSource.spatialBlend = 0;
            }

            audioSource.PlayOneShot(clip, GameSettings.GetSfxVolume());
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
