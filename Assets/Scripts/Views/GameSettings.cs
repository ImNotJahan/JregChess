using UnityEngine;

namespace Views
{
    #nullable enable

    /// <summary>
    /// Player preferences, kept between sessions.
    /// </summary>
    public static class GameSettings
    {
        private const string SfxVolumeKey = "SfxVolume";

        /// <summary>
        /// From 0 to 1.
        /// </summary>
        public static float GetSfxVolume() => PlayerPrefs.GetFloat(SfxVolumeKey, 1);

        public static void SetSfxVolume(float volume)
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(volume));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Unity remembers this between sessions itself.
        /// </summary>
        public static bool IsFullscreen() => Screen.fullScreen;

        public static void SetFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                Resolution display = Screen.currentResolution;

                Screen.SetResolution(display.width, display.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
    }
}
