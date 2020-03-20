using Zilon.Core.World;

namespace Zilon.Core.Client
{
    public interface IGlobeNodeViewModel: ISelectableViewModel
    {
        ProvinceNode Node { get; }

        Province ParentRegion { get; }
    }
}
