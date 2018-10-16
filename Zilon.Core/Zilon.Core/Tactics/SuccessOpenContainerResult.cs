using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Props;

namespace Zilon.Core.Tactics
{
    public class SuccessOpenContainerResult: OpenContainerResultBase
    {
        [ExcludeFromCodeCoverage]
        public SuccessOpenContainerResult(IProp[] props)
        {
            Props = props;
        }

        public IProp[] Props { get; }
    }
}
