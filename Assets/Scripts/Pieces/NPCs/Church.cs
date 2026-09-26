using Boards;

namespace Pieces.NPCs
{
    #nullable enable

    public class Church : Npc
    {
        public const string ID = "church";

        public override string GetId() => ID;

        public override string GetDescription() => "";

        public override int GetWidth () => 2;
        public override int GetHeight() => 3;

        /// <summary>
        /// Sends the killer to its bottom left square on the Normal board.
        /// </summary>
        public override bool Kill(Board board, Piece? killer)
        {
            if (killer != null) board.GetGame().Transfer(killer, BoardType.Normal, position.ChangeBoard(BoardType.Normal));

            return false;
        }
    }
}
