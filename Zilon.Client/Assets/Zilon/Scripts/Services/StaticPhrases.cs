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

                { "prop-tag-pole", new MultilangPhrase{ En = "pole", Ru = "древковое" } },
                { "prop-tag-sword", new MultilangPhrase{ En = "sword", Ru = "меч" } },
                { "prop-tag-axe", new MultilangPhrase{ En = "axe", Ru = "топор" } },
                { "prop-tag-mace", new MultilangPhrase{ En = "mace", Ru = "булава" } },

                { "impact-kinetic", new MultilangPhrase{ En = "kinetic", Ru = "кинетический" } },
                { "impact-psy", new MultilangPhrase{ En = "psy", Ru = "пси" } },
                { "impact-termal", new MultilangPhrase{ En = "termal", Ru = "термальный" } },
                { "impact-explosion", new MultilangPhrase{ En = "explosion", Ru = "взрывной" } },
                { "impact-acid", new MultilangPhrase{ En = "acid", Ru = "кислотный" } },

                { "ap-rank", new MultilangPhrase{ En = "rank", Ru = "ранг" } },
                { "armor-rank", new MultilangPhrase{ En = "rank", Ru = "ранг" } },
                { "efficient-heal", new MultilangPhrase{ En = "heal", Ru = "лечение" } },
            };
        }

        class MultilangPhrase
        {
            public string En { get; set; }

            public string Ru { get; set; }
        }
    }
}
