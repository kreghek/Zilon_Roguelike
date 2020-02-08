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
            };
        }

        class MultilangPhrase
        {
            public string En { get; set; }

            public string Ru { get; set; }
        }
    }
}
