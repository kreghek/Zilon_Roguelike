using Zilon.Core.World;

namespace Zilon.Core.Client
{
    public interface IGlobeNodeViewModel: ISelectableViewModel
    {
        GlobeRegionNode Node { get; }

        GlobeRegion ParentRegion { get; }
    }
}
