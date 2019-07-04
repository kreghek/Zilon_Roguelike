using System;

namespace Zilon.Core.Persons
{

    [Serializable]
    public class CreatePersonException : Exception
    {
        public CreatePersonException() { }
        public CreatePersonException(string message) : base(message) { }
        public CreatePersonException(string message, Exception inner) : base(message, inner) { }
        protected CreatePersonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
