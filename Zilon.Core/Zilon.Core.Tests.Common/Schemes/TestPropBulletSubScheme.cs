using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestPropBulletSubScheme : IPropBulletSubScheme
    {
        public string Caliber { get; set; }
    }
}