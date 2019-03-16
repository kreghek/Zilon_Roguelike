using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Client
{
    public interface IMapNodeViewModel: ISelectableViewModel
    {
        HexNode Node { get; set; }
    }
}
