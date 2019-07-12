using System;

namespace Zilon.Bot.Sdk
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class AvailableModesAttribute : Attribute
    {
        public AvailableModesAttribute()
        {
           
        }
    }
}
