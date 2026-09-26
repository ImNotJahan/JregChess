using UnityEngine;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    /// <summary>
    /// A single floating text box, shown by <see cref="TooltipTrigger"/>s. Lives on its own
    /// layer above everything else, and never takes the pointer.
    /// </summary>
    public class Tooltip : VisualElement
    {
        private readonly Label label;

        public Tooltip()
        {
            AddToClassList("tooltip");
            pickingMode = PickingMode.Ignore;

            label = new Label { pickingMode = PickingMode.Ignore };
            label.AddToClassList("tooltip__text");
            Add(label);

            style.display = DisplayStyle.None;

            // layout isn't known until after Show, so clamp once it resolves
            RegisterCallback<GeometryChangedEvent>(_ => ClampToScreen());
        }

        /// <param name="position">The tooltip's top left, in panel space.</param>
        public void Show(string text, Vector2 position)
        {
            label.text = text;

            style.left    = position.x;
            style.top     = position.y;
            style.display = DisplayStyle.Flex;

            BringToFront();
        }

        public void Refresh(string text) => label.text = text;

        public void Hide() => style.display = DisplayStyle.None;

        /*
        Make sure that the tooltip's position places the entirety of the
        tooltip within its layer's bounds. If not, changes the tooltip's
        position to be within, even if that means it overlaps whatever
        it was anchored to.
        */
        private void ClampToScreen()
        {
            if (parent == null || resolvedStyle.display == DisplayStyle.None) return;

            float left = Mathf.Clamp(resolvedStyle.left, 0, Mathf.Max(0, parent.resolvedStyle.width  - resolvedStyle.width));
            float top  = Mathf.Clamp(resolvedStyle.top,  0, Mathf.Max(0, parent.resolvedStyle.height - resolvedStyle.height));

            if (!Mathf.Approximately(left, resolvedStyle.left)) style.left = left;
            if (!Mathf.Approximately(top,  resolvedStyle.top))  style.top  = top;
        }
    }
}
