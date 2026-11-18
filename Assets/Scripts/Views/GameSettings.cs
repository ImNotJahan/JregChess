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
#if UNITY_WEBGL && !UNITY_EDITOR
            // The browser picks the size. It only allows going fullscreen during a click or
            // key press, so Unity holds the request until the next one: call this on the
            // press so it happens on the release.
            Screen.fullScreen = fullscreen;
#else
            if (fullscreen)
            {
                Resolution display = Screen.currentResolution;

                Screen.SetResolution(display.width, display.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
#endif
        }
    }
}
