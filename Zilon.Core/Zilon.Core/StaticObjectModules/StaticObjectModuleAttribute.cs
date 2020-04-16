using System;

namespace Zilon.Core.StaticObjectModules
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    internal sealed class StaticObjectModuleAttribute : Attribute
    {
    }
}
