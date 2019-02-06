using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestDropTableScheme : SchemeBase, IDropTableScheme
    {
        public IDropTableRecordSubScheme[] Records { get; set; }

        public int Rolls { get; }


        public TestDropTableScheme(int rolls, params IDropTableRecordSubScheme[] records)
        {
            Rolls = rolls;
            Records = records;
        }
    }
}
