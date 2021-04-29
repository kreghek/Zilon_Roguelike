using System;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public static class IPersonExtensions
    {
        public static TPersonModule GetModule<TPersonModule>(this IPerson person) where TPersonModule : IPersonModule
        {
            if (person is null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            return person.GetModule<TPersonModule>(typeof(TPersonModule).Name);
        }

        public static TPersonModule GetModuleSafe<TPersonModule>(this IPerson source)
            where TPersonModule : IPersonModule
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.HasModule<TPersonModule>())
            {
#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.
            }

            return source.GetModule<TPersonModule>();
        }

        /// <inheritdoc />
        public static bool HasModule<TPersonModule>(this IPerson person) where TPersonModule : IPersonModule
        {
            if (person is null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            return person.HasModule(typeof(TPersonModule).Name);
        }
    }
}