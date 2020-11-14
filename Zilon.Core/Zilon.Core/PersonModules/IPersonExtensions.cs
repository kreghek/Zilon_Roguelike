using System;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public static class IPersonExtensions
    {
        public static TPersonModule GetModuleSafe<TPersonModule>(this IPerson source)
            where TPersonModule : IPersonModule
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.HasModule<TPersonModule>())
            {
                return default;
            }

            return source.GetModule<TPersonModule>();
        }

        public static TPersonModule GetModule<TPersonModule>(this IPerson person) where TPersonModule : IPersonModule
        {
            if (person is null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            return person.GetModule<TPersonModule>(typeof(TPersonModule).Name);
        }

        /// <inheritdoc/>
        public static bool HasModule<TPersonModule>(this IPerson staticObject) where TPersonModule : IPersonModule
        {
            if (staticObject is null)
            {
                throw new ArgumentNullException(nameof(staticObject));
            }

            return staticObject.HasModule(typeof(TPersonModule).Name);
        }
    }
}