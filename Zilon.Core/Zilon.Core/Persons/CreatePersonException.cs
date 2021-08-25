using System;
using System.Runtime.Serialization;

namespace Zilon.Core.Persons
{
    [Serializable]
    public class CreatePersonException : Exception
    {
        public CreatePersonException() { }
        public CreatePersonException(string message) : base(message) { }
        public CreatePersonException(string message, Exception inner) : base(message, inner) { }

        protected CreatePersonException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}