using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    public interface IContainerViewModel: ISelectableViewModel
    {
        IPropContainer Container { get; set; }
    }
}
