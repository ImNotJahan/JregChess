using Gameplay;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    public class RulesWindow : Window
    {
        private readonly GameController controller;
        private readonly UISprites      sprites;
        private readonly ScrollView     list;

        public RulesWindow(GameController controller, UISprites sprites) : base("Rules")
        {
            this.controller = controller;
            this.sprites    = sprites;

            AddToClassList("rules");

            list = new ScrollView();
            list.AddToClassList("rule-list");
            GetContent().Add(list);
        }

        public void Refresh()
        {
            GameState state = controller.GetState();

            list.Clear();

            foreach (Rule rule in state.GetRules()) list.Add(CreateRow(rule));
        }

        private VisualElement CreateRow(Rule rule)
        {
            VisualElement row = new();
            row.AddToClassList("rule");

            row.Add(UISprites.CreateImage(sprites.GetRule(rule), "rule__image"));

            Label description = new(Rules.GetDescription(rule));
            description.AddToClassList("rule__text");
            row.Add(description);

            return row;
        }
    }
}
