using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Zilon.Core
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class AppException : Exception
    {
        public AppException() { }

        public AppException(string message) : base(message) { }
        public AppException(string message, Exception inner) : base(message, inner) { }

        protected AppException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}