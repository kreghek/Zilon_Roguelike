using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.World.NameGeneration
{
    /// <summary>
    ///     RandomName class, used to generate a random name.
    /// </summary>
    public class RandomName
    {
        private readonly IDice _dice;
        private readonly List<string> _female;
        private readonly List<string> _last;
        private readonly List<string> _male;

        /// <summary>
        ///     Initialises a new instance of the RandomName class.
        /// </summary>
        public RandomName(IDice dice)
        {
            _dice = dice;

            JsonSerializer serializer = new JsonSerializer();

            var assembly = this.GetType().Assembly;
            var resourceName = "Zilon.Core.World.NameGeneration.names.json";


            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            using (JsonReader jreader = new JsonTextReader(reader))
            {
                var nameList = serializer.Deserialize<NameList>(jreader);

                _male = new List<string>(nameList.Boys);
                _female = new List<string>(nameList.Girls);
                _last = new List<string>(nameList.Last);
            }
        }

        /// <summary>
        ///     Returns a new random name
        /// </summary>
        /// <param name="sex">The sex of the person to be named. true for male, false for female</param>
        /// <returns>The random name as a string</returns>
        public string Generate(Sex sex)
        {
            return Generate(sex, 0, false);
        }

        /// <summary>
        ///     Returns a new random name
        /// </summary>
        /// <param name="sex">The sex of the person to be named. true for male, false for female</param>
        /// <param name="middle">How many middle names do generate</param>
        /// <returns>The random name as a string</returns>
        public string Generate(Sex sex, int middle)
        {
            return Generate(sex, middle, false);
        }

        /// <summary>
        ///     Returns a new random name
        /// </summary>
        /// <param name="sex">The sex of the person to be named. true for male, false for female</param>
        /// <param name="isInital">Should the middle names be initials or not?</param>
        /// <returns>The random name as a string</returns>
        public string Generate(Sex sex, bool isInital)
        {
            return Generate(sex, 0, isInital);
        }

        /// <summary>
        ///     Returns a new random name
        /// </summary>
        /// <param name="sex">The sex of the person to be named. true for male, false for female</param>
        /// <param name="middle">How many middle names do generate</param>
        /// <param name="isInital">Should the middle names be initials or not?</param>
        /// <returns>The random name as a string</returns>
        public string Generate(Sex sex, int middle, bool isInital)
        {
            var first = sex == Sex.Male
                ? _male[_dice.Roll(0, _male.Count - 1)]
                : _female[
                    _dice.Roll(0,
                        _female.Count -
                        1)]; // determines if we should select a name from male or female, and randomly picks
            var last = _last[_dice.Roll(0, _last.Count - 1)]; // gets the last name

            List<string> middles = new List<string>();

            for (int i = 0; i < middle; i++)
            {
                if (isInital)
                {
                    middles.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_dice.Roll(0, 25 - 1)]
                                    .ToString(System.Globalization.CultureInfo.InvariantCulture) +
                                "."); // randomly selects an uppercase letter to use as the inital and appends a dot
                }
                else
                {
                    middles.Add(sex == Sex.Male
                        ? _male[_dice.Roll(0, _male.Count - 1)]
                        : _female[
                            _dice.Roll(0,
                                _female.Count - 1)]); // randomly selects a name that fits with the sex of the person
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
        ///     Generates a list of random names
        /// </summary>
        /// <param name="number">The number of names to be generated</param>
        /// <param name="maxMiddleNames">The maximum number of middle names</param>
        /// <returns>List of strings of names</returns>
        public List<string> RandomNames(int number, int maxMiddleNames)
        {
            return RandomNames(number, maxMiddleNames, null, null);
        }

        /// <summary>
        ///     Generates a list of random names
        /// </summary>
        /// <param name="number">The number of names to be generated</param>
        /// <param name="maxMiddleNames">The maximum number of middle names</param>
        /// <param name="sex">The sex of the names, if null sex is randomised</param>
        /// <returns>List of strings of names</returns>
        public List<string> RandomNames(int number, int maxMiddleNames, Sex? sex)
        {
            return RandomNames(number, maxMiddleNames, sex, null);
        }

        /// <summary>
        ///     Generates a list of random names
        /// </summary>
        /// <param name="number">The number of names to be generated</param>
        /// <param name="maxMiddleNames">The maximum number of middle names</param>
        /// <param name="initials">Should the middle names have initials, if null this will be randomised</param>
        /// <returns>List of strings of names</returns>
        public List<string> RandomNames(int number, int maxMiddleNames, bool? initials)
        {
            return RandomNames(number, maxMiddleNames, null, initials);
        }

        /// <summary>
        ///     Generates a list of random names
        /// </summary>
        /// <param name="number">The number of names to be generated</param>
        /// <param name="maxMiddleNames">The maximum number of middle names</param>
        /// <param name="sex">The sex of the names, if null sex is randomised</param>
        /// <param name="initials">Should the middle names have initials, if null this will be randomised</param>
        /// <returns>List of strings of names</returns>
        public List<string> RandomNames(int number, int maxMiddleNames, Sex? sex, bool? initials)
        {
            List<string> names = new List<string>(number);

            for (int i = 0; i < number; i++)
            {
                Sex s = sex != null ? sex.Value : (Sex)_dice.Roll(0, 2 - 1);
                bool init = initials != null ? (bool)initials : _dice.Roll(0, 2 - 1) != 0;
                int middle = _dice.Roll(0, (maxMiddleNames + 1) - 1);

                names.Add(Generate(s, middle, init));
            }

            return names;
        }

        /// <summary>
        ///     Class for holding the lists of names from names.json
        /// </summary>
        private class NameList
        {
            public NameList()
            {
                Boys = System.Array.Empty<string>();
                Girls = System.Array.Empty<string>();
                Last = System.Array.Empty<string>();
            }

            [JsonProperty(PropertyName = "boys")] public string[] Boys { get; set; }

            [JsonProperty(PropertyName = "girls")] public string[] Girls { get; set; }

            [JsonProperty(PropertyName = "last")] public string[] Last { get; set; }
        }
    }
}