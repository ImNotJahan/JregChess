using System;
using Pieces;
using UnityEngine;

namespace Views
{
    #nullable enable

    [CreateAssetMenu(fileName = "PieceSprites", menuName = "JregChess/Piece Sprite Library")]
    public class PieceSpriteLibrary : ScriptableObject
    {
        [Serializable]
        private struct Entry
        {
            public string id;
            public Sprite white;
            public Sprite black;
            [Tooltip("Used for NPC pieces, and for any color that has no sprite set")]
            public Sprite neutral;
        }

        [SerializeField] private Entry[] entries  = Array.Empty<Entry>();
        [SerializeField] private Sprite  fallback = null!;

        public Sprite GetSprite(Piece piece) => GetSprite(piece.GetId(), piece.GetColor());

        public Sprite GetSprite(string id, Piece.Color color)
        {
            foreach (Entry entry in entries)
            {
                if (entry.id != id) continue;

                Sprite? sprite = color switch
                {
                    Piece.Color.White => entry.white,
                    Piece.Color.Black => entry.black,
                    _                 => null
                };

                if (sprite != null)        return sprite;
                if (entry.neutral != null) return entry.neutral;
                if (entry.white != null)   return entry.white;

                break;
            }

            Debug.LogWarning($"No sprite for {color} {id}");

            return fallback;
        }
    }
}
