using System;
using UnityEngine;

namespace Views
{
    #nullable enable

    /// <summary>
    /// Sprites that aren't pieces, such as effects.
    /// </summary>
    [CreateAssetMenu(fileName = "Icons", menuName = "JregChess/Icon Library")]
    public class IconLibrary : ScriptableObject
    {
        [Serializable]
        private struct Entry
        {
            public string id;
            public Sprite sprite;
        }

        [SerializeField] private Entry[] entries = Array.Empty<Entry>();

        public Sprite? GetIcon(string id)
        {
            foreach (Entry entry in entries)
                if (entry.id == id) return entry.sprite;

            return null;
        }
    }
}
