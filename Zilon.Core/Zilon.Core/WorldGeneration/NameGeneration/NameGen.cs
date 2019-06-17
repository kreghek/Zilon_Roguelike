using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Zilon.Core.CommonServices.Dices;
using System.Reflection;

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
        List<string> Male;
        List<string> Female;
        List<string> Last;

        /// <summary>
        /// Initialises a new instance of the RandomName class.
        /// </summary>
        /// <param name="rand">A Random that is used to pick names</param>
        public RandomName(IDice dice)
        {
            _dice = dice;
            NameList l = new NameList();

            JsonSerializer serializer = new JsonSerializer();

            var assembly = this.GetType().Assembly;
            var resourceName = "Zilon.Core.WorldGeneration.NameGeneration.names.json";


            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            using (JsonReader jreader = new JsonTextReader(reader))
            {
                l = serializer.Deserialize<NameList>(jreader);
            }

            Male = new List<string>(l.boys);
            Female = new List<string>(l.girls);
            Last = new List<string>(l.last);
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
            string first = sex == Sex.Male ? Male[_dice.Roll(0, Male.Count - 1)] : Female[_dice.Roll(0, Female.Count - 1)]; // determines if we should select a name from male or female, and randomly picks
            string last = Last[_dice.Roll(0, Last.Count - 1)]; // gets the last name

            List<string> middles = new List<string>();

            for (int i = 0; i < middle; i++)
            {
                if (isInital)
                {
                    middles.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ"[_dice.Roll(0, 25 - 1)].ToString() + "."); // randomly selects an uppercase letter to use as the inital and appends a dot
                }
                else
                {
                    middles.Add(sex == Sex.Male ? Male[_dice.Roll(0 ,Male.Count - 1)] : Female[_dice.Roll(0 , Female.Count - 1)]); // randomly selects a name that fits with the sex of the person
                }
            }

            StringBuilder b = new StringBuilder();
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