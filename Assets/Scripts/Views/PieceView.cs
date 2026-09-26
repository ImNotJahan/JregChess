using System.Collections;
using Pieces;
using UnityEngine;

namespace Views
{
    #nullable enable

    public class PieceView : MonoBehaviour
    {
        /// <summary>
        /// Fraction of its squares the sprite fills
        /// </summary>
        private const float Fill = 0.85f;

        private const float MoveDuration    = 0.5f;
        private const int   SortingOrder    = 10;
        private const float HealthBarHeight = 0.08f;

        private static readonly Color HealthBackColor = new(0.1f, 0.1f, 0.1f, 0.85f);
        private static readonly Color HealthColor     = new(0.35f, 0.9f, 0.35f);

        private SpriteRenderer     spriteRenderer = null!;
        private SpriteRenderer?    healthBack;
        private SpriteRenderer?    healthFill;
        private Piece              piece          = null!;
        private PieceSpriteLibrary sprites        = null!;
        private float              tileSize       = 1;
        private Coroutine?         sliding;

        public static PieceView Create(Piece piece, PieceSpriteLibrary sprites, Transform parent)
        {
            GameObject obj = new($"{piece.GetColor()} {piece.GetId()}");
            obj.transform.SetParent(parent, false);

            PieceView view = obj.AddComponent<PieceView>();

            view.piece   = piece;
            view.sprites = sprites;

            GameObject spriteObj = new("Sprite");
            spriteObj.transform.SetParent(obj.transform, false);

            view.spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
            view.spriteRenderer.sortingOrder = SortingOrder;

            if (piece.GetMaxHealth() > 1)
            {
                view.healthBack = CreateBar(obj.transform, "Health", HealthBackColor, SortingOrder + 1);
                view.healthFill = CreateBar(view.healthBack.transform, "Fill", HealthColor, SortingOrder + 2);
            }

            piece.Changed += view.Refresh;

            view.Refresh();

            return view;
        }

        private void OnDestroy()
        {
            if (piece != null) piece.Changed -= Refresh;
        }

        public Piece GetPiece() => piece;

        /// <summary>
        /// Scales the sprite to fit the piece's squares, centered on
        /// <paramref name="center"/> in the parent's space.
        /// </summary>
        public void Place(Vector3 center, float tileSize, bool animate = false)
        {
            this.tileSize = tileSize;

            if (sliding != null) StopCoroutine(sliding);

            sliding = null;

            if (animate && isActiveAndEnabled) sliding = StartCoroutine(Slide(center));
            else                               transform.localPosition = center;

            FitSprite();
            PlaceHealthBar();
        }

        private IEnumerator Slide(Vector3 to)
        {
            Vector3 from = transform.localPosition;

            for (float elapsed = 0; elapsed < MoveDuration; elapsed += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0, 1, elapsed / MoveDuration));

                yield return null;
            }

            transform.localPosition = to;
            sliding = null;
        }

        private void Refresh()
        {
            spriteRenderer.sprite = sprites.GetSprite(piece);

            FitSprite();

            if (healthFill != null)
            {
                float fraction = (float)piece.GetHealth() / piece.GetMaxHealth();

                healthFill.transform.localScale    = new Vector3(fraction, 1, 1);
                healthFill.transform.localPosition = new Vector3((fraction - 1) / 2, 0, 0);
            }
        }

        private void FitSprite()
        {
            Bounds bounds = spriteRenderer.sprite.bounds;

            float scale = Fill * Mathf.Min(
                piece.GetWidth () * tileSize / bounds.size.x,
                piece.GetHeight() * tileSize / bounds.size.y
            );

            spriteRenderer.transform.localScale    = new Vector3(scale, scale, 1);
            spriteRenderer.transform.localPosition = -bounds.center * scale;
        }

        private void PlaceHealthBar()
        {
            if (healthBack == null) return;

            float width = piece.GetWidth() * tileSize * Fill;

            healthBack.transform.localScale    = new Vector3(width, HealthBarHeight * tileSize, 1);
            healthBack.transform.localPosition = new Vector3(0, piece.GetHeight() * tileSize / 2 - HealthBarHeight * tileSize, 0);
        }

        private static SpriteRenderer CreateBar(Transform parent, string name, Color color, int order)
        {
            GameObject obj = new(name);
            obj.transform.SetParent(parent, false);

            SpriteRenderer bar = obj.AddComponent<SpriteRenderer>();

            bar.sprite       = SolidSprite.Get();
            bar.color        = color;
            bar.sortingOrder = order;

            return bar;
        }
    }
}
