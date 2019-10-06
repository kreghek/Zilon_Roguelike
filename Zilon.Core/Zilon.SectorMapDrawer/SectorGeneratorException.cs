using System;

namespace Zilon.SectorGegerator
{
    [Serializable]
    public class SectorGeneratorException : Exception
    {
        public SectorGeneratorException() { }
        public SectorGeneratorException(string message) : base(message) { }
        public SectorGeneratorException(string message, Exception inner) : base(message, inner) { }
        protected SectorGeneratorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
