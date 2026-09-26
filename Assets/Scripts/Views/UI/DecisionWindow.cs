using System.Collections.Generic;
using Gameplay;
using Gameplay.Decisions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Views.UI
{
    #nullable enable

    /// <summary>
    /// Shows the current decision. Can't be closed.
    /// </summary>
    public class DecisionWindow : Window
    {
        private readonly GameController controller;
        private readonly UISprites      sprites;

        private Decision? shown;

        public DecisionWindow(GameController controller, UISprites sprites) : base("", false)
        {
            this.controller = controller;
            this.sprites    = sprites;

            AddToClassList("decision");
        }

        public Decision? GetShown() => shown;

        /// <summary>
        /// Rebuilds the content if the decision changed.
        /// </summary>
        public void Show(Decision decision)
        {
            if (decision == shown) return;

            shown = decision;

            GameState     state   = controller.GetState();
            VisualElement content = GetContent();

            content.Clear();

            SetTitle(decision.GetTitle());

            Label player = new($"{decision.GetPlayer()} decides");
            player.AddToClassList("window__hint");
            content.Add(player);

            Sprite? icon = sprites.Get(decision.GetIcon());

            if (icon != null) content.Add(UISprites.CreateImage(icon, "decision__icon"));

            if (decision.GetTargetBoard() is { } board)
            {
                Label hint = new($"Click a highlighted piece on the {board} board.");
                hint.AddToClassList("decision__instruction");
                content.Add(hint);

                return;
            }

            IReadOnlyList<string> options = decision.GetOptions(state);
            IReadOnlyList<Rule>?  rules   = decision switch
            {
                RuleDecision       => state.GetRulePool(),
                RemoveRuleDecision => state.GetRules(),
                _                  => null
            };

            ScrollView list = new();
            list.AddToClassList("decision__options");
            content.Add(list);

            for (int i = 0; i < options.Count; i++)
            {
                int option = i;

                Button button = new(() => controller.ChooseOption(option));
                button.AddToClassList("decision__option");

                if (rules != null && i < rules.Count)
                    button.Add(UISprites.CreateImage(sprites.GetRule(rules[i]), "decision__option-image"));

                Label text = new(options[i]);
                text.AddToClassList("decision__option-text");
                button.Add(text);

                list.Add(button);
            }
        }

        public void Hide()
        {
            shown = null;

            Close();
        }
    }
}
