using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Components
{
    /// <summary>
    /// Набор ресурсов. Используется для крафта, диалогов, прокачки перков.
    /// </summary>
    public sealed class PropSet
    {
        public string PropSid { get; set; }
        public int Count { get; set; }

        [ExcludeFromCodeCoverage]
        public PropSet Clone()
        {
            return new PropSet
            {
                PropSid = PropSid,
                Count = Count
            };
        }
    }
}
