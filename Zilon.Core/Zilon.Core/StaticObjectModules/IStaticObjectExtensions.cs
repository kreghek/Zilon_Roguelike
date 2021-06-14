using System;

using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    public static class IStaticObjectExtensions
    {
        public static TStaticObjectModule GetModule<TStaticObjectModule>(this IStaticObject staticObject)
            where TStaticObjectModule : IStaticObjectModule
        {
            if (staticObject is null)
            {
                throw new ArgumentNullException(nameof(staticObject));
            }

            return staticObject.GetModule<TStaticObjectModule>(typeof(TStaticObjectModule).Name);
        }

        public static TStaticObjectModule GetModuleSafe<TStaticObjectModule>(this IStaticObject source)
            where TStaticObjectModule : IStaticObjectModule
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.HasModule<TStaticObjectModule>())
            {
#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.
            }

            return source.GetModule<TStaticObjectModule>();
        }

        /// <inheritdoc />
        public static bool HasModule<TStaticObjectModule>(this IStaticObject staticObject)
            where TStaticObjectModule : IStaticObjectModule
        {
            if (staticObject is null)
            {
                throw new ArgumentNullException(nameof(staticObject));
            }

            return staticObject.HasModule(typeof(TStaticObjectModule).Name);
        }
    }
}