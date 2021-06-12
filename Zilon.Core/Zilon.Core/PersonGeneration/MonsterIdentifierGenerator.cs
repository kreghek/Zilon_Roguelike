using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.PersonGeneration
{
    /// <summary>
    /// Base implementation of identifier provider.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class MonsterIdentifierGenerator : IMonsterIdentifierGenerator
    {
        private int _currentIdCounter;

        /// <inheritdoc />
        public int GetNewId()
        {
            _currentIdCounter++;

            return _currentIdCounter;
        }
    }
}