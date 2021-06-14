using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Props;

namespace Zilon.Core.Tests.Common
{
    [ExcludeFromCodeCoverage]
    public class TestPropItemViewModel : IPropItemViewModel
    {
        public IProp Prop { get; set; }
    }
}