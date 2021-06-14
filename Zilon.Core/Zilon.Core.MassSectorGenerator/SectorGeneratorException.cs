using System;
using System.Runtime.Serialization;

namespace Zilon.Core.MassSectorGenerator
{
    /// <summary>
    /// Исключение, которое выбрасывается в случае предстказуемой ошибки в работе рисовальщика.
    /// </summary>
    [Serializable]
    public class SectorGeneratorException : Exception
    {
        public SectorGeneratorException() { }
        public SectorGeneratorException(string message) : base(message) { }
        public SectorGeneratorException(string message, Exception inner) : base(message, inner) { }

        protected SectorGeneratorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}