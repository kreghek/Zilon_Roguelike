using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zilon.DependencyInjection
{
    /// <summary>
    /// Helper to group methods used during multiple implementations registration in DI.
    /// </summary>
    public static class ImplementationGatheringHelper
    {
        /// <summary>
        /// Get all types by specified interfact in assembly.
        /// </summary>
        /// <typeparam name="TInterface">Searching interface implementations.</typeparam>
        /// <param name="assembly">Assemby to scan.</param>
        /// <returns>Collection of specified interface implementations.</returns>
        public static IEnumerable<Type> GetImplementations<TInterface>(Assembly assembly)
        {
            var logicTypes = assembly.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsInterface && typeof(TInterface).IsAssignableFrom(x));
            return logicTypes;
        }
    }
}