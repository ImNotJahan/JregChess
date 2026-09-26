using UnityEngine;

namespace Views
{
    #nullable enable

    /// <summary>
    /// A white 1x1 unit sprite, for tinting.
    /// </summary>
    public static class SolidSprite
    {
        private static Sprite? sprite;

        public static Sprite Get()
        {
            if (sprite != null) return sprite;

            Texture2D texture = new(1, 1) { filterMode = FilterMode.Point };
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);

            return sprite;
        }
    }
}
