using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public sealed class AppException : Exception
    {
        public AppException() { }
        public AppException(string message) : base(message) { }
        public AppException(string message, Exception inner) : base(message, inner) { }
        public AppException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
