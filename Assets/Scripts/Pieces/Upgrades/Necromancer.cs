using System;
using Boards;
using Util;

namespace Pieces.Upgrades
{
    #nullable enable

    public class Necromancer : Bishop
    {
        public new const string ID = "necromancer";

        public Necromancer(Color color = default) : base(color) {}

        public override string GetId() => ID;

        /// <summary>
        /// Brings the captured piece back on its side, one square back along the
        /// path it moved.
        /// </summary>
        public override void OnCapture(Board board, Piece captured, Position from)
        {
            if (!captured.IsPlayerPiece()) return;

            Position respawnAt = position.MoveBy(
                Math.Sign(from.GetX() - position.GetX()),
                Math.Sign(from.GetY() - position.GetY())
            );

            if (!board.CanPlace(captured, respawnAt)) return;

            board.GetGame().FindBoard(captured)?.RemovePiece(captured);

            captured.SetColor(color);
            captured.Heal();

            board.AddPiece(captured, respawnAt);
        }
    }
}
