using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public sealed class TestPersonScheme : SchemeBase, IPersonScheme
    {
        /// <inheritdoc />
        public string DefaultAct { get; set; }

        /// <inheritdoc />
        public int Hp { get; set; }

        /// <inheritdoc />
        public PersonSlotSubScheme[] Slots { get; set; }

        /// <inheritdoc />
        public IPersonSurvivalStatSubScheme[] SurvivalStats { get; set; }

        /// <inheritdoc />
        public string[] DefaultActs { get; set; }
    }
}