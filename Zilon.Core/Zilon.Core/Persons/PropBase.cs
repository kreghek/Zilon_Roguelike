using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public abstract class PropBase
    {
        // ReSharper disable once EmptyConstructor
        protected PropBase()
        {
        }

        public PropScheme Scheme { get; set; }
    }
}
