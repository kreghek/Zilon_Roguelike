using System;

namespace Zilon.Core.Persons
{
    public class Disease : IDisease
    {
        public Disease(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}
