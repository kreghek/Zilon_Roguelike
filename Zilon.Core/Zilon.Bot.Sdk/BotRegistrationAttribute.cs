using System;

namespace Zilon.Bot.Sdk
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class BotRegistrationAttribute : Attribute
    {
    }
}