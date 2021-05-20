using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public sealed class TestJobSubScheme : IJobSubScheme
    {
        public string[] Data { get; }
        public JobScope Scope { get; }
        public JobType Type { get; set; }
        public int Value { get; set; }
    }
}