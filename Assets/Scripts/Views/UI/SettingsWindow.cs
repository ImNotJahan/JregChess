using UnityEngine;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    public class SettingsWindow : Window
    {
        private readonly Slider volumeSlider;
        private readonly Label  volumeLabel;
        private readonly Toggle fullscreenToggle;

        public SettingsWindow() : base("Settings")
        {
            AddToClassList("settings");

            VisualElement content = GetContent();

            Label volumeHint = new("Sound effects");
            volumeHint.AddToClassList("window__hint");
            content.Add(volumeHint);

            VisualElement volumeRow = new();
            volumeRow.AddToClassList("settings__row");
            content.Add(volumeRow);

            volumeSlider = new Slider(0, 1);
            volumeSlider.AddToClassList("settings__slider");
            volumeRow.Add(volumeSlider);

            volumeLabel = new Label();
            volumeLabel.AddToClassList("settings__value");
            volumeRow.Add(volumeLabel);

            volumeSlider.RegisterValueChangedCallback(change =>
            {
                GameSettings.SetSfxVolume(change.newValue);
                ShowVolume(change.newValue);
            });

            fullscreenToggle = new Toggle("Fullscreen");
            fullscreenToggle.AddToClassList("settings__toggle");
            fullscreenToggle.RegisterValueChangedCallback(change => GameSettings.SetFullscreen(change.newValue));
            content.Add(fullscreenToggle);

            Refresh();
        }

        /// <summary>
        /// Shows the current settings, which may have changed elsewhere, such as
        /// fullscreen through the OS.
        /// </summary>
        public void Refresh()
        {
            float volume = GameSettings.GetSfxVolume();

            volumeSlider    .SetValueWithoutNotify(volume);
            fullscreenToggle.SetValueWithoutNotify(GameSettings.IsFullscreen());

            ShowVolume(volume);
        }

        private void ShowVolume(float volume) => volumeLabel.text = $"{Mathf.RoundToInt(volume * 100)}%";
    }
}
