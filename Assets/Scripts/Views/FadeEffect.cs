using UnityEngine;

namespace Views
{
    #nullable enable

    /// <summary>
    /// A sprite which fades out and destroys itself.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class FadeEffect : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer = null!;
        private float          duration;
        private float          elapsed;

        public static FadeEffect Create(Sprite sprite, Transform parent, Vector3 localPosition, float size, float duration, int sortingOrder)
        {
            GameObject obj = new("Effect");
            obj.transform.SetParent(parent, false);

            FadeEffect effect = obj.AddComponent<FadeEffect>();

            effect.duration       = duration;
            effect.spriteRenderer = obj.GetComponent<SpriteRenderer>();

            effect.spriteRenderer.sprite       = sprite;
            effect.spriteRenderer.sortingOrder = sortingOrder;

            Bounds bounds = sprite.bounds;
            float  scale  = size / Mathf.Max(bounds.size.x, bounds.size.y);

            obj.transform.localScale    = new Vector3(scale, scale, 1);
            obj.transform.localPosition = localPosition - bounds.center * scale;

            return effect;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            Color color = spriteRenderer.color;
            color.a = 1 - elapsed / duration;
            spriteRenderer.color = color;

            if (elapsed >= duration) Destroy(gameObject);
        }
    }
}
