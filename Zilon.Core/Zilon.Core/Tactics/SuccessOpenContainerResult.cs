using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    public class SuccessOpenContainerResult: OpenContainerResultBase
    {
        protected SuccessOpenContainerResult(IProp[] props)
        {
            Props = props;
        }

        public IProp[] Props { get; }

        
    }
}
