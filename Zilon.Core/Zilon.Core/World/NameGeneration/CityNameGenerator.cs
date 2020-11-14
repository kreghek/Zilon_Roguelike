using System.Globalization;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.World.NameGeneration
{
    public sealed class CityNameGenerator
    {
        private IDice _dice;

        public CityNameGenerator(IDice dice)
        {
            _dice = dice;
        }

        public string Generate()
        {
            // Портирован скрипт генерации с этого ресурса.
            // https://www.fantasynamegenerators.com/city-names.php
            var names1 = new[] { "a", "e", "i", "o", "u", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            var names2 = new[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z", "br", "cr", "dr", "fr", "gr", "kr", "pr", "qr", "sr", "tr", "vr", "wr", "yr", "zr", "str", "bl", "cl", "fl", "gl", "kl", "pl", "sl", "vl", "yl", "zl", "ch", "kh", "ph", "sh", "yh", "zh" };
            var names3 = new[] { "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "ae", "ai", "au", "aa", "ee", "ea", "eu", "ia", "ie", "oi", "ou", "ua", "ue", "ui", "uo", "uu", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "a", "e", "i", "o", "u" };
            var names4 = new[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z", "br", "cr", "dr", "fr", "gr", "kr", "pr", "tr", "vr", "wr", "zr", "st", "bl", "cl", "fl", "gl", "kl", "pl", "sl", "vl", "zl", "ch", "kh", "ph", "sh", "zh" };
            var names5 = new[] { "c", "d", "f", "h", "k", "l", "m", "n", "p", "r", "s", "t", "x", "y", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            var names6 = new[] { "aco", "ada", "adena", "ago", "agos", "aka", "ale", "alo", "am", "anbu", "ance", "and", "ando", "ane", "ans", "anta", "arc", "ard", "ares", "ario", "ark", "aso", "athe", "eah", "edo", "ego", "eigh", "eim", "eka", "eles", "eley", "ence", "ens", "ento", "erton", "ery", "esa", "ester", "ey", "ia", "ico", "ido", "ila", "ille", "in", "inas", "ine", "ing", "irie", "ison", "ita", "ock", "odon", "oit", "ok", "olis", "olk", "oln", "ona", "oni", "onio", "ont", "ora", "ord", "ore", "oria", "ork", "osa", "ose", "ouis", "ouver", "ul", "urg", "urgh", "ury" };
            var names7 = new[] { "bert", "bridge", "burg", "burgh", "burn", "bury", "bus", "by", "caster", "cester", "chester", "dale", "dence", "diff", "ding", "don", "fast", "field", "ford", "gan", "gas", "gate", "gend", "ginia", "gow", "ham", "hull", "land", "las", "ledo", "lens", "ling", "mery", "mond", "mont", "more", "mouth", "nard", "phia", "phis", "polis", "pool", "port", "pus", "ridge", "rith", "ron", "rora", "ross", "rough", "sa", "sall", "sas", "sea", "set", "sey", "shire", "son", "stead", "stin", "ta", "tin", "tol", "ton", "vale", "ver", "ville", "vine", "ving", "well", "wood" };

            var i = _dice.Roll(0, 10);

            string names;
            if (i < 3)
            {
                var rnd0 = _dice.Roll(0, names1.Length - 1);
                var rnd1 = _dice.Roll(0, names2.Length - 1);
                var rnd2 = _dice.Roll(0, names3.Length - 1);
                var rnd3 = _dice.Roll(0, names5.Length - 1);
                while (names5[rnd3] == names2[rnd1])
                {
                    rnd3 = _dice.Roll(0, names5.Length - 1);
                }
                var rnd4 = _dice.Roll(0, names7.Length - 1);
                names = names1[rnd0] + names2[rnd1] + names3[rnd2] + names5[rnd3] + names7[rnd4];
            }
            else if (i < 5)
            {
                var rnd3 = _dice.Roll(0, names4.Length - 1);
                var rnd4 = _dice.Roll(0, names3.Length - 1);
                var rnd5 = _dice.Roll(0, names5.Length - 1);
                while (names5[rnd5] == names4[rnd3])
                {
                    rnd5 = _dice.Roll(0, names5.Length - 1);
                }
                var rnd6 = _dice.Roll(0, names7.Length - 1);
                names = names4[rnd3] + names3[rnd4] + names5[rnd5] + names7[rnd6];
            }
            else if (i < 8)
            {
                var rnd0 = _dice.Roll(0, names1.Length - 1);
                var rnd1 = _dice.Roll(0, names2.Length - 1);
                var rnd2 = _dice.Roll(0, names6.Length - 1);
                names = names1[rnd0] + names2[rnd1] + names6[rnd2];
            }
            else
            {
                var rnd3 = _dice.Roll(0, names7.Length - 1);
                var rnd4 = _dice.Roll(0, names6.Length - 1);
                names = names6[rnd4] + names7[rnd3];
            }

            // Culture en-US fixed, because algorithm focused on this culture.
            return names.FirstCharToUpper(CultureInfo.GetCultureInfo("en-US"));
        }
    }
}