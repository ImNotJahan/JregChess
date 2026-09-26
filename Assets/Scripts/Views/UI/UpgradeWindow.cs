using System.Collections.Generic;
using Gameplay;
using Pieces;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    public class UpgradeWindow : Window
    {
        private readonly GameController controller;
        private readonly UISprites      sprites;

        private readonly List<(Upgrade upgrade, Button button, Image image, List<Image> from)> rows = new();

        public UpgradeWindow(GameController controller, UISprites sprites, Tooltip tooltip) : base("Upgrades")
        {
            this.controller = controller;
            this.sprites    = sprites;

            AddToClassList("upgrades");

            Label hint = new($"Everything costs 5 (just five!) gold!");
            hint.AddToClassList("window__hint");
            GetContent().Add(hint);

            ScrollView list = new();
            list.AddToClassList("upgrade-list");
            GetContent().Add(list);

            foreach (Upgrade upgrade in UpgradeCatalog.Upgrades)
            {
                Button button = new(() => controller.BeginUpgrade(upgrade.Id));
                button.AddToClassList("upgrade");
                button.AddManipulator(new TooltipTrigger(tooltip, UISprites.FormatName(upgrade.Id)));

                Image image = UISprites.CreateImage(null, "upgrade__image");
                button.Add(image);

                Label name = new(UISprites.FormatName(upgrade.Id));
                name.AddToClassList("upgrade__name");
                button.Add(name);

                Label arrow = new("from");
                arrow.AddToClassList("upgrade__from-label");
                button.Add(arrow);

                List<Image> from = new();

                foreach (string id in upgrade.From)
                {
                    Image fromImage = UISprites.CreateImage(null, "upgrade__from");
                    button.Add(fromImage);
                    from.Add(fromImage);
                }

                list.Add(button);
                rows.Add((upgrade, button, image, from));
            }
        }

        public void Refresh()
        {
            GameState   state = controller.GetState();
            Piece.Color turn  = state.GetTurn();
            bool        open  = state.GetDecision() == null && !state.IsOver();

            foreach ((Upgrade upgrade, Button button, Image image, List<Image> from) in rows)
            {
                image.sprite = sprites.GetPiece(upgrade.Id, turn);

                for (int i = 0; i < from.Count; i++)
                    from[i].sprite = sprites.GetPiece(upgrade.From[i], turn);

                button.SetEnabled(open && state.GetGold(turn) >= UpgradeCatalog.Cost);
                button.EnableInClassList("upgrade--active",
                    controller.GetMode() == GameController.Mode.Upgrade && controller.GetModeItem() == upgrade.Id);
            }
        }
    }
}
