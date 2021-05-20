﻿using System;
using System.Runtime.Serialization;

namespace Zilon.Core.MassSectorGenerator
{
    /// <summary>
    /// Выбрасывается в процессе проверки валидности сектора.
    /// </summary>
    [Serializable]
    public class SectorValidationException : Exception
    {
        public SectorValidationException() { }
        public SectorValidationException(string message) : base(message) { }
        public SectorValidationException(string message, Exception inner) : base(message, inner) { }

        protected SectorValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}