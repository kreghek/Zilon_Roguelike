using System;
using System.Globalization;
using System.Linq;

namespace Zilon.Core.Common
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input, CultureInfo invariantCulture)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            switch (input)
            {
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper(invariantCulture) + input.Substring(1);
            }
        }
    }
}