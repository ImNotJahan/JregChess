using UnityEngine;

namespace Views
{
    #nullable enable

    [RequireComponent(typeof(Camera))]
    public class BoardCameraFitter : MonoBehaviour
    {
        [SerializeField] private BoardView boardView = null!;
        [SerializeField] private float     padding   = 0.5f;
        [Tooltip("Room kept free on each side of the board for the side panels, as a fraction of the board's width")]
        [SerializeField] private float     sideRoom  = 0.45f;

        private Camera cam = null!;

        private void Awake() => cam = GetComponent<Camera>();

        private void LateUpdate()
        {
            Bounds bounds = boardView.GetWorldBounds();

            if (bounds.size == Vector3.zero) return;

            float halfWidth = bounds.extents.x * (1 + 2 * sideRoom);

            cam.orthographicSize = Mathf.Max(bounds.extents.y, halfWidth / cam.aspect) + padding;

            transform.position = new Vector3(bounds.center.x, bounds.center.y, transform.position.z);
        }
    }
}
