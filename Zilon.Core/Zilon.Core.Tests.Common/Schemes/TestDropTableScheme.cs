using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestDropTableScheme : SchemeBase, IDropTableScheme
    {
        public DropTableRecordSubScheme[] Records { get; }
        public int Rolls { get; }

        
        public TestDropTableScheme(int rolls, params DropTableRecordSubScheme[] records)
        {
            Rolls = rolls;
            Records = records;
        }
    }
}
