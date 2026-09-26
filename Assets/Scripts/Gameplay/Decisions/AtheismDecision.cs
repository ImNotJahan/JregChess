using System.Collections.Generic;
using Boards;
using Pieces;
using Pieces.NPCs;
using Util;

namespace Gameplay.Decisions
{
    #nullable enable

    public class AtheismDecision : Decision
    {
        public const string ID = "atheism";

        private static readonly string[] Options =
        {
            "Destroy Heaven",
            "Destroy Hell",
            "Destroy all metaphysical pieces on the material plane"
        };

        public AtheismDecision(Piece.Color player) : base(player) {}

        public override string GetId() => ID;

        public override string GetTitle() => "The God of Atheism";

        public override string? GetIcon() => Atheism.ID;

        public override IReadOnlyList<string> GetOptions(GameState game) => Options;

        public override void Resolve(GameState game, int option, Position? target)
        {
            switch (option)
            {
                case 0:
                    game.DestroyBoard(BoardType.Heaven);
                    break;

                case 1:
                    game.DestroyBoard(BoardType.Hell);
                    break;

                case 2:
                    if (!game.TryGetBoard(BoardType.Normal, out Board? normal)) break;

                    foreach (Piece piece in new List<Piece>(normal.GetPieces()))
                    {
                        if (piece is Angel or AggroAngel or Devil or AggroDevil) normal.RemovePiece(piece);
                    }

                    break;
            }
        }
    }
}
