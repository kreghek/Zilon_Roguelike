using System.Collections.Generic;

namespace Assets.Zilon.Scripts.Services
{
    static class StaticPhrases
    {
        private static Dictionary<string, MultilangPhrase> _pharses;

        static StaticPhrases()
        {
            _pharses = InitPhrases();
        }

        public static string GetValue(string key, Language language)
        {
            if (_pharses.TryGetValue(key, out var phrase))
            {
                switch (language)
                {
                    case Language.English:
                    default:
                        return phrase.En ?? "[no-name]";

                    case Language.Russian:
                        return phrase.Ru ?? "[не определено]";
                }
            }

            return key;
        }

        private static Dictionary<string, MultilangPhrase> InitPhrases()
        {
            return new Dictionary<string, MultilangPhrase> 
            {
                { "prop-tag-weapon", new MultilangPhrase{ En = "weapon", Ru = "оружие" } },
                { "prop-tag-armor", new MultilangPhrase{ En = "armor", Ru = "броня" } },
                { "prop-tag-ranged", new MultilangPhrase{ En = "ranged", Ru = "дистанционное" } },

                { "prop-tag-pole", new MultilangPhrase{ En = "pole", Ru = "древковое" } },
                { "prop-tag-sword", new MultilangPhrase{ En = "sword", Ru = "меч" } },
                { "prop-tag-axe", new MultilangPhrase{ En = "axe", Ru = "топор" } },
                { "prop-tag-mace", new MultilangPhrase{ En = "mace", Ru = "булава" } },
                { "prop-tag-pistol", new MultilangPhrase{ En = "pistol", Ru = "пистолет" } },
                { "prop-tag-bow", new MultilangPhrase{ En = "bow", Ru = "лук" } },
                { "prop-tag-staff", new MultilangPhrase{ En = "staff", Ru = "посох" } },
                { "prop-tag-shield", new MultilangPhrase{ En = "shield", Ru = "щит" } },

                { "impact-kinetic", new MultilangPhrase{ En = "kinetic", Ru = "кинетический" } },
                { "impact-psy", new MultilangPhrase{ En = "psy", Ru = "пси" } },
                { "impact-termal", new MultilangPhrase{ En = "termal", Ru = "термальный" } },
                { "impact-explosion", new MultilangPhrase{ En = "explosion", Ru = "взрывной" } },
                { "impact-acid", new MultilangPhrase{ En = "acid", Ru = "кислотный" } },

                { "rule-lesser", new MultilangPhrase{ En = "lesser", Ru = "слабо" } },
                { "rule-normal", new MultilangPhrase{ En = "normal", Ru = "нормально" } },
                { "rule-grand", new MultilangPhrase{ En = "grand", Ru = "сильно" } },
                { "rule-absolute", new MultilangPhrase{ En = "absolute", Ru = "абсолютно" } },

                { "rule-satiety", new MultilangPhrase{ En = "Satiety", Ru = "Сытость" } },
                { "rule-thirst", new MultilangPhrase{ En = "Thirst", Ru = "Напитость" } },
                { "rule-health", new MultilangPhrase{ En = "Health", Ru = "Здоровье" } },
                { "rule-intoxication", new MultilangPhrase{ En = "Intoxication", Ru = "Токсины" } },

                { "rule-healthifnobody", new MultilangPhrase{ En = "Health withour body armor", Ru = "Здоровье без нательной брони" } },
                { "rule-hungerresistance", new MultilangPhrase{ En = "Hunger resistance", Ru = "Сопротивление голоду" } },
                { "rule-thristresistance", new MultilangPhrase{ En = "Thrist resistance", Ru = "Сопротивление жажде" } },

                { "ap-rank", new MultilangPhrase{ En = "rank", Ru = "ранг" } },
                { "armor-rank", new MultilangPhrase{ En = "rank", Ru = "ранг" } },
                { "efficient-heal", new MultilangPhrase{ En = "heal", Ru = "лечение" } },
                { "armor-protects", new MultilangPhrase{ En = "Protects", Ru = "Защита" } },
                { "prop-durable", new MultilangPhrase{ En = "Durable", Ru = "Прочность" } },
                { "prop-bonus", new MultilangPhrase{ En = "Bonus", Ru = "Бонус" } },
                { "prop-penalty", new MultilangPhrase{ En = "Penalty", Ru = "Штраф" } },
                { "no-bullets", new MultilangPhrase{ En = "NO BULLETS", Ru = "НЕТ СНАРЯДОВ" } },

                { "tooltip-no-bullets", new MultilangPhrase{ En = "No bullets for current weapon", Ru = "В инвентаре нет подходящих снарядов для текущего оружия" } },
                { "tooltip-wait", new MultilangPhrase{ En = "Wait one turn", Ru = "Подождать один ход" } },
                { "tooltip-person", new MultilangPhrase{ En = "Person window", Ru = "Окно персонажа" } },
                { "tooltip-transition", new MultilangPhrase{ En = "Go to the next level", Ru = "Перейти на следующий уровень" } },
                { "tooltip-exit-title", new MultilangPhrase{ En = "Quit game and go to Main menu", Ru = "Покинуть игру и перейти в главное меню" } },
                { "tooltip-exit", new MultilangPhrase{ En = "Quit game", Ru = "Покинуть игру" } },
            };
        }

        class MultilangPhrase
        {
            public string En { get; set; }

            public string Ru { get; set; }
        }
    }
}
