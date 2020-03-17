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

                { "state-hp-healthy", new MultilangPhrase{ En = "Healthy", Ru = "Здоров" } },
                { "state-hp-slightly-injured", new MultilangPhrase{ En = "Slightly injured", Ru = "Слабо ранен" } },
                { "state-hp-wounded", new MultilangPhrase{ En = "Wounded", Ru = "Ранен" } },
                { "state-hp-badly-wounded", new MultilangPhrase{ En = "Badly wounded", Ru = "Серьезно ранен" } },
                { "state-hp-at-death", new MultilangPhrase{ En = "At Death", Ru = "Присмерти" } },

                { "ap-rank", new MultilangPhrase{ En = "rank", Ru = "ранг" } },
                { "armor-rank", new MultilangPhrase{ En = "rank", Ru = "ранг" } },
                { "efficient-heal", new MultilangPhrase{ En = "heal", Ru = "лечение" } },
                { "armor-protects", new MultilangPhrase{ En = "Protects", Ru = "Защита" } },
                { "prop-durable", new MultilangPhrase{ En = "Durable", Ru = "Прочность" } },
                { "prop-bonus", new MultilangPhrase{ En = "Bonus", Ru = "Бонус" } },
                { "prop-penalty", new MultilangPhrase{ En = "Penalty", Ru = "Штраф" } },
                { "no-bullets", new MultilangPhrase{ En = "NO BULLETS", Ru = "НЕТ СНАРЯДОВ" } },

                { "tooltip-no-bullets", new MultilangPhrase{ En = "No bullets for current weapon", Ru = "В инвентаре нет подходящих снарядов для текущего оружия" } },
                { "tooltip-wait", new MultilangPhrase{ En = "Wait one turn [E]", Ru = "Подождать один ход [E]" } },
                { "tooltip-person", new MultilangPhrase{ En = "Person window [P]", Ru = "Окно персонажа [P]" } },
                { "tooltip-transition", new MultilangPhrase{ En = "Go to the next level [T]", Ru = "Перейти на следующий уровень [T]" } },
                { "tooltip-open-loot", new MultilangPhrase{ En = "Open loot [O]", Ru = "Открыть лут [O]" } },
                { "tooltip-exit-title", new MultilangPhrase{ En = "Quit game and go to Main menu", Ru = "Покинуть игру и перейти в главное меню" } },
                { "tooltip-exit", new MultilangPhrase{ En = "Quit game", Ru = "Покинуть игру" } },

                { "log-player-damage-template", new MultilangPhrase{ En = "Player damaged {0} on {1}", Ru = "Игрок ранил {0} на {1} ед. урона" } },
                { "log-monster-damage-template", new MultilangPhrase{ En = "{0} damaged plyer on {1}", Ru = "{0} ранил игрока на {1} ед. урона" } },

                { "indicator-lesser-wound", new MultilangPhrase{ En = "Lesser wound", Ru = "Легкая рана" } },
                { "indicator-serious-wound", new MultilangPhrase{ En = "Serious wound", Ru = "Серьезная рана" } },
                { "indicator-critical-wound", new MultilangPhrase{ En = "Critical wound", Ru = "Критическая рана" } },
                { "indicator-block", new MultilangPhrase{ En = "Block!", Ru = "Блок!" } },
                { "indicator-miss", new MultilangPhrase{ En = "Miss!", Ru = "Промах!" } },
                { "indicator-dodge", new MultilangPhrase{ En = "Dodge!", Ru = "Уклонение!" } },
                { "indicator-found-nothing", new MultilangPhrase{ En = "Found\nNoting!", Ru = "Ничего\nне найдено!" } },

                { "weak-injury", new MultilangPhrase{ En = "Weak injury", Ru = "Легкая рана" } },
                { "weak-hunger", new MultilangPhrase{ En = "Weak hunger", Ru = "Слабый голод" } },
                { "weak-thirst", new MultilangPhrase{ En = "Weak thirst", Ru = "Слабая жажда" } },
                { "weak-intoxication", new MultilangPhrase{ En = "Weak intoxication", Ru = "Слабая токсикация" } },

                { "strong-injury", new MultilangPhrase{ En = "Strong injury", Ru = "Серьзная рана" } },
                { "strong-hunger", new MultilangPhrase{ En = "Hunger", Ru = "Голод" } },
                { "strong-thirst", new MultilangPhrase{ En = "Thirst", Ru = "Жажда" } },
                { "strong-intoxication", new MultilangPhrase{ En = "Intoxication", Ru = "Токсакация" } },

                { "max-injury", new MultilangPhrase{ En = "Vital wound!", Ru = "Смертельная рана!" } },
                { "max-hunger", new MultilangPhrase{ En = "Starvation!", Ru = "Голодание!" } },
                { "max-thirst", new MultilangPhrase{ En = "Dehydration!", Ru = "Обезвоживание!" } },
                { "max-intoxication", new MultilangPhrase{ En = "Overdose!", Ru = "Передозировка!" } }
            };
        }

        class MultilangPhrase
        {
            public string En { get; set; }

            public string Ru { get; set; }
        }
    }
}
