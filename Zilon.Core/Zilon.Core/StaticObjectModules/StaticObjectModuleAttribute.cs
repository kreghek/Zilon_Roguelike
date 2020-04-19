using System;

namespace Zilon.Core.StaticObjectModules
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class StaticObjectModuleAttribute : Attribute
    {
    }
}
