using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    /// <summary>
    /// A panel which can be dragged by its title bar within its parent.
    /// </summary>
    public class Window : VisualElement
    {
        public event Action? Closed;

        private readonly VisualElement titleBar;
        private readonly Label         titleLabel;
        private readonly VisualElement content;

        private Vector2 dragStart;
        private Vector2 positionAtDragStart;

        public Window(string title, bool closable = true)
        {
            AddToClassList("window");

            titleBar = new VisualElement();
            titleBar.AddToClassList("window__title-bar");
            Add(titleBar);

            titleLabel = new Label(title);
            titleLabel.AddToClassList("window__title");
            titleLabel.pickingMode = PickingMode.Ignore;
            titleBar.Add(titleLabel);

            if (closable)
            {
                Button close = new(Close) { text = "×" };
                close.AddToClassList("window__close");
                titleBar.Add(close);
            }

            content = new VisualElement();
            content.AddToClassList("window__content");
            Add(content);

            titleBar.RegisterCallback<PointerDownEvent>(OnPointerDown);
            titleBar.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            titleBar.RegisterCallback<PointerUpEvent>(OnPointerUp);

            RegisterCallback<PointerDownEvent>(_ => BringToFront(), TrickleDown.TrickleDown);
        }

        public VisualElement GetContent() => content;

        public void SetTitle(string title) => titleLabel.text = title;

        public bool IsOpen() => parent != null;

        /// <summary>
        /// Brings the window to the front if it's already open, without moving it.
        /// </summary>
        public void Open(VisualElement layer, Vector2 position)
        {
            if (!IsOpen())
            {
                layer.Add(this);

                style.left = position.x;
                style.top  = position.y;
            }

            BringToFront();
        }

        public void Toggle(VisualElement layer, Vector2 position)
        {
            if (IsOpen()) Close();
            else          Open(layer, position);
        }

        public void Close()
        {
            if (!IsOpen()) return;

            RemoveFromHierarchy();

            Closed?.Invoke();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0 || evt.target != titleBar) return;

            titleBar.CapturePointer(evt.pointerId);

            dragStart           = evt.position;
            positionAtDragStart = new Vector2(resolvedStyle.left, resolvedStyle.top);

            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!titleBar.HasPointerCapture(evt.pointerId)) return;

            Vector2 position = positionAtDragStart + ((Vector2)evt.position - dragStart);

            if (parent != null)
            {
                position.x = Mathf.Clamp(position.x, 0, Mathf.Max(0, parent.resolvedStyle.width  - resolvedStyle.width));
                position.y = Mathf.Clamp(position.y, 0, Mathf.Max(0, parent.resolvedStyle.height - titleBar.resolvedStyle.height));
            }

            style.left = position.x;
            style.top  = position.y;
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (titleBar.HasPointerCapture(evt.pointerId)) titleBar.ReleasePointer(evt.pointerId);
        }
    }
}
