using System;

namespace Zilon.Core.Tactics
{
    public static class IStaticObjectExtensions
    {
        public static TStaticObjectModule GetModuleSafe<TStaticObjectModule>(this IStaticObject source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.HasModule<TStaticObjectModule>())
            {
                return default;
            }

            return source.GetModule<TStaticObjectModule>();
        }
    }
}
