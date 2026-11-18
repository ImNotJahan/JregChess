using System;
using Boards;
using UnityEngine;

namespace Views
{
    #nullable enable

    /// <summary>
    /// Tiles a sprite across the camera's whole view, behind the board.
    /// Runs after <see cref="BoardCameraFitter"/> so it matches this frame's zoom.
    /// </summary>
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BoardBackdrop : MonoBehaviour
    {
        [Serializable]
        private struct Theme
        {
            public BoardType board;
            public Sprite    sprite;
        }

        [SerializeField] private Camera?    cam;
        [Tooltip("Picks the sprite from the bound board's type. Leave empty to always use the SpriteRenderer's sprite, as in the menu.")]
        [SerializeField] private BoardView? boardView;
        [SerializeField] private Theme[]    themes = Array.Empty<Theme>();
        [Tooltip("World units covered by one repeat of the sprite. A board tile is 1 unit.")]
        [SerializeField] private float      tileSize     = 2;
        [SerializeField] private int        sortingOrder = -100;

        private SpriteRenderer spriteRenderer = null!;
        private Sprite?        defaultSprite;

        private void Awake()
        {
            if (cam == null) cam = Camera.main!;

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.drawMode     = SpriteDrawMode.Tiled;
            spriteRenderer.sortingOrder = sortingOrder;

            defaultSprite = spriteRenderer.sprite;
        }

        private void LateUpdate()
        {
            spriteRenderer.sprite = GetSprite();

            if (spriteRenderer.sprite == null) return;

            float height = cam!.orthographicSize * 2;
            float width  = height * cam.aspect;

            // Tiled size is in the sprite's units, so scale the transform to set the repeat size
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            float   scale      = tileSize / Mathf.Max(spriteSize.x, spriteSize.y);

            transform.localScale  = new Vector3(scale, scale, 1);
            spriteRenderer.size   = new Vector2(width, height) / scale;

            Vector3 camPos = cam.transform.position;
            transform.position = new Vector3(camPos.x, camPos.y, transform.position.z);
        }

        private Sprite? GetSprite()
        {
            if (boardView == null || boardView.GetBoard() is not Board board) return defaultSprite;

            BoardType type = board.GetBoardType();

            foreach (Theme theme in themes)
                if (theme.board == type && theme.sprite != null)
                    return theme.sprite;

            return defaultSprite;
        }
    }
}
