using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.NameGeneration
{
    /// <summary>
    /// RandomName class, used to generate a random name.
    /// </summary>
    public class RandomName
    {
        /// <summary>
        /// Class for holding the lists of names from names.json
        /// </summary>
        class NameList
        {
            public string[] boys { get; set; }
            public string[] girls { get; set; }
            public string[] last { get; set; }

            public NameList()
            {
                boys = new string[] { };
                girls = new string[] { };
                last = new string[] { };
            }
        }

        IDice _dice;
        private readonly List<string> _male;
        private readonly List<string> _female;
        private readonly List<string> _last;

        /// <summary>
        /// Initialises a new instance of the RandomName class.
        /// </summary>
        public RandomName(IDice dice)
        {
            _dice = dice;

            JsonSerializer serializer = new JsonSerializer();

            var assembly = this.GetType().Assembly;
            var resourceName = "Zilon.Core.WorldGeneration.NameGeneration.names.json";


            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            using (JsonReader jreader = new JsonTextReader(reader))
            {
                var nameList = serializer.Deserialize<NameList>(jreader);

                _male = new List<string>(nameList.boys);
                _female = new List<string>(nameList.girls);
                _last = new List<string>(nameList.last);
            }
        }

        /// <summary>
        /// Returns a new random name
        /// </summary>
        /// <param name="sex">The sex of the person to be named. true for male, false for female</param>
        /// <param name="middle">How many middle names do generate</param>
        /// <param name="isInital">Should the middle names be initials or not?</param>
        /// <returns>The random name as a string</returns>
        public string Generate(Sex sex, int middle = 0, bool isInital = false)
        {
            var first = sex == Sex.Male ? _male[_dice.Roll(0, _male.Count - 1)] : _female[_dice.Roll(0, _female.Count - 1)]; // determines if we should select a name from male or female, and randomly picks
            var last = _last[_dice.Roll(0, _last.Count - 1)]; // gets the last name

            List<string> middles = new List<string>();

            for (int i = 0; i < middle; i++)
            {
                if (isInital)
                {
                    middles.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_dice.Roll(0, 25 - 1)].ToString() + "."); // randomly selects an uppercase letter to use as the inital and appends a dot
                }
                else
                {
                    middles.Add(sex == Sex.Male ? _male[_dice.Roll(0, _male.Count - 1)] : _female[_dice.Roll(0, _female.Count - 1)]); // randomly selects a name that fits with the sex of the person
                }
            }

            var b = new StringBuilder();
            b.Append(first + " "); // put a space after our names;
            foreach (string m in middles)
            {
                b.Append(m + " ");
            }
            b.Append(last);

            return b.ToString();
        }

        /// <summary>
        /// Generates a list of random names
        /// </summary>
        /// <param name="number">The number of names to be generated</param>
        /// <param name="maxMiddleNames">The maximum number of middle names</param>
        /// <param name="sex">The sex of the names, if null sex is randomised</param>
        /// <param name="initials">Should the middle names have initials, if null this will be randomised</param>
        /// <returns>List of strings of names</returns>
        public List<string> RandomNames(int number, int maxMiddleNames, Sex? sex = null, bool? initials = null)
        {
            List<string> names = new List<string>();

            for (int i = 0; i < number; i++)
            {
                if (sex != null && initials != null)
                {
                    names.Add(Generate((Sex)sex, _dice.Roll(0, maxMiddleNames + 1 - 1), (bool)initials));
                }
                else if (sex != null)
                {
                    bool init = _dice.Roll(0, 2 - 1) != 0;
                    names.Add(Generate((Sex)sex, _dice.Roll(0, maxMiddleNames + 1 - 1), init));
                }
                else if (initials != null)
                {
                    Sex s = (Sex)_dice.Roll(0, 2 - 1);
                    names.Add(Generate(s, _dice.Roll(0, maxMiddleNames + 1 - 1), (bool)initials));
                }
                else
                {
                    Sex s = (Sex)_dice.Roll(0, 2 - 1);
                    bool init = _dice.Roll(0, 2 - 1) != 0;
                    names.Add(Generate(s, _dice.Roll(0, maxMiddleNames + 1 - 1), init));
                }
            }

            return names;
        }
    }

    public enum Sex
    {
        Male,
        Female
    }
}