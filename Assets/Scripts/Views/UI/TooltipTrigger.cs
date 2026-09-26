using UnityEngine;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    /// <summary>
    /// Shows a <see cref="Tooltip"/> under its element's bottom right corner after the
    /// pointer has rested on it for a moment.
    /// </summary>
    public class TooltipTrigger : Manipulator
    {
        private const long ShowDelayMs = 300;

        private readonly Tooltip tooltip;

        private string? text;

        private IVisualElementScheduledItem? pendingShow;

        private bool isHovering = false;

        public TooltipTrigger(Tooltip tooltip, string? text = null)
        {
            this.tooltip = tooltip;
            this.text    = text;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.RegisterCallback<DetachFromPanelEvent>(OnDetach);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.UnregisterCallback<DetachFromPanelEvent>(OnDetach);

            EndHover();
        }

        public void SetText(string? text)
        {
            this.text = text;

            if (!isHovering) return;

            if (text is null) EndHover();
            else              tooltip.Refresh(text);
        }

        private void OnPointerEnter(PointerEnterEvent _)
        {
            if (text == null) return;

            pendingShow?.Pause();
            pendingShow = target.schedule.Execute(Show).StartingIn(ShowDelayMs);
        }

        private void OnPointerLeave(PointerLeaveEvent _) => EndHover();

        // an element removed while hovered never gets a pointer leave, so its tooltip would stay up
        private void OnDetach(DetachFromPanelEvent _) => EndHover();

        private void Show()
        {
            if (text == null) return;

            Rect bounds = target.worldBound;

            tooltip.Show(text, new Vector2(bounds.xMax, bounds.yMax));

            isHovering = true;
        }

        private void EndHover()
        {
            pendingShow?.Pause();
            pendingShow = null;

            if (isHovering) tooltip.Hide();

            isHovering = false;
        }
    }
}
