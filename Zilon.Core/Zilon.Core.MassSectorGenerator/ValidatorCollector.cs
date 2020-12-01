using System;
using System.Linq;
using System.Reflection;

namespace Zilon.Core.MassSectorGenerator
{
    internal static class ValidatorCollector
    {
        public static ISectorValidator[] GetValidatorsInAssembly()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var validatorTypes = thisAssembly.GetTypes()
                .Where(x => typeof(ISectorValidator).IsAssignableFrom(x))
                .Where(x => !x.IsInterface && !x.IsAbstract);

            var validators = validatorTypes.Select(x => Activator.CreateInstance(x)).Cast<ISectorValidator>();

            return validators.ToArray();
        }
    }
}
