using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestDropTableRecordSubScheme : IDropTableRecordSubScheme
    {
        public int MaxCount { get; set; }

        public int MinCount { get; set; }

        public string SchemeSid { get; set; }

        public int Weight { get; set; }

        /// <summary>
        /// Дополнительный дроп.
        /// </summary>
        public IDropTableScheme[] Extra { get; set; }

        public static TestDropTableRecordSubScheme CreateEmpty(int weight)
        {
            return new TestDropTableRecordSubScheme
            {
                SchemeSid = null,
                Weight = weight
            };
        }
    }
}