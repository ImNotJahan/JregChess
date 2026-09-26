using System.Collections.Generic;
using Gameplay;
using Pieces;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    public class ShopWindow : Window
    {
        private readonly GameController controller;
        private readonly UISprites      sprites;

        private readonly List<(ShopItem item, Button button, Image image)> tiles = new();

        public ShopWindow(GameController controller, UISprites sprites, Tooltip tooltip) : base("Shop")
        {
            this.controller = controller;
            this.sprites    = sprites;

            AddToClassList("shop");

            VisualElement grid = new();
            grid.AddToClassList("item-grid");
            GetContent().Add(grid);

            foreach (ShopItem item in ShopCatalog.Items)
            {
                Button button = new(() => controller.BeginBuy(item.Id));
                button.AddToClassList("item");
                button.AddManipulator(new TooltipTrigger(tooltip, UISprites.FormatName(item.Id)));

                Image image = UISprites.CreateImage(null, "item__image");
                button.Add(image);

                Label name = new(UISprites.FormatName(item.Id));
                name.AddToClassList("item__name");
                button.Add(name);

                Label cost = new($"{item.Cost} gold");
                cost.AddToClassList("item__cost");
                button.Add(cost);

                grid.Add(button);
                tiles.Add((item, button, image));
            }
        }

        public void Refresh()
        {
            GameState   state = controller.GetState();
            Piece.Color turn  = state.GetTurn();
            bool        open  = state.GetDecision() == null && !state.IsOver();

            foreach ((ShopItem item, Button button, Image image) in tiles)
            {
                image.sprite = sprites.GetPiece(item.Id, item.Neutral ? Piece.Color.NPC : turn);

                button.SetEnabled(open && state.GetGold(turn) >= item.Cost);
                button.EnableInClassList("item--active",
                    controller.GetMode() == GameController.Mode.Buy && controller.GetModeItem() == item.Id);
            }
        }
    }
}
