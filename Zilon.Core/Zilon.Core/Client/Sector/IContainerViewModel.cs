using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    public interface IContainerViewModel: ISelectableViewModel
    {
        IStaticObject Container { get; set; }
    }
}
