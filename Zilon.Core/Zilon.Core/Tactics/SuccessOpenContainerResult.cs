using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    public class SuccessOpenContainerResult: OpenContainerResultBase
    {
        [ExcludeFromCodeCoverage]
        public SuccessOpenContainerResult(IProp[] props)
        {
            Props = props;
        }

        [ExcludeFromCodeCoverage]
        public IProp[] Props { get; }
    }
}
