using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class TestPersonScheme : SchemeBase, IPersonScheme
    {
        public string DefaultAct { get; set; }
        public int Hp { get; set; }
        public PersonSlotSubScheme[] Slots { get; set; }
        public IPersonSurvivalStatSubScheme[] SurvivalStats { get; set; }
    }
}