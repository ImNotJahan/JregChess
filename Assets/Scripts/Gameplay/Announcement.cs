namespace Gameplay
{
    #nullable enable

    /// <summary>
    /// A message for the players. Doesn't affect the game.
    /// </summary>
    public readonly struct Announcement
    {
        public readonly string  Text;
        /// <summary>
        /// Id of an icon or piece to show with the text.
        /// </summary>
        public readonly string? Icon;

        public Announcement(string text, string? icon = null)
        {
            Text = text;
            Icon = icon;
        }
    }
}
