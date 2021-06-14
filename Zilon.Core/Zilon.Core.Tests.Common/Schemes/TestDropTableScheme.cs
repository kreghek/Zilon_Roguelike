using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestDropTableScheme : SchemeBase, IDropTableScheme
    {
        public TestDropTableScheme(int rolls, params IDropTableRecordSubScheme[] records)
        {
            Rolls = rolls;
            Records = records;
        }

        public IDropTableRecordSubScheme[] Records { get; set; }

        public int Rolls { get; }
    }
}